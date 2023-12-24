using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CodeArt.Web;
using CodeArt.Web.WebPages;
using CodeArt.Util;
using CodeArt.Concurrent;


namespace CodeArt.Web.WebPages.Xaml.Markup
{
    [SafeAccess]
    public class StaticResourceExtensionConvert : TypeConverter
    {
        protected override object GetDefaultValue()
        {
            return null;
        }

        protected override object ConvertTo(string value)
        {
            var resourceKey = GetResourceKey(value);
            if (string.IsNullOrEmpty(resourceKey)) throw new XamlException("静态资源标记扩展表达式" + value + "没有找到资源键");
            return new StaticResourceExtension(resourceKey);
        }

        /// <summary>
        /// 获取绑定源的路径
        /// </summary>
        /// <returns></returns>
        private string GetResourceKey(string value)
        {
            var pos = value.IndexOf(" ");
            if (pos > -1) return value.Substring(pos + 1, value.Length - pos - 2);
            return string.Empty;
        }

    }
}
