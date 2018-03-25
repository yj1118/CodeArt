using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using CodeArt.Util;
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    [AttributeUsage(AttributeTargets.All)]
    public sealed class TypeConverterAttribute : Attribute
    {
        public TypeConverterAttribute(Type converterType)
        {
            this.ConverterType = converterType;
        }

        public Type ConverterType
        {
            get;
            private set;
        }



        /// <summary>
        /// 获取目标类型的类型转换器
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static TypeConverter CreateConverter(Type targetType)
        {
            var attr = targetType.GetCustomAttribute<TypeConverterAttribute>(true);
            if (attr == null) return null;
            return SafeAccessAttribute.CreateInstance<TypeConverter>(attr.ConverterType);
        }


    }

}
