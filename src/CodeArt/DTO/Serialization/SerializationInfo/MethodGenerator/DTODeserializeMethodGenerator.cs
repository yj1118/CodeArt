using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics;

using CodeArt.Runtime;
using CodeArt.Runtime.IL;

namespace CodeArt.DTO
{
    /// <summary>
    /// 序列化类型的动态方法生成器（自动序列化）
    /// </summary>
    internal static class DTODeserializeMethodGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        public static DeserializeMethod GenerateMethod(TypeSerializationInfo typeInfo)
        {
            DynamicMethod method = new DynamicMethod(string.Format("DTODeserialize_{0}", Guid.NewGuid().ToString("n"))
                                                    , null
                                                    , new Type[] { typeof(object), typeof(IDTOReader) }
                                                    , true);

            MethodGenerator g = new MethodGenerator(method);


            DeclareInstance(g, typeInfo);
            ReadMembers(g, typeInfo);

            //g.LoadVariable(SerializationArgs.InstanceName);

            //if (typeInfo.IsValueType) g.Box();//如果是结构体，需要装箱

            g.Return();

            return (DeserializeMethod)method.CreateDelegate(typeof(DeserializeMethod));
        }

        /// <summary>
        /// 声明强类型的instance变量留待后续代码使用，避免频繁类型转换
        /// </summary>
        private static void DeclareInstance(MethodGenerator g, TypeSerializationInfo typeInfo)
        {
            //TypeClassName instance = (TypeClassName)instance;
            var instance = g.Declare(typeInfo.ClassType, SerializationArgs.InstanceName);
            g.Assign(instance, () =>
            {
                g.LoadParameter(SerializationArgs.InstanceIndex);
                g.UnboxAny(typeInfo.ClassType);
            });
        }

        //private static void DeclareInstance(MethodGenerator g, TypeSerializationInfo typeInfo)
        //{
        //    if (typeInfo.ClassType.IsValueType)
        //    {
        //        var instance = g.Declare(typeInfo.ClassType, SerializationArgs.InstanceName);
        //        //g.LoadVariable(instance, LoadOptions.ValueAsAddress);
        //        //g.InitValue();
        //    }
        //    else
        //    {
        //        var instance = g.Declare(typeInfo.ClassType, SerializationArgs.InstanceName);
        //        g.Assign(instance, () =>
        //        {
        //            g.NewObject(typeInfo.ClassType);
        //        });
        //    }
        //}

        private static void ReadMembers(MethodGenerator g, TypeSerializationInfo typeInfo)
        {
            if (typeInfo.ClassAttribute.Mode == DTOSerializableMode.General)
            {
                foreach (var member in typeInfo.MemberInfos)
                {
                    //只有可以写入并且不是抽象的成员才能从dto中赋值
                    if (member.CanWrite && !member.IsAbstract)
                    {
                        g.BeginScope();
                        member.GenerateDeserializeIL(g);
                        g.EndScope();
                    }
                }
            }
            else
            {
                //在函数模式,只有标记了Parameter的成员才会被反序列化到对象实例中
                foreach (var member in typeInfo.MemberInfos)
                {
                    if (member.MemberAttribute.Type == DTOMemberType.Parameter && member.CanWrite && !member.IsAbstract)
                    {
                        g.BeginScope();
                        member.GenerateDeserializeIL(g);
                        g.EndScope();
                    }
                }
            }
        }
    }
}
