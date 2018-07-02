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
    [TypeConverter(typeof(DynmicResourceExtensionConvert))]
    public class DynamicResourceExtension : MarkupExtension
    {
        public string ResurceKey { get; private set; }

        public DynamicResourceExtension(string resourceKey)
        {
            this.ResurceKey = resourceKey;
        }

        public override object ProvideValue(object target, DependencyProperty property)
        {
            return new ResourceReferenceExpression(this.ResurceKey);
        }

        public override object ProvideValue(object target, string propertyName)
        {
            return new ResourceReferenceExpression(this.ResurceKey);
        }

    }

}
