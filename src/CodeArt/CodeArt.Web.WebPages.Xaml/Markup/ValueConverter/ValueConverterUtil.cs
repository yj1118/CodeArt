using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using CodeArt.Concurrent;
using CodeArt.Util;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    internal static class ValueConverterUtil
    {
        private static RegexOptions _flags = RegexOptions.IgnoreCase | RegexOptions.Compiled;


        #region GetPath

        private static RegexPool _getTemplateBindingPathRegex = new RegexPool(@"TemplateBinding[ ]+?([^,=]+)", _flags);
        private static RegexPool _getBindingPathRegex = new RegexPool(@"Binding[ ]+?([^,=]+)", _flags);

        private static Func<string, Func<bool, string>> _getPath = LazyIndexer.Init<string, Func<bool, string>>((expression) =>
        {
            return LazyIndexer.Init<bool, string>((templateBinding) =>
            {
                using (var temp = templateBinding ? _getTemplateBindingPathRegex.Borrow() : _getBindingPathRegex.Borrow())
                {
                    var reg = temp.Item;
                    var match = reg.Match(expression);
                    if (!match.Success) return string.Empty;
                    var path = match.Groups[1].Value.Trim();
                    if (path.EqualsIgnoreCase("Converter") || path.EqualsIgnoreCase("ConverterParameter")) return string.Empty;
                    return path;
                }
            });
        });

        /// <summary>
        /// 从表达式中得到路径
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetPath(string expression, bool templateBinding)
        {
            return _getPath(expression)(templateBinding);
        }

        #endregion


        #region GetConverter

        private static RegexPool _getConverterExpressionRegex = new RegexPool(@"[, ]Converter[ ]*=[ ]*(\{[^,]*\})", _flags);


        private static Func<string, string> getConverterExpression = LazyIndexer.Init<string, string>((expression) =>
        {
            using (var temp = _getConverterExpressionRegex.Borrow())
            {
                var reg = temp.Item;
                var match = reg.Match(expression);
                if (!match.Success) return null;
                return match.Groups[1].Value;
            }
        });

        /// <summary>
        /// 从表达式中得到值转换器对象
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IValueConverter GetConverter(string expression, object target, string propertyName)
        {
            var vcExpression = getConverterExpression(expression);
            if (vcExpression == null) return null;
            var value = ProvideValue(vcExpression, target, propertyName);
            var converter = value as IValueConverter;
            if (converter == null) throw new TypeMismatchException(value.GetType(), typeof(IValueConverter));
            return converter;
        }

   
        #endregion



        #region GetParameter

        private static RegexPool _getParameterExpressionRegex = new RegexPool(@"[, ]ConverterParameter[ ]*=[ ]*((\{[^,]*\})|([^,]+))", _flags);


        private static Func<string, string> getParameterExpression = LazyIndexer.Init<string, string>((expression) =>
        {
            using (var temp = _getParameterExpressionRegex.Borrow())
            {
                var reg = temp.Item;
                var match = reg.Match(expression);
                if (!match.Success) return null;
                return match.Groups[1].Value;
            }
        });

        /// <summary>
        /// 从表达式中得到值转换器需要用到的参数
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static object GetParameter(string expression, object target, string propertyName)
        {
            var cpExpression = getParameterExpression(expression);
            if (cpExpression == null) return null;
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

        #endregion
    }
}
