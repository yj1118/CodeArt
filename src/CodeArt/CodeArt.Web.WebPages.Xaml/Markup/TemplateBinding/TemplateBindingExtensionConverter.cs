using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Concurrent;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    [SafeAccess]
    public class TemplateBindingExtensionConverter : TypeConverter
    {
        protected override object GetDefaultValue()
        {
            return null;
        }

        protected override object ConvertTo(string value)
        {
            var expression = GetExpression(value);
            if (string.IsNullOrEmpty(expression)) throw new XamlException("模板绑定表达式" + value + "格式不正确");
            return new TemplateBindingExtension(expression);
        }

        /// <summary>
        /// 获取绑定源的路径
        /// </summary>
        /// <returns></returns>
        private string GetExpression(string value)
        {
            return value.Substring(1, value.Length - 2);//移除{ }
        }

    }
}