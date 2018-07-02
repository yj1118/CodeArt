using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages;
using CodeArt.Concurrent;

namespace CodeArt.Web.XamlControls.Metronic
{
    [SafeAccess]
    public class DiskAssetsPathBindingExtensionConverter : TypeConverter
    {
        protected override object GetDefaultValue()
        {
            return null;
        }

        protected override object ConvertTo(string value)
        {
            var expression = GetExpression(value);
            if (string.IsNullOrEmpty(expression)) throw new XamlException("disk路径绑定表达式" + value + "格式不正确");
            return new DiskAssetsPathBindingExtension(expression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetExpression(string value)
        {
            var pos = value.IndexOf(" ");
            if (pos > -1) return value.Substring(pos + 1, value.Length - pos - 2);
            return string.Empty;
        }

    }
}