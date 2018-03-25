using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Runtime;
using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Markup;
using HtmlAgilityPack;
using CodeArt.HtmlWrapper;
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages.Xaml
{
    [SafeAccess]

    public class SetterLoader : ComponentLoader
    {
        protected override void Load(object obj, HtmlNode objNode)
        {
            var ctx = LoadContext.Current;
            var style = ctx.Parent as XamlStyle;

            if (style == null) throw new XamlException("Setter的父对象必须为XamlStyle");
            var targetType = style.TargetType;
            if (targetType == null) throw new XamlException("请指定正确的Style.TargetType属性");

            var propertyName = objNode.GetAttributeValue("Property", string.Empty);
            if (string.IsNullOrEmpty(propertyName)) throw new XamlException("请指定正确的Setter.Property属性");

            var setter = obj as Setter;
            setter.Property = DependencyProperty.GetProperty(targetType, propertyName);

            string value = objNode.GetAttributeValue("Value", string.Empty);
            if(!string.IsNullOrEmpty(value))
            {
                //简单值
                var propertyInfo = targetType.ResolveProperty(propertyName);
                PropertiesLoader.SetValue(setter, Setter.ValueProperty, propertyInfo, value);
            }
            else
            {
                var valueNode = objNode.SelectSingleNodeEx("Setter.Value");
                if(valueNode != null)
                {
                    setter.Value = XamlReader.ReadComponent(valueNode.InnerHtml);
                }
            }
            
        }
    }
}
