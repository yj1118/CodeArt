using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeArt.Util;
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    /// <summary>
    /// 类型转换器，用于将字符串文本转换为特定的类型
    /// </summary>
    [SafeAccess]
    public abstract class TypeConverter : ITypeConverter
    {
        public object ConvertTo(string value, Type destinationType)
        {
            //如果是空字符串那么返回默认值
            if (string.IsNullOrEmpty(value)) return GetDefaultValue(destinationType);

            if (destinationType.IsEnum) return ConvertTo(value);

            TypeCode code = Type.GetTypeCode(destinationType);
            switch (code)
            {
                case TypeCode.Boolean: return bool.Parse(value);
                case TypeCode.Byte: return byte.Parse(value);
                case TypeCode.DateTime: return DateTime.Parse(value);
                case TypeCode.Decimal: return decimal.Parse(value);
                case TypeCode.Double: return double.Parse(value);
                case TypeCode.Int16: return short.Parse(value);
                case TypeCode.Int32: return int.Parse(value);
                case TypeCode.Int64: return long.Parse(value);
                case TypeCode.SByte: return sbyte.Parse(value);
                case TypeCode.Single: return float.Parse(value);
                case TypeCode.String: return value;
                case TypeCode.UInt16: return ushort.Parse(value);
                case TypeCode.UInt32: return int.Parse(value);
                case TypeCode.UInt64: return ulong.Parse(value);
                case TypeCode.Object:
                    {
                        if (destinationType == typeof(Guid)) return Guid.Parse(value);
                        return ConvertTo(value);
                    }
            }
            throw new XamlException("从字符串转换成类型" + destinationType.FullName + "出现意外错误");
        }

        private object GetDefaultValue(Type destinationType)
        {
            if (destinationType.IsEnum) return GetDefaultValue();

            TypeCode code = Type.GetTypeCode(destinationType);
            switch (code)
            {
                case TypeCode.Boolean: return default(bool);
                case TypeCode.Byte: return default(byte);
                case TypeCode.DateTime: return default(DateTime);
                case TypeCode.Decimal: return default(decimal);
                case TypeCode.Double: return default(double);
                case TypeCode.Int16: return default(short);
                case TypeCode.Int32: return default(int);
                case TypeCode.Int64: return default(long);
                case TypeCode.SByte: return default(sbyte);
                case TypeCode.Single: return default(float);
                case TypeCode.String: return string.Empty;
                case TypeCode.UInt16: return default(ushort);
                case TypeCode.UInt32: return default(uint);
                case TypeCode.UInt64: return default(ulong);
                case TypeCode.Object:
                    {
                        if (destinationType == typeof(Guid)) return Guid.Empty;
                        return GetDefaultValue();
                    }
            }
            throw new XamlException("获取类型" + destinationType.FullName + "的默认值出现意外错误");
        }

        /// <summary>
        /// 当传递的值为空字符串时，得到目标类型的默认值
        /// </summary>
        /// <returns></returns>
        protected abstract object GetDefaultValue();

        protected abstract object ConvertTo(string value);

    }
}
