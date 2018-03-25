using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Web;

using CodeArt.Util;
using CodeArt.Runtime;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class TemplateCodeFactoryAttribute : Attribute
    {
        public string TemplatePropertyName { get; private set; }

        public Type TemplateCodeFactoryType { get; private set; }

        public TemplateCodeFactoryAttribute(string templatePropertyName, Type templateCodeFactoryType)
        {
            this.TemplatePropertyName = templatePropertyName;
            this.TemplateCodeFactoryType = templateCodeFactoryType;
        }

        public static TemplateCodeFactoryAttribute GetAttribute(Type objType, string templatePropertyName)
        {
            var attrs = objType.GetCustomAttributes<TemplateCodeFactoryAttribute>(true);
            if (attrs == null) return TemplateCodeFactoryAttribute.Default;
            foreach (var item in attrs)
            {
                if (item.TemplatePropertyName.Equals(templatePropertyName, StringComparison.OrdinalIgnoreCase)) return item;
            }
            return TemplateCodeFactoryAttribute.Default;
        }

        public static TemplateCodeFactoryAttribute Default = new TemplateCodeFactoryAttribute("*", typeof(DefaultTemplateCodeFactory));

    }
}
