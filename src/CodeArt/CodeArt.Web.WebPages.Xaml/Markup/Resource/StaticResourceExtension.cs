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
    [TypeConverter(typeof(StaticResourceExtensionConvert))]
    public class StaticResourceExtension : MarkupExtension
    {
        public string ResurceKey { get; private set; }

        public StaticResourceExtension(string resourceKey)
        {
            this.ResurceKey = resourceKey;
        }

        public override object ProvideValue(object target, DependencyProperty property)
        {
            var e = target as FrameworkElement;
            if (e == null) throw new XamlException("无法对类型" + e.GetType().FullName + "应用静态资源标记扩展表达式");
            return e.FindResource(this.ResurceKey);
        }

        public override object ProvideValue(object target, string propertyName)
        {
            var e = target as FrameworkElement;
            if (e == null) throw new XamlException("无法对类型" + e.GetType().FullName + "应用静态资源标记扩展表达式");
            return e.FindResource(this.ResurceKey);
        }
    }
}
