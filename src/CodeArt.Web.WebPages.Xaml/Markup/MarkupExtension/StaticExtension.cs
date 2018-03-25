using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

using CodeArt.Web;
using CodeArt.Web.WebPages;
using CodeArt.Util;
using CodeArt.Concurrent;
using CodeArt.Runtime;


namespace CodeArt.Web.WebPages.Xaml.Markup
{
    /// <summary>
    /// 如果标记表达式为static Strings.xxx，那么表示提取全局资源文件Strings里的数据
    /// Strings作为保留的资源文件关键字专门用于存储多语言的字符串数据
    /// </summary>
    [TypeConverter(typeof(StaticExtensionConverter))]
    public class StaticExtension : MarkupExtension
    {
        private Expression _expression;

        public StaticExtension(string path)
        {
            ArgumentAssert.IsNotNullOrEmpty(path, "path");
            Init(path);
        }

        private void Init(string path)
        {
            if(path.StartsWith("Strings."))
            {
                var resourceKey = path.Substring("Strings.".Length);
                _expression = GetLanguageExpression(string.Empty, resourceKey);
            }
            else if(path.IndexOf(":Strings.") > -1)
            {
                var pos = path.IndexOf(":Strings.");
                var prefixName = path.Substring(0, pos);
                var resourceKey = path.Substring(pos + ":Strings.".Length);
                _expression = GetLanguageExpression(prefixName, resourceKey);
            }
            else
            {
                var index = path.IndexOf('.');
                var ownerTypeName = path.Substring(0, index);
                var memberName = path.Substring(index + 1, (path.Length - index) - 1);

                var ownerType = ComponentTypeLocator.Locate(ownerTypeName);
                if (ownerType == null) throw new XamlException("没有找到类型" + ownerTypeName);

                _expression = new StaticReferenceExpression(path, ownerType, memberName);
            }
        }

        private LanguageExpression GetLanguageExpression(string prefixName, string resourceKey)
        {
            if(prefixName == "x" && resourceKey == "AddFeatures")
            {

            }

            var assembly = ComponentTypeLocator.GetAssembly(prefixName);
            return new LanguageExpression(assembly, resourceKey);
        }

        private object ProvideValue()
        {
            return _expression;
        }

        public override object ProvideValue(object target, DependencyProperty property)
        {
            return ProvideValue();
        }

        public override object ProvideValue(object target, string propertyName)
        {
            return ProvideValue();
        }
    }
}
