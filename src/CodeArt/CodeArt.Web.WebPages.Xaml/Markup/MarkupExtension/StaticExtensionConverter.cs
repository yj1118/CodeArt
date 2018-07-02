using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages;
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    [SafeAccess]
    public class StaticExtensionConverter : TypeConverter
    {
        protected override object GetDefaultValue()
        {
            return null;
        }

        protected override object ConvertTo(string value)
        {
            var path = GetPath(value);
            if (string.IsNullOrEmpty(path)) throw new XamlException("静态绑定表达式" + path + "格式不正确");
            return new StaticExtension(path);
        }

        /// <summary>
        /// 获取绑定源的路径
        /// </summary>
        /// <returns></returns>
        private string GetPath(string value)
        {
            var pos = value.IndexOf(" ");
            if (pos > -1) return value.Substring(pos + 1, value.Length - pos - 2);
            return string.Empty;
        }

    }
}