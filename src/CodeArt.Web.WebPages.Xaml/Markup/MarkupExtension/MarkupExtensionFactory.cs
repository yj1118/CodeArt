using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using CodeArt.Web.WebPages;
using CodeArt.Util;
using CodeArt.Concurrent;

using CodeArt.Web.WebPages.Xaml;
using CodeArt.Runtime;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    /// <summary>
    /// 
    /// </summary>
    public static class MarkupExtensionFactory
    {
        private static readonly Type _markupExtensionType = typeof(MarkupExtension);

        /// <summary>
        /// 通过标记扩展表达式，创建标记扩展对象
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MarkupExtension Create(string expression)
        {
            if (!IsExpressionFormat(expression)) return null;
            var context = LoadContext.Current;
            var componentName = string.Format("{0}Extension", GetComponentName(expression));
            var type = ComponentTypeLocator.Locate(componentName);
            if (type == null) return null;
            if (!type.IsImplementOrEquals(_markupExtensionType)) throw new XamlException(type.FullName + "不是标记扩展元素");
            var converter = TypeConverterAttribute.CreateConverter(type);
            if (converter == null) throw new XamlException("没有找到标记扩展" + componentName + "的类型转换器");
            return converter.ConvertTo(expression, _markupExtensionType) as MarkupExtension;
        }

        private static string GetComponentName(string expression)
        {
            int pos = expression.IndexOf(" ");
            if (pos == -1) return expression.Substring(1, expression.Length - 1);
            return expression.Substring(1, pos - 1);
        }

        /// <summary>
        /// 给定字符串是否为标记扩展表达式
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        private static bool IsExpressionFormat(string value)
        {
            return value.StartsWith("{") && value.EndsWith("}");
        }

    }
}
