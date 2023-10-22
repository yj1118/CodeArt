using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Reflection.Emit;

using CodeArt.Util;
using CodeArt.Runtime.IL;

namespace CodeArt.Runtime
{
    public static class TypeExtensions
    {
        #region 构造函数

        private static Func<ConstructorKey, ConstructorInfo> _getConstructor = LazyIndexer.Init<ConstructorKey, ConstructorInfo>(GetConstructor);

        private static ConstructorInfo GetConstructor(ConstructorKey key)
        {
            ConstructorInfo result = null;

            //遍历整个继承树，根据参数数量和类型找到方法
            const BindingFlags oneLevelFlags =
              BindingFlags.DeclaredOnly |
              BindingFlags.Instance |
              BindingFlags.Public |
              BindingFlags.NonPublic |
                BindingFlags.IgnoreCase;

            Type currentType = key.ObjectType;
            do
            {
                ConstructorInfo[] infos = currentType.GetConstructors(oneLevelFlags);
                foreach (ConstructorInfo info in infos)
                {
                    if (ValidateGenericArguments(info, key.GenericTypes)
                            && ValidateParameters(info, key.Parameters))
                    {
                        result = info;
                        break;
                    }
                }
                if (result != null) break;
                currentType = currentType.BaseType;
            } while (currentType != null);

            return result;
        }

        private static ConstructorInfo GetConstructor(Type type, Type[] genericTypes, MethodParameter[] prms, bool isGenericVersion)
        {
            genericTypes = genericTypes ?? Type.EmptyTypes;
            prms = prms ?? MethodParameter.EmptyParameters;

            ConstructorKey key = new ConstructorKey(type, genericTypes, prms, isGenericVersion);
            return _getConstructor(key);
        }

        private static ConstructorInfo GetConstructor(Type type, Type[] genericTypes, MethodParameter[] prms)
        {
            return GetConstructor(type, genericTypes, prms, false);
        }

        public static ConstructorInfo ResolveConstructor(this Type type, params Type[] parameterTypes)
        {
            if (parameterTypes == null || parameterTypes.Length == 0) return GetConstructor(type, null, null);
            return GetConstructor(type, null, parameterTypes.Select(t => { return new MethodParameter(t, false, false); }).ToArray());
        }

        public static ConstructorInfo ResolveConstructor(this Type type, MethodParameter[] prms)
        {
            return GetConstructor(type, null, prms);
        }

        public static ConstructorInfo ResolveConstructor(this Type type, Type[] genericTypes, params MethodParameter[] parameters)
        {
            return GetConstructor(type, genericTypes, parameters);
        }

        public static ConstructorInfo ResolveConstructorByGeneric(this Type type, Type[] genericTypes)
        {
            return GetConstructor(type, genericTypes, null);
        }


        #endregion

        #region 从当前类型中得到方法

        private static Func<MethodKey, MethodInfo> _getMethod = LazyIndexer.Init<MethodKey, MethodInfo>(GetMethod);

        private static MethodInfo GetMethod(MethodKey key)
        {
            MethodInfo result = null;

            //遍历整个继承树，根据参数数量和类型找到方法
            const BindingFlags oneLevelFlags =
              BindingFlags.DeclaredOnly |
              BindingFlags.Instance |
              BindingFlags.Public |
              BindingFlags.NonPublic |
                BindingFlags.IgnoreCase | BindingFlags.Static;

            Type currentType = key.ObjectType;
            do
            {
                MethodInfo[] infos = currentType.GetMethods(oneLevelFlags);
                foreach (MethodInfo info in infos)
                {
                    if (info.Name.Equals(key.MethodName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (key.IsOnlyMathName ||
                            (ValidateGenericArguments(info, key.GenericTypes)
                                && ValidateParameters(info, key.Parameters)))
                        {
                            result = info;
                            break;
                        }
                    }
                }
                if (result != null) break;
                currentType = currentType.BaseType;
            } while (currentType != null);

            if (result == null) return result;

            if (key.IsGenericVersion) return result;
            return key.GenericTypes.Length == 0 ? result : result.MakeGenericMethod(key.GenericTypes);
        }


        private static MethodInfo GetMethod(Type type, string methodName, Type[] genericTypes, MethodParameter[] prms, bool isGenericVersion)
        {
            ArgumentAssert.IsNotNull(type, "type");
            ArgumentAssert.IsNotNull(methodName, "methodName");

            genericTypes = genericTypes ?? Type.EmptyTypes;
            prms = prms ?? MethodParameter.EmptyParameters;

            MethodKey key = new MethodKey(type, methodName, genericTypes, prms, isGenericVersion);
            return _getMethod(key);
        }

        private static MethodInfo GetMethod(Type type, string methodName, Type[] genericTypes, MethodParameter[] prms)
        {
            return GetMethod(type, methodName, genericTypes, prms, false);
        }

        public static MethodInfo ResolveMethod(this Type type, string methodName)
        {
            return GetMethod(type, methodName, null, null);
        }

        public static MethodInfo ResolveMethod(this Type type, string methodName, params Type[] parameterTypes)
        {
            if (parameterTypes == null || parameterTypes.Length == 0) return GetMethod(type, methodName, null, null);
            return GetMethod(type, methodName, null, parameterTypes.Select(t => { return new MethodParameter(t, false, false); }).ToArray());
        }

        public static MethodInfo ResolveMethod(this Type type, string methodName, MethodParameter[] prms)
        {
            return GetMethod(type, methodName, null, prms);
        }

        public static MethodInfo ResolveMethod(this Type type, string methodName, Type[] genericTypes, params MethodParameter[] parameters)
        {
            return GetMethod(type, methodName, genericTypes, parameters);
        }

        /// <summary>
        /// 获取通用的泛型版本方法
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="genericArgsCount"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static MethodInfo ResolveMethod(this Type type, string methodName, int genericArgsCount, params MethodParameter[] parameters)
        {
            Type[] genericTypes = new Type[genericArgsCount];
            for (var i = 0; i < genericArgsCount; i++) genericTypes[i] = typeof(int);
            return GetMethod(type, methodName, genericTypes, parameters, true);
        }

        #endregion

        #region 方法检查

        private static bool ValidateParameters(MethodBase method, MethodParameter[] prms)
        {
            ParameterInfo[] prmInfos = method.GetParameters();
            if (prmInfos.Length == prms.Length)
            {
                for (var i = 0; i < prms.Length; i++)
                {
                    MethodParameter prm = prms[i];
                    Type prmInfoType = prmInfos[i].ParameterType;
                    if (prm.Type.IsByRef && !prmInfoType.IsByRef) return false;
                    if (prm.IsGeneric) continue;//泛型不用判断类型是否相同
                    if (prm.Type != prmInfoType) return false;
                }
                return true;
            }
            return false;
        }

        private static bool ValidateGenericArguments(MethodBase method, Type[] genericTypes)
        {
            if (genericTypes.Length == 0) return !method.IsGenericMethodDefinition;
            if (method.IsGenericMethodDefinition)
            {
                //如果指定了泛型参数，并且方法是泛型方法，则进一步验证
                return method.GetGenericArguments().Length == genericTypes.Length;
            }
            return false;
        }



        #endregion

        #region 集合相关

        private static Func<Type, Type> _elementTypes = LazyIndexer.Init<Type, Type>(type => { return GetElementType(type); });

        public static Type ResolveElementType(this Type type)
        {
            return _elementTypes(type);
        }

        private static Type GetElementType(Type type)
        {
            Type _listType = null;
            Type _dictionaryType = null;
            if (FindCollectionInterface(
                          type,
                          ref _listType,
                          ref _dictionaryType))
            {
                Type itf = _dictionaryType ?? _listType;
                Type[] genArgs = itf.GetGenericArguments();

                if(genArgs.Length > 0)
                {
                    if (_dictionaryType != null) //IDictionary{Key,Value}
                        return typeof(KeyValuePair<,>).MakeGenericType(genArgs);
                    else //ICollection{T}
                        return genArgs[0];
                }
            }
            return type.GetElementType();
        }

        public static bool FindCollectionInterface(this Type targetType,
                                                    ref Type listType,
                                                    ref Type dictionaryType)
        {
            if (targetType == typeof(string)) return false;

            if (_FindCollectionInterface(targetType, ref listType, ref dictionaryType))
            {
                return true;
            }

            foreach (Type interfaceType in targetType.GetInterfaces())
            {
                if (_FindCollectionInterface(interfaceType, ref listType, ref dictionaryType))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool _FindCollectionInterface(Type targetType,
                                                    ref Type listType,
                                                    ref Type dictionaryType)
        {
            if (targetType.IsGenericType)
            {
                Type genericType = targetType.GetGenericTypeDefinition();

                if (genericType == typeof(IDictionary<,>))
                {
                    dictionaryType = targetType;
                }
                else if (genericType == typeof(IEnumerable<>))
                {
                    listType = targetType;
                }
            }
            else
            {
                if (targetType == typeof(IDictionary))
                {
                    dictionaryType = targetType;
                }
                else if (targetType == typeof(IEnumerable))
                {
                    listType = targetType;
                }
            }

            return listType != null || dictionaryType != null;
        }

        /// <summary>
        /// 是否为集合类，例如 array、collection、dictionary等
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsCollection(this Type type)
        {
            return _isCollection(type);
        }

        private static Func<Type, bool> _isCollection = LazyIndexer.Init<Type, bool>((type) =>
        {
            if (type.IsArray) return true;
            Type listType = null;
            Type dictionaryType = null;
            return FindCollectionInterface(
                          type,
                          ref listType,
                          ref dictionaryType);
        });


        public static bool IsList(this Type type)
        {
            return _isList(type);
        }

        private static Func<Type, bool> _isList = LazyIndexer.Init<Type, bool>((type)=>
        {
            Type listType = null;
            Type dictionaryType = null;
            if (FindCollectionInterface(
                          type,
                          ref listType,
                          ref dictionaryType))
            {
                return listType != null && dictionaryType == null;
            }
            return false;
        });


        public static bool IsDictionary(this Type type)
        {
            return _isDictionary(type);
        }

        private static Func<Type, bool> _isDictionary = LazyIndexer.Init<Type, bool>((type) =>
        {
            Type listType = null;
            Type dictionaryType = null;
            if (FindCollectionInterface(
                          type,
                          ref listType,
                          ref dictionaryType))
            {
                return dictionaryType != null;
            }
            return false;
        });


        /// <summary>
        /// <para>得到对象的泛型参数</para>
        /// <para>该方法与type.GetGenericArguments()的不同之处在于</para>
        /// <para>它会查找整个循环链</para>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type[] ResolveGenericArguments(this Type type)
        {
            Type[] result = null;
            while (type != null)
            {
                result = type.GetGenericArguments();
                if (result.Length > 0) return result;
                type = type.BaseType;
            }
            return result == null ? Type.EmptyTypes : result;
        }



        #endregion

        #region 得到按地址传递时的类型

        private static readonly Func<Type, Type> _byRefTypes = LazyIndexer.Init<Type, Type>(type => type.MakeByRefType());
        /// <summary>
        /// <para>返回表示作为 ref 参数（在 Visual Basic 中为 ByRef 参数）传递时的当前类型的 System.Type 对象。</para>
        /// <para>例如：int&、long& </para>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type ResolveByRef(this Type type)
        {
            return _byRefTypes(type);
        }

        #endregion

        #region 得到属性的定义

        //private sealed class PropertyKey
        //{
        //    public Type OwnerType { get; set; }
        //    public string PropertyName { get; set; }

        //    public override bool Equals(object obj)
        //    {
        //        PropertyKey key = obj as PropertyKey;
        //        return key.OwnerType == this.OwnerType && key.PropertyName == this.PropertyName;
        //    }

        //    public override int GetHashCode()
        //    {
        //        return this.OwnerType.GetHashCode() ^ this.PropertyName.GetHashCode();
        //    }

        //}



        private static Func<Type, Func<string, PropertyInfo>> _getPropertyInfo = LazyIndexer.Init<Type, Func<string, PropertyInfo>>((ownerType) =>
        {
            return LazyIndexer.Init<string, PropertyInfo>((propertyName) =>
            {
                PropertyInfo property = null;
                Type baseType = ownerType;
                do
                {
                    if (baseType.IsInterface)
                    {
                        property = baseType.GetProperty(propertyName, _flags);
                        if (property == null)
                        {
                            Type[] interfaceTypes = baseType.GetInterfaces();
                            foreach (var t in interfaceTypes)
                            {
                                property = t.ResolveProperty(propertyName);
                                if (property != null) break;
                            }
                        }
                        if (property != null) break;
                    }
                    else
                    {
                        property = baseType.GetProperty(propertyName, _flags);
                        if (property != null) break;
                        baseType = baseType.BaseType;
                    }
                }
                while (baseType != null);

                return property;
            });
        });

        private const BindingFlags _flags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        private static Func<Type, Func<string, FieldInfo>> _getFieldInfo = LazyIndexer.Init<Type, Func<string, FieldInfo>>((ownerType) =>
        {
            return LazyIndexer.Init<string, FieldInfo>((fieldName) =>
            {
                FieldInfo field = null;
                Type baseType = ownerType;
                do
                {
                    if (baseType.IsInterface)
                    {
                        field = baseType.GetField(fieldName, _flags);
                        if (field == null)
                        {
                            Type[] interfaceTypes = baseType.GetInterfaces();
                            foreach (var t in interfaceTypes)
                            {
                                field = t.ResolveField(fieldName);
                                if (field != null) break;
                            }
                        }
                        if (field != null) break;
                    }
                    else
                    {
                        field = baseType.GetField(fieldName, _flags);
                        if (field != null) break;
                        baseType = baseType.BaseType;
                    }
                }
                while (baseType != null);

                return field;
            });
        });

        //private static Func<PropertyKey, PropertyInfo> _getPropertyInfo = LazyIndexer.Init<PropertyKey, PropertyInfo>(key =>
        //{
        //    var type = key.OwnerType;
        //    var propertyName = key.PropertyName;
        //    if (type.IsInterface)
        //    {
        //        PropertyInfo result = type.GetProperty(propertyName);
        //        if (result == null)
        //        {
        //            Type[] interfaceTypes = type.GetInterfaces();
        //            foreach (var t in interfaceTypes)
        //            {
        //                result = t.ResolveProperty(propertyName);
        //                if (result != null) break;
        //            }
        //        }
        //        return result;
        //    }
        //    return type.GetProperty(propertyName);
        //});


        public static PropertyInfo ResolveProperty(this Type type, string propertyName)
        {
            return _getPropertyInfo(type)(propertyName);
        }

        public static FieldInfo ResolveField(this Type type, string fieldName)
        {
            return _getFieldInfo(type)(fieldName);
        }

        #endregion

        public static MethodInfo ResolveMethod(this Type type, Type baseType, string methodName, params Type[] parameterTypes)
        {
            var baseMethod = baseType.ResolveMethod(methodName, parameterTypes);
            if (baseMethod == null) return null;
            return GetBestCallableOverride(type, baseMethod);
        }

        public static Type ResolveGenericType(this Type type, params Type[] typeArguments)
        {
            return type.MakeGenericType(typeArguments);
        }

        internal static MethodInfo GetBestCallableOverride(this Type type, MethodInfo method)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (method == null) throw new ArgumentNullException("method");

            if (type.IsInterface || type.IsAbstract)
            {
                if (!method.DeclaringType.IsAssignableFrom(type))
                {
                    throw new ArgumentException("类型没有实现方法", "type");
                }
                return method;
            }

            if (method.DeclaringType.IsInterface)
            {
                var map = type.GetInterfaceMap(method.DeclaringType);

                for (int i = 0; i < map.InterfaceMethods.Length; ++i)
                {
                    if (map.InterfaceMethods[i] == method)
                    {
                        // implemented explicity (not callable)
                        if (map.TargetMethods[i].IsPrivate) break;
                        return map.TargetMethods[i];
                    }
                }

                if (method.DeclaringType.IsAssignableFrom(type))
                {
                    return method;
                }
                throw new MissingMethodException(type.Name, method.Name);
            }

            while (type != null)
            {
                if (type == method.DeclaringType)
                {
                    if (method.IsAbstract)
                    {
                        throw new MissingMethodException(type.Name, method.Name);
                    }
                    return method;
                }
                type = type.BaseType;
            }
            throw new MissingMethodException(type.Name, method.Name);
        }



        private static Func<Type, string> _resolveName = LazyIndexer.Init<Type, string>((type) =>
        {
            if (type.IsGenericType)
            {
                StringBuilder temp = new StringBuilder();
                string name = type.Name;
                int pos = name.IndexOf("`");
                temp.AppendFormat("{0}<", name.Substring(0, pos));
                Type[] args = type.GetGenericArguments();
                foreach (Type a in args)
                {
                    temp.AppendFormat("{0},", a.ResolveName());
                }
                temp.Length--;
                temp.Append(">");
                return temp.ToString();
            }
            else
            {
                string name = type.FullName ?? string.Empty;
                int pos = name.IndexOf("+");
                if (pos > -1)
                    return string.Format("{0}.{1}", name.Substring(0, pos), name.Substring(pos + 1));
                else
                    return name;
            }
        });

        /// <summary>
        /// 获取类型名称，会自动解析泛型名称
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ResolveName(this Type type)
        {
            return _resolveName(type);
        }

        public static IEnumerable<MemberInfo> GetPropertyAndFields(this Type type, BindingFlags flag)
        {
            return type.FindMembers(MemberTypes.Field | MemberTypes.Property, flag, null, null).Where((member)=>
            {
                return !member.IsBackingField();
            });
        }

        public static IEnumerable<MemberInfo> GetPropertyAndFields(this Type type)
        {
            const BindingFlags flag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.NonPublic;
            return type.GetPropertyAndFields(flag);
        }

        /// <summary>
        /// 检查类型是否实现了某个接口
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        public static bool ImplementInterface(this Type type, Type interfaceType)
        {
            return type.GetInterface(interfaceType.FullName) != null;
        }

        /// <summary>
        /// 类型是否实现了<paramref name="target"/>类型的要求（继承或者实现了接口），或等于<paramref name="target"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsImplementOrEquals(this Type type, Type target)
        {
            if (type.Equals(target)) return true;
            else
            {
                if (target.IsInterface)
                    return type.GetInterface(target.FullName, true) != null;
                if (type.IsSubclassOf(target)) return true;
                if (target.IsGenericType && target.ContainsGenericParameters)
                {
                    if(type.IsGenericType)
                    {
                        var genericTypeDefinition = type.GetGenericTypeDefinition();
                        if (genericTypeDefinition != type) //有的泛型例如 List<T>的定义就等于其自身
                            return type.GetGenericTypeDefinition().IsImplementOrEquals(target);
                    }
                    else
                    {
                        if (type.BaseType != null)
                            return type.BaseType.IsImplementOrEquals(target);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 类型是否为自定义结构体
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsStruct(this Type type)
        {
            return type.IsValueType && !type.IsPrimitive && !type.IsEnum && type != typeof(Guid) && type != typeof(DateTime);
        }

        public static object GetStaticValue(this Type type, string name)
        {
            object value = null;
            if(TryGetStaticValue(type, name,out value))
            {
                return value;
            }
            return null;
        }

        /// <summary>
        /// 获得静态字段或属性的值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetStaticValue(this Type type, string name, out object value)
        {
            if(type.IsEnum)
            {
                value = Enum.Parse(type, name);
                return true;
            }

            FieldInfo field = type.ResolveField(name);
            if (field != null && field.IsStatic)
            {
                try
                {
                    value = field.GetValue(null);
                }
                catch(Exception ex)
                {
                    value = null;
                }
                finally
                {
                    
                }
                return true;
            }

            PropertyInfo property = type.ResolveProperty(name);
            if (property != null)
            {
                value = property.GetValue(null, null);
                return true;
            }

            value = null;
            return false;
        }


        public static Type GetFieldOrPropertyType(this MemberInfo member)
        {
            var type = member.MemberType;
            if(type == MemberTypes.Field)
            {
                var field = member as FieldInfo;
                return field.FieldType;
            }

            if (type == MemberTypes.Property)
            {
                var property = member as PropertyInfo;
                return property.PropertyType;
            }
            return null;
        }

        /// <summary>
        /// 是否为自动属性产生的BackingField字段，通常这类字段我们不需要处理
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static bool IsBackingField(this MemberInfo member)
        {
            return member.Name.EndsWith("k__BackingField");
        }


        #region 继承

        /// <summary>
        /// <para>类型的继承深度，直接从object继承而来的对象深度为1，每继承一个类，深度加1</para>
        /// <para>object的深度值为0</para>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int GetDepth(this Type type)
        {
            return _getInheriteds(type).Count();
        }

        /// <summary>
        /// 获得继承链
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetInheriteds(this Type type)
        {
            return _getInheriteds(type);
        }

        private static Func<Type, IEnumerable<Type>> _getInheriteds = LazyIndexer.Init<Type, IEnumerable<Type>>((objectType) =>
        {
            Stack<Type> inheriteds = new Stack<Type>();

            var type = objectType;
            while (type.BaseType != null)
            {
                inheriteds.Push(type.BaseType);
                type = type.BaseType;
            }
            return inheriteds;
        });

        #endregion

        #region 创建实例非泛型版

        /// <summary>
        /// 创建实例，IL的实现，高效率
        /// </summary>
        /// <param name="instanceType"></param>
        /// <returns></returns>
        public static object CreateInstance(this Type instanceType)
        {
            CreateInstanceMethod method = null;
            if (!_getCreateInstanceMethods.TryGetValue(instanceType, out method))
            {
                if (method == null)
                {
                    lock (_getCreateInstanceMethods)
                    {
                        if (!_getCreateInstanceMethods.TryGetValue(instanceType, out method))
                        {
                            if (method == null)
                            {
                                method = GenerateCreateInstanceMethod(instanceType);
                                _getCreateInstanceMethods.Add(instanceType, method);
                            }
                        }
                    }
                }
            }

            var invoke = (Func<object>)method.Invoke;
            return invoke();
        }


        private static Dictionary<Type, CreateInstanceMethod> _getCreateInstanceMethods = new Dictionary<Type, CreateInstanceMethod>();


        private static CreateInstanceMethod GenerateCreateInstanceMethod(Type objectType)
        {
            DynamicMethod method = new DynamicMethod(string.Format("CreateInstance_{0}", Guid.NewGuid().ToString("n"))
                                                    , typeof(object)
                                                    , Array.Empty<Type>()
                                                    , true);

            MethodGenerator g = new MethodGenerator(method);

            var result = g.Declare<object>("result");
            g.Assign(result, () =>
            {
                g.NewObject(objectType);
                g.Cast(typeof(object));
            });

            g.LoadVariable("result");
            g.Return();

            var invoke = method.CreateDelegate(typeof(Func<object>));
            return new CreateInstanceMethod(invoke);
        }

        private class CreateInstanceMethod
        {
            public object Invoke
            {
                get;
                private set;
            }

            public CreateInstanceMethod(object invoke)
            {
                this.Invoke = invoke;
            }
        }


        /// <summary>
        /// 创建实例，IL的实现，高效率
        /// </summary>
        /// <param name="instanceType"></param>
        /// <returns></returns>
        public static object CreateInstance(this ConstructorInfo constructor, object[] args)
        {
            CreateInstanceMethod method = null;
            if (!_getCreateInstanceByConstructorMethods.TryGetValue(constructor, out method))
            {
                if (method == null)
                {
                    lock (_getCreateInstanceByConstructorMethods)
                    {
                        if (!_getCreateInstanceByConstructorMethods.TryGetValue(constructor, out method))
                        {
                            if (method == null)
                            {
                                method = GenerateCreateInstanceMethod(constructor);
                                _getCreateInstanceByConstructorMethods.Add(constructor, method);
                            }
                        }
                    }
                }
            }

            var invoke = (Func<object[], object>)method.Invoke;
            return invoke(args);
        }


        private static Dictionary<ConstructorInfo, CreateInstanceMethod> _getCreateInstanceByConstructorMethods = new Dictionary<ConstructorInfo, CreateInstanceMethod>();

        private static CreateInstanceMethod GenerateCreateInstanceMethod(ConstructorInfo constructor)
        {
            var objectType = constructor.DeclaringType;
            DynamicMethod method = new DynamicMethod(string.Format("CreateInstanceByConstructor_{0}", Guid.NewGuid().ToString("n"))
                                                    , typeof(object)
                                                    , new Type[] { typeof(object[]) }
                                                    , true);

            MethodGenerator g = new MethodGenerator(method);
            //以下代码把数组参数转成，new T(arg0,arg1)的形式
            var result = g.Declare(objectType,"result");
            var objs = g.Declare<object[]>();
            g.Assign(objs, () =>
            {
                g.LoadParameter(0);
            });

            g.Assign(result, () =>
            {
                g.NewObject(constructor, () =>
                 {
                     var index = g.Declare<int>();
                     var prms = constructor.GetParameters();
                     for (var i = 0; i < prms.Length; i++)
                     {
                         g.Assign(index, () =>
                         {
                             g.Load(i);
                         });

                         g.LoadElement(objs, index);
                         g.Cast(prms[i].ParameterType);
                     }
                 });
            });

            g.LoadVariable("result");
            g.Cast(typeof(object));
            g.Return();

            var invoke = method.CreateDelegate(typeof(Func<object[], object>));
            return new CreateInstanceMethod(invoke);
        }

        #endregion

        #region 调用静态方法

        /// <summary>
        /// 执行静态方法
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object InvokeMethod(this Type objectType, string methodName, object[] args)
        {
            //经过测试发现以下代码和IL动态生成的代码效率一样，因此就用这个了
            var method = objectType.ResolveMethod(methodName);
            return method.Invoke(null, args);

            //if (args == null) args = Array.Empty<object>();
            //StaticMethod method = _getGenerateCallStaticMethods.GetValue(objectType, (item) => { return item.Name.EqualsIgnoreCase(methodName); });
            //if (method == null)
            //{
            //    lock (_getGenerateCallStaticMethods)
            //    {
            //        _getGenerateCallStaticMethods.GetValue(objectType, (item) => { return item.Name.EqualsIgnoreCase(methodName); });
            //        if (method == null)
            //        {
            //            method = GenerateCallStaticMethod(objectType, methodName);
            //            _getGenerateCallStaticMethods.Add(objectType, method);
            //        }
            //    }
            //}
            //return method.Invoke(args);
        }

        //private static MultiDictionary<Type, StaticMethod> _getGenerateCallStaticMethods = new MultiDictionary<Type, StaticMethod>(false);


        //private static StaticMethod GenerateCallStaticMethod(Type objectType, string methodName)
        //{
        //    var methodInfo = objectType.ResolveMethod(methodName);
        //    DynamicMethod method = new DynamicMethod(string.Format("CallStaticMethod_{0}", Guid.NewGuid().ToString("n"))
        //                                            , typeof(object)
        //                                            , new Type[] { typeof(object[]) }
        //                                            , true);

        //    MethodGenerator g = new MethodGenerator(method);
        //    var objs = g.Declare<object[]>();
        //    g.Assign(objs, () =>
        //    {
        //        g.LoadParameter(0);
        //    });

        //    if (methodInfo.ReturnType == typeof(void))
        //    {
        //        g.Call(methodInfo, () =>
        //        {
        //            var index = g.Declare<int>();
        //            var prms = methodInfo.GetParameters();
        //            for (var i = 0; i < prms.Length; i++)
        //            {
        //                g.Assign(index, () =>
        //                {
        //                    g.Load(i);
        //                });

        //                g.LoadElement(objs, index);
        //                g.Cast(prms[i].ParameterType);
        //            }
        //        });
        //        g.LoadNull();
        //    }
        //    else
        //    {
        //        var result = g.Declare(typeof(object), "result");
        //        g.Assign(result, () =>
        //        {
        //            g.Call(methodInfo, () =>
        //            {
        //                var index = g.Declare<int>();
        //                var prms = methodInfo.GetParameters();
        //                for (var i = 0; i < prms.Length; i++)
        //                {
        //                    g.Assign(index, () =>
        //                    {
        //                        g.Load(i);
        //                    });

        //                    g.LoadElement(objs, index);
        //                    g.Cast(prms[i].ParameterType);
        //                }
        //            });
        //        });
        //        g.LoadVariable("result");
        //    }



        //    g.Return();

        //    var invoke = method.CreateDelegate(typeof(Func<object[], object>));
        //    return new StaticMethod(methodName, (Func<object[], object>)invoke);
        //}

        //private class StaticMethod
        //{
        //    public string Name
        //    {
        //        get;
        //        private set;
        //    }

        //    public Func<object[], object> Invoke
        //    {
        //        get;
        //        private set;
        //    }

        //    public StaticMethod(string name, Func<object[], object> invoke)
        //    {
        //        this.Name = name;
        //        this.Invoke = invoke;
        //    }
        //}

        #endregion



    }
}
