using System;
using System.Collections.Generic;
using System.Runtime;
using System.IO;

using CodeArt.Runtime;
using System.Collections;

namespace CodeArt.Util
{
    public static class DataUtil
    {
        public static T ToValue<T>(object value)
        {
            var valueType = typeof(T);
            return (T)ToValue(value, valueType);
        }

        public static object ToValue(object value, Type valueType)
        {
            if (value == null) return GetDefaultValue(valueType);
            if(valueType == typeof(Guid))
            {
                if (value is Guid) return (Guid)value;
                return Guid.Parse(value.ToString());
            }
            if(valueType == typeof(string))
            {
                if (value is Guid) return value.ToString();
            }
            return Convert.ChangeType(value, valueType);
        }


        /// <summary>
        /// 将字符串所包含的内容，转换成<paramref name="valueType"/>对应的类型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public static object ToValue(string value, Type valueType)
        {
            if (string.IsNullOrEmpty(value)) return GetDefaultValue(valueType);
            return Convert.ChangeType(value, valueType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="typeName">
        /// char,bool,byte,datetime,double,short,int,long,sbyte,float,string,ushort,uint,ulong
        /// </param>
        /// <returns></returns>
        public static object ToValue(string value, string typeName)
        {
            var type = GetPrimitiveType(typeName);
            return ToValue(value, type);
        }

        /// <summary>
        /// 是否为基元类型的数据（PrimitiveTypes指示的数据）
        /// </summary>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public static bool IsPrimitiveType(Type valueType)
        {
            TypeCode code = Type.GetTypeCode(valueType);
            switch (code)
            {
                case TypeCode.Char:
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.DateTime:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.String:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Object:
                    {
                        if (valueType == typeof(Guid))
                        {
                            return true;
                        }
                    }
                    break;
                default:
                    return false;
            }
            return false;
        }

        public static bool IsDefaultValue(object value)
        {
            var type = value.GetType();
            var defaultValue = _getDefaultValue(type);
            if (defaultValue == null) return value == null;
            return defaultValue.Equals(value);  //不能直接用==，因为值类型包装后的object对象的地址不同
        }

        public static bool IsDefaultValue<T>(T value)
        {
            var type = typeof(T);
            var defaultValue = _getDefaultValue(type);
            if (defaultValue == null) return value == null;
            return defaultValue.Equals(value);
        }

        private static Func<Type, object> _getDefaultValue = LazyIndexer.Init<Type, object>((valueType) =>
        {
            return valueType.IsValueType ? Activator.CreateInstance(valueType) : null;
        });


        public static object GetDefaultValue(Type valueType)
        {
            return _getDefaultValue(valueType);
        }


        public static Type GetPrimitiveType(string typeName)
        {
            switch (typeName.ToLower())
            {
                case "char": return typeof(char);
                case "bool":
                case "boolean":
                    return typeof(bool);
                case "byte":
                    return typeof(byte);
                case "datetime":
                    return typeof(DateTime);
                case "decimal":
                    return typeof(decimal);
                case "double":
                    return typeof(double);
                case "short":
                    return typeof(short);
                case "int":
                    return typeof(int);
                case "single":
                case "long":
                    return typeof(long);
                case "sbyte":
                    return typeof(sbyte);
                case "float":
                    return typeof(float);
                case "string":
                    return typeof(string);
                case "ushort":
                    return typeof(ushort);
                case "uint":
                    return typeof(uint);
                case "ulong":
                    return typeof(ulong);
                case "guid":
                    return typeof(Guid);
                default:
                    return null;
            }
        }

        public static bool IsEmpty(this IEnumerable items)
        {
            if (items == null) return true;
            foreach (var item in items)
            {
                return false;
            }
            return true;
        }

        public static string PercentageText(this double value)
        {
            var result = string.Format("{0:F}", ((double)value) * 100);
            if (result.EndsWith(".00")) result = result.Substring(0, result.Length - 3);
            else if (result.EndsWith(".0")) result = result.Substring(0, result.Length - 2);
            return string.Format("{0}%", result);
        }


        public const string PrimitiveTypes = "char,bool,byte,datetime,double,short,int,long,sbyte,single,float,string,ushort,uint,ulong,guid";
    }
}
