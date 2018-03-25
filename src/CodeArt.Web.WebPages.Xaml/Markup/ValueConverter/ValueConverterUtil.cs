using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using CodeArt.Util;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    internal static class ValueConverterUtil
    {
        private static RegexOptions _flags = RegexOptions.IgnoreCase | RegexOptions.Compiled;

        /// <summary>
        /// 从表达式中得到路径
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetPath(string expression, bool templateBinding)
        {
            Regex reg = templateBinding ? new Regex(@"TemplateBinding[ ]+?([^,=]+)", _flags) : new Regex(@"Binding[ ]+?([^,=]+)", _flags);
            var match = reg.Match(expression);
            if (!match.Success) return string.Empty;
            var path = match.Groups[1].Value.Trim();
            if (path.EqualsIgnoreCase("Converter") || path.EqualsIgnoreCase("ConverterParameter")) return string.Empty;
            return path;
        }

        /// <summary>
        /// 从表达式中得到值转换器对象
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IValueConverter GetConverter(string expression, object target, string propertyName)
        {
            Regex reg = new Regex(@"[, ]Converter[ ]*=[ ]*(\{[^,]*\})", _flags);
            var match = reg.Match(expression);
            if (!match.Success) return null;
            var vcExpression = match.Groups[1].Value;
            var value = ProvideValue(vcExpression, target, propertyName);
            var converter = value as IValueConverter;
            if (converter == null) throw new TypeMismatchException(value.GetType(), typeof(IValueConverter));
            return converter;
        }

        /// <summary>
        /// 从表达式中得到值转换器需要用到的参数
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static object GetParameter(string expression, object target, string propertyName)
        {
            Regex reg = new Regex(@"[, ]ConverterParameter[ ]*=[ ]*((\{[^,]*\})|([^,]+))", _flags);
            var match = reg.Match(expression);
            if (!match.Success) return null;
            var cpExpression = match.Groups[1].Value;
            return ProvideValue(cpExpression, target, propertyName);
        }

        /// <summary>
        /// 从表达式中得到值
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="target"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private static object ProvideValue(string expression, object target, string propertyName)
        {
            var extension = MarkupExtensionFactory.Create(expression);
            if (extension == null) return expression;  //不是标记，直接返回表达式作为值
            var value = extension.ProvideValue(target, propertyName);
            var exp = value as Expression;
            if (exp != null)
            {
                //如果是表达式，那么执行表达式后再获取值
                value = exp.GetValue(target, propertyName);
            }
            return value;
        }



    }
}
