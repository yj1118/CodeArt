using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;

using CodeArt.Concurrent;
using CodeArt.Runtime;
using CodeArt.Runtime.IL;

namespace CodeArt.DTO
{
    /// <summary>
    /// <para>封装了根据类型获取序列化、反序列化的方法信息</para>
    /// <para>该类是一个重构用的对象</para>
    /// </summary>
    internal static class SerializationMethodHelper
    {
        private static readonly Dictionary<SerializationMethodType, Dictionary<Type, MethodInfo>> _typeMethods = new Dictionary<SerializationMethodType, Dictionary<Type, MethodInfo>>();

        #region 构造

        /// <summary>
        /// 静态构造中，初始化一个内部的静态集合
        /// </summary>
        static SerializationMethodHelper()
        {
            InitMethodCache();
        }

        /// <summary>
        /// 初始化方法缓存
        /// </summary>
        private static void InitMethodCache()
        {
            _typeMethods.Add(SerializationMethodType.Serialize, new Dictionary<Type, MethodInfo>());
            _typeMethods.Add(SerializationMethodType.Deserialize, new Dictionary<Type, MethodInfo>());
        }


        #endregion


        private static readonly ReaderWriterLockSlim _typeMethodsLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);


        #region 重构

        private static MethodInfo GetMethodByCache(Type type, SerializationMethodType methodType)
        {
            MethodInfo method = null;
            Dictionary<Type, MethodInfo> methodTypeMethods = null;

            _typeMethodsLock.Read(() =>
            {
                if (_typeMethods.TryGetValue(methodType, out methodTypeMethods))
                {
                    methodTypeMethods.TryGetValue(type, out method);
                }
            });
            return method;
        }


        private static MethodInfo GetMethodByEnum(Type type, SerializationMethodType methodType)
        {
            return GetTypeMethod(Enum.GetUnderlyingType(type), methodType);
        }

        private static MethodInfo GetMethodBySimple(Type type, SerializationMethodType methodType)
        {
            MethodInfo method = null;
            switch (methodType)
            {
                case SerializationMethodType.Serialize:
                    if (type.Name.StartsWith("Nullable"))
                    {
                        Type[] typeArgs = new Type[] { typeof(string), type };
                        method = typeof(IDTOWriter).ResolveMethod("Write", typeArgs);
                    }
                    else
                    {
                        Type[] typeArgs = new Type[] { typeof(string), type };
                        method = typeof(IDTOWriter).ResolveMethod("Write", typeArgs);
                        if ((method == null) && (type.IsPrimitive == false))
                        {
                            //如果不是int、long等基础类型，而有可能是自定义类型，那么用以下代码得到方法
                            method = typeof(IDTOWriter).ResolveMethod("Write", _writeObjectArgs);
                        }
                    }
                    break;
                case SerializationMethodType.Deserialize:
                    if (type.Name.StartsWith("Nullable"))
                    {
                        Type[] targs = type.GetGenericArguments();
                        string methodName = string.Format("ReadNullable{0}", targs[0].Name);
                        method = typeof(IDTOReader).ResolveMethod(methodName, _readArgs);
                    }
                    else
                    {
                        Type[] targs = type.GetGenericArguments();
                        string methodName = string.Format("Read{0}", type.Name);
                        method = typeof(IDTOReader).ResolveMethod(methodName, _readArgs);
                    }
                    if ((method == null) && (type.IsPrimitive == false))
                    {
                        //如果不是int、long等基础类型，而有可能是自定义类型，那么用以下代码得到方法
                        //IDTOWriter.ReadObject<T>(string name);
                        method = typeof(IDTOReader).ResolveMethod("ReadObject", new Type[] { type }, MethodParameter.Create<string>());
                    }
                    break;
            }

            if(method == null)
            {
                throw new DTOException("没有找到"+ type.ResolveName() + "的dto序列化方法");
            }

            return method;
        }

        private static Type[] _writeObjectArgs = new Type[] { typeof(string), typeof(object) };
        private static Type[] _readArgs = new Type[] { typeof(string) };

        #endregion


        /// <summary>
        /// 根据类型得到序列化方法
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodType"></param>
        /// <returns></returns>
        public static MethodInfo GetTypeMethod(Type type, SerializationMethodType methodType)
        {
            MethodInfo method = GetMethodByCache(type, methodType);
            if (method == null)
            {
                // 对于枚举类型：根据基础类型找到适当的序列化器方法
                // 对于简单类型：找到对应的 IPrimitiveReader/IPrimitveWriter 方法
                // 对于复杂类型：根据类型得到适当的序列化器方法
                if (type.IsEnum)
                    method = GetMethodByEnum(type, methodType);
                else
                    method = GetMethodBySimple(type, methodType);

                if (method == null)
                    throw new NotImplementedException(string.Format("无法得到类型的DTOSerialize或DTODeserialize方法，类型： {0}", type.FullName));

                //更新方法缓存
                _typeMethodsLock.Write(() => _typeMethods[methodType][type] = method);

            }

            return method;
        }

        /// <summary>
        /// 获取方法的拥有者在参数中的序号
        /// </summary>
        /// <param name="method"></param>
        /// <param name="methodType"></param>
        /// <returns></returns>
        public static int GetParameterIndex(MethodInfo method, SerializationMethodType methodType)
        {
            string typeName = method.DeclaringType.Name;
            if (methodType == SerializationMethodType.Serialize)
            {
                if (typeName == typeof(DTObjectSerializer).Name) return SerializationArgs.SerializerIndex;
                if (typeName == typeof(IDTOWriter).Name) return SerializationArgs.WriterIndex;
            }
            else
            {
                if (typeName == typeof(DTObjectDeserializer).Name) return SerializationArgs.DeserializerIndex;
                if (typeName == typeof(IDTOReader).Name) return SerializationArgs.ReaderIndex;
            }
            throw new SerializationException("没有找到类型" + typeName + "对应的参数序号");
        }


        /// <summary>
        /// <para>得到写入某个类型的IL代码</para>
        /// <para>writer.Write(value); 或 serialzer.Serialze(value);</para>
        /// </summary>
        /// <param name="g"></param>
        /// <param name="valueType"></param>
        /// <param name="loadValue"></param>
        public static void Write(MethodGenerator g, string dtoMemberName, Type valueType, Action<Type> loadValue)
        {
            var method = SerializationMethodHelper.GetTypeMethod(valueType, SerializationMethodType.Serialize);
            var prmIndex = SerializationMethodHelper.GetParameterIndex(method, SerializationMethodType.Serialize);
            g.Call(method, () =>
            {
                g.LoadParameter(prmIndex);
                g.Load(dtoMemberName);
                var argType = method.GetParameters()[1].ParameterType;
                loadValue(argType);



                //if (prmIndex == SerializationArgs.SerializerIndex)
                //{
                //    //是serializer.Serializ();
                //    g.LoadVariable(SerializationArgs.TypeNameTable);
                //}
            });
        }

        public static bool IsPrimitive(Type type)
        {
            return type == typeof(string)
                || type == typeof(int)
                || type == typeof(long)
                || type == typeof(DateTime)
                || type == typeof(Guid)
                || type == typeof(float)
                || type == typeof(double)
                || type == typeof(uint)
                || type == typeof(ulong)
                || type == typeof(ushort)
                || type == typeof(sbyte)
                || type == typeof(char)
                || type == typeof(byte)
                || type == typeof(bool)
                || type == typeof(decimal)
                || type == typeof(short);
        }

        public static void WriteBlob(MethodGenerator g, string dtoMemberName, Action loadValue)
        {
            var method = typeof(IDTOWriter).ResolveMethod("WriteBlob",
                                                            new Type[] { typeof(string), typeof(byte[]) });
            var prmIndex = SerializationArgs.WriterIndex;
            g.Call(method, () =>
            {
                g.LoadParameter(prmIndex);
                g.Load(dtoMemberName);
                loadValue();
            });
        }

        public static void WriteElement(MethodGenerator g, string dtoMemberName, Type elementType, Action loadValue)
        {
            var method = typeof(IDTOWriter).ResolveMethod("WriteElement",
                                                            new Type[] { elementType },
                                                            MethodParameter.Create<string>(), 
                                                            MethodParameter.Create<bool>(),
                                                            MethodParameter.CreateGeneric(elementType));
            var prmIndex = SerializationArgs.WriterIndex;
            g.Call(method, () =>
            {
                g.LoadParameter(prmIndex);
                g.Load(dtoMemberName);
                g.Load(IsPrimitive(elementType));
                loadValue();
            });
        }

        public static void WriteArray(MethodGenerator g, string dtoMemberName)
        {
            var method = typeof(IDTOWriter).ResolveMethod("WriteArray",
                                                            new Type[] { typeof(string) });
            var prmIndex = SerializationArgs.WriterIndex;
            g.Call(method, () =>
            {
                g.LoadParameter(prmIndex);
                g.Load(dtoMemberName);
            });
        }


        /// <summary>
        /// <para>得到读取某个类型的IL代码</para>
        /// <para>reader.ReadXXX(); 或 deserialzer.Deserialze();</para>
        /// </summary>
        /// <param name="g"></param>
        /// <param name="valueType"></param>
        /// <param name="loadValue"></param>
        public static void Read(MethodGenerator g,string dtoMemberName, Type valueType)
        {
            var method = SerializationMethodHelper.GetTypeMethod(valueType, SerializationMethodType.Deserialize);
            var prmIndex = SerializationMethodHelper.GetParameterIndex(method, SerializationMethodType.Deserialize);
            g.Call(method, () =>
            {
                g.LoadParameter(prmIndex);
                g.Load(dtoMemberName);
                //if (prmIndex == SerializationArgs.DeserializerIndex)
                //{
                //    //是deserializer.Deserializ();
                //    g.LoadVariable(SerializationArgs.TypeNameTable);
                //}
            });
        }

        public static void ReadBlob(MethodGenerator g, string dtoMemberName)
        {
            var method = typeof(IDTOReader).ResolveMethod("ReadBlob", typeof(string));
            var prmIndex = SerializationArgs.ReaderIndex;
            g.Call(method, () =>
            {
                g.LoadParameter(prmIndex);
                g.Load(dtoMemberName);
            });
        }

        /// <summary>
        /// 读取数组的长度
        /// </summary>
        /// <param name="g"></param>
        /// <param name="dtoMemberName"></param>
        /// <param name="valueType"></param>
        public static void ReadLength(MethodGenerator g, string dtoMemberName)
        {
            var method = typeof(IDTOReader).ResolveMethod("ReadLength", _readArgs);
            var prmIndex = SerializationArgs.ReaderIndex;
            g.Call(method, () =>
            {
                g.LoadParameter(prmIndex);
                g.Load(dtoMemberName);
            });
        }

        public static void ReadElement(MethodGenerator g, string dtoMemberName,Type elementType, IVariable index)
        {
            var method = typeof(IDTOReader).ResolveMethod("ReadElement", new Type[] { elementType }, MethodParameter.Create<string>(), MethodParameter.Create<int>());
            var prmIndex = SerializationArgs.ReaderIndex;
            g.Call(method, () =>
            {
                g.LoadParameter(prmIndex);
                g.Load(dtoMemberName);
                g.LoadVariable(index, LoadOptions.Default);
            });
        }

    }

    /// <summary>
    /// 方法类型
    /// </summary>
    internal enum SerializationMethodType
    {
        Serialize,
        Deserialize
    }

}
