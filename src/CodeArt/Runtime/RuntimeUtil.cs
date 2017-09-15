using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Reflection;

using CodeArt.Util;
using CodeArt.Runtime.IL;

namespace CodeArt.Runtime
{
    public static class RuntimeUtil
    {
        //#region 线程挂起

        //private static Func<int, Stopwatch> _getStopwatch = LazyIndexer.Init<int, Stopwatch>((t) =>
        //{
        //    return new Stopwatch();
        //});

        ///// <summary>
        ///// 将当前线程挂起指定的毫秒数
        ///// 非常精确，但是比较消耗CPU
        ///// </summary>
        ///// <param name="millisecondsTimeout"></param>
        //public static void Sleep(int millisecondsTimeout)
        //{
        //    if (millisecondsTimeout < 1000)
        //    {
        //        var stopwatch = _getStopwatch(Thread.CurrentThread.ManagedThreadId);
        //        stopwatch.Restart();
        //        while (true)
        //        {
        //            if (stopwatch.ElapsedMilliseconds >= millisecondsTimeout)
        //                break;
        //        }
        //    }
        //    else if (millisecondsTimeout > 0)
        //    {
        //        SpinWait.SpinUntil(() => { return false; }, millisecondsTimeout);
        //    }
        //}

        //#endregion

        #region 泛型版本的设置属性和获取属性


        /// <summary>
        /// 使用该方法可以在无任何装箱和拆箱的情况下获取属性值，等同于直接调用 对象.属性 的写法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static T GetPropertyValue<T>(this object obj, string propertyName)
        {
            var objectType = obj.GetType();
            return GetPropertyValue<T>(objectType, obj, propertyName);
        }

        private static T GetPropertyValue<T>(Type objectType, object obj, string propertyName)
        {
            var method = _getPropertyValuesGeneric.GetValue(objectType, (item) => { return item.PropertyName.EqualsIgnoreCase(propertyName); });
            if (method == null)
            {
                lock (_getPropertyValuesGeneric)
                {
                    _getPropertyValuesGeneric.GetValue(objectType, (item) => { return item.PropertyName.EqualsIgnoreCase(propertyName); });
                    if (method == null)
                    {
                        method = GenerateGetPropertyValueMethod<T>(objectType, propertyName);
                        _getPropertyValuesGeneric.Add(objectType, method);
                    }
                }
            }

            var invoke = (Func<object, T>)method.Invoke;
            return invoke(obj);
        }


        private static MultiDictionary<Type, PropertyValueMethod> _getPropertyValuesGeneric = new MultiDictionary<Type, PropertyValueMethod>(false);


        private static PropertyValueMethod GenerateGetPropertyValueMethod<T>(Type objectType, string propertyName)
        {
            DynamicMethod method = new DynamicMethod(string.Format("GetPropertyValue_{0}", Guid.NewGuid().ToString("n"))
                                                    , typeof(T)
                                                    , new Type[] { typeof(object) }
                                                    , true);

            MethodGenerator g = new MethodGenerator(method);

            var result = g.Declare<T>("result");
            g.Assign(result, () =>
            {
                g.LoadParameter(0);
                g.Cast(objectType);
                g.LoadMember(propertyName);
            });

            g.LoadVariable("result");
            g.Return();

            var invoke = method.CreateDelegate(typeof(Func<object, T>));
            return new PropertyValueMethod(propertyName, invoke);
        }


        /// <summary>
        /// 使用该方法可以在无任何装箱和拆箱的情况下设置属性值，等同于直接调用 对象.属性=值 的写法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static void SetPropertyValue<T>(this object obj, string propertyName, T value)
        {
            var objectType = obj.GetType();
            var method = _setPropertyValuesGeneric.GetValue(objectType, (item) => { return item.PropertyName.EqualsIgnoreCase(propertyName); });
            if (method == null)
            {
                lock (_setPropertyValuesGeneric)
                {
                    _setPropertyValuesGeneric.GetValue(objectType, (item) => { return item.PropertyName.EqualsIgnoreCase(propertyName); });
                    if (method == null)
                    {
                        method = GenerateSetPropertyValueMethod<T>(objectType, propertyName);
                        _setPropertyValuesGeneric.Add(objectType, method);
                    }
                }
            }

            var invoke = (Action<object, T>)method.Invoke;
            invoke(obj, value);
        }


        private static MultiDictionary<Type, PropertyValueMethod> _setPropertyValuesGeneric = new MultiDictionary<Type, PropertyValueMethod>(false);


        private static PropertyValueMethod GenerateSetPropertyValueMethod<T>(Type objectType, string propertyName)
        {
            DynamicMethod method = new DynamicMethod(string.Format("SetPropertyValue_{0}", Guid.NewGuid().ToString("n"))
                                                    , null
                                                    , new Type[] { typeof(object), typeof(T) }
                                                    , true);

            MethodGenerator g = new MethodGenerator(method);

            g.LoadParameter(0);
            g.Cast(objectType);
            g.Assign(propertyName, () =>
            {
                g.LoadParameter(1);
            });
            g.Return();

            var invoke = method.CreateDelegate(typeof(Action<object, T>));
            return new PropertyValueMethod(propertyName, invoke);
        }

        #endregion

        #region 非泛型版


        /// <summary>
        /// 非泛型版的获取属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetPropertyValue(this object obj, string propertyName)
        {
            var objectType = obj.GetType();
            return GetPropertyValue(objectType, obj, propertyName);
        }


        /// <summary>
        /// 非泛型版的获取属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private static object GetPropertyValue(Type objectType, object obj, string propertyName)
        {
            var method = _getPropertyValues.GetValue(objectType, (item) => { return item.PropertyName.EqualsIgnoreCase(propertyName); });
            if (method == null)
            {
                lock (_getPropertyValues)
                {
                    _getPropertyValues.GetValue(objectType, (item) => { return item.PropertyName.EqualsIgnoreCase(propertyName); });
                    if (method == null)
                    {
                        method = GenerateGetPropertyValueMethod(objectType, propertyName);
                        _getPropertyValues.Add(objectType, method);
                    }
                }
            }

            var invoke = (Func<object, object>)method.Invoke;
            return invoke(obj);
        }



        private static MultiDictionary<Type, PropertyValueMethod> _getPropertyValues = new MultiDictionary<Type, PropertyValueMethod>(false);


        private static PropertyValueMethod GenerateGetPropertyValueMethod(Type objectType, string propertyName)
        {
            DynamicMethod method = new DynamicMethod(string.Format("GetPropertyValue_{0}", Guid.NewGuid().ToString("n"))
                                                    , typeof(object)
                                                    , new Type[] { typeof(object) }
                                                    , true);

            MethodGenerator g = new MethodGenerator(method);

            var result = g.Declare<object>("result");
            g.Assign(result, () =>
            {
                g.LoadParameter(0);
                g.Cast(objectType);
                g.LoadMember(propertyName);
                g.Cast(typeof(object));
            });

            g.LoadVariable("result");
            g.Return();

            var invoke = method.CreateDelegate(typeof(Func<object, object>));
            return new PropertyValueMethod(propertyName, invoke);
        }


        /// <summary>
        /// 非泛型版
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static void SetPropertyValue(this object obj, string propertyName, object value)
        {
            var objectType = obj.GetType();
            var method = _setPropertyValues.GetValue(objectType, (item) => { return item.PropertyName.EqualsIgnoreCase(propertyName); });
            if (method == null)
            {
                lock (_setPropertyValues)
                {
                    _setPropertyValues.GetValue(objectType, (item) => { return item.PropertyName.EqualsIgnoreCase(propertyName); });
                    if (method == null)
                    {
                        method = GenerateSetPropertyValueMethod(objectType, propertyName);
                        _setPropertyValues.Add(objectType, method);
                    }
                }
            }

            var invoke = (Action<object, object>)method.Invoke;
            invoke(obj, value);
        }


        private static MultiDictionary<Type, PropertyValueMethod> _setPropertyValues = new MultiDictionary<Type, PropertyValueMethod>(false);


        private static PropertyValueMethod GenerateSetPropertyValueMethod(Type objectType, string propertyName)
        {
            var propertyInfo = objectType.ResolveProperty(propertyName);
            var propertyType = propertyInfo.PropertyType;

            DynamicMethod method = new DynamicMethod(string.Format("SetPropertyValue_{0}", Guid.NewGuid().ToString("n"))
                                                    , null
                                                    , new Type[] { typeof(object), typeof(object) } //obj  value
                                                    , true);

            MethodGenerator g = new MethodGenerator(method);

            g.LoadParameter(0);
            g.Cast(objectType);
            g.Assign(propertyName, () =>
            {
                g.LoadParameter(1);
                g.Cast(propertyType);
            });
            g.Return();

            var invoke = method.CreateDelegate(typeof(Action<object, object>));
            return new PropertyValueMethod(propertyName, invoke);
        }


        #endregion

        private class PropertyValueMethod
        {
            public string PropertyName
            {
                get;
                private set;
            }

            public object Invoke
            {
                get;
                private set;
            }

            public PropertyValueMethod(string propertyName, object invoke)
            {
                this.PropertyName = propertyName;
                this.Invoke = invoke;
            }

        }


        /// <summary>
        /// 属性是的设置方法是否为公开的
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static bool IsPublicSet(this PropertyInfo propertyInfo)
        {
            if (propertyInfo.SetMethod == null) return false;
            return propertyInfo.SetMethod.IsPublic;
        }

    }
}
