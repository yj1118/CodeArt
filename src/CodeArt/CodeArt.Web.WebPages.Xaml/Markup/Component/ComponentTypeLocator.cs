using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

using HtmlAgilityPack;
using CodeArt.HtmlWrapper;
using CodeArt.Util;

using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    internal static class ComponentTypeLocator
    {
        /// <summary>
        /// 定位节点对应的xaml组件的类型，注意该方法忽略了x:Class属性，是以xaml根节点的名称为判断依据
        /// </summary>
        /// <param name="componentNode"></param>
        /// <returns></returns>
        public static Type Locate(HtmlNode componentNode)
        {
            if (componentNode == null || componentNode.OriginalName.IndexOf(".") > -1) return null;
            if (componentNode.NodeType == HtmlNodeType.Text) return Run.Type;
            if (componentNode.NodeType == HtmlNodeType.Comment) return Comment.Type;
            var xmlns = XmlnsDictionary.Collect(componentNode);
            var xamlNamespace = xmlns.GetXamlNamespace(componentNode.OriginalName);
            return Locate(xamlNamespace, componentNode.LocalOriginalName());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xamlNamespace"></param>
        /// <param name="componentName">不带命名前缀的元素实际名称</param>
        /// <returns></returns>
        public static Type Locate(string xamlNamespace, string componentName)
        {
            if (componentName.IndexOf(".") > -1) return null;
            if (componentName == "#text") return Run.Type;
            if (componentName == "#comment") return Comment.Type;
            if (xamlNamespace == null) return null;

            var getType = _getTypeMethod(xamlNamespace);
            var type = getType == null ? null : getType(componentName);
            //if (type == null) type = _htmlElementType; //找不到控件类型的，默认为htmlElement控件
            return type;
        }

        public static Type Locate(string componentName)
        {
            var xmlns = XmlnsDictionary.Current;
            var xamlNamespace = xmlns.GetXamlNamespace(componentName);
            return ComponentTypeLocator.Locate(xamlNamespace, componentName);
        }

        /// <summary>
        /// 根据xmlns前缀，获取程序集信息
        /// </summary>
        /// <param name="prefixName"></param>
        /// <returns></returns>
        public static Assembly GetAssembly(string prefixName)
        {
            var xmlns = XmlnsDictionary.Current;
            var xamlNamespace = xmlns.GetXamlNamespaceByPrefixName(prefixName);
            return XmlnsDefinitionAttribute.GetAssembly(xamlNamespace);
        }


        private static Func<string, Func<string, Type>> _getTypeMethod = LazyIndexer.Init<string, Func<string, Type>>((xamlNamespace) =>
        {
            return LazyIndexer.Init<string, Type>((componentName) =>
            {
                ArgumentAssert.IsNotNullOrEmpty(xamlNamespace, "xamlNamespace");
                ArgumentAssert.IsNotNullOrEmpty(componentName, "componentName");

                return GetBySpecified(xamlNamespace, componentName) ?? GetByDefinition(xamlNamespace, componentName);
            });
        });

        /// <summary>
        /// 通过指定的方式获取类型
        /// </summary>
        /// <param name="xamlNamespace"></param>
        /// <param name="componentName"></param>
        /// <returns></returns>
        private static Type GetBySpecified(string xamlNamespace, string componentName)
        {

            if (xamlNamespace.StartsWith("clr-namespace:")) xamlNamespace = xamlNamespace.Replace("clr-namespace:", "using:");

            if (xamlNamespace.StartsWith("using:")) //using:命名空间名称;assembly:程序集名称
            {
                return _getByUsing(xamlNamespace)(componentName);
            }
            return null;
        }

        private static readonly RegexPool _usingRegex = new RegexPool("using:([^;]+);assembly=([^;]+);?", RegexOptions.IgnoreCase);

        private static Func<string, Func<string, Type>> _getByUsing = LazyIndexer.Init<string, Func<string, Type>>((xamlNamespace) => {
            return LazyIndexer.Init<string, Type>((componentName) =>
            {
                using (var temp = _usingRegex.Borrow())
                {
                    var reg = temp.Item;
                    var match = reg.Match(xamlNamespace);
                    if (!match.Success) throw new WebException("无法识别的xmlns信息" + xamlNamespace);
                    var namespaceValue = match.Groups[1].Value;
                    var assemblyName = match.Groups[2].Value;

                    var localComponentName = GetLocalComponentName(componentName);
                    var typeName = string.Format("{0}.{1},{2}", namespaceValue, localComponentName, assemblyName);
                    var componentType = Type.GetType(typeName, false, true);
                    if (componentType == null) throw new NoTypeDefinedException("没有找到" + typeName + "的类型定义");
                    return componentType;
                }
            });
        });




        private static Type GetByDefinition(string xamlNamespace, string componentName)
        {
            var localComponentName = GetLocalComponentName(componentName);
            return XmlnsDefinitionAttribute.GetComponentType(xamlNamespace, localComponentName);
        }

        private static string GetLocalComponentName(string componentName)
        {
            int pos = componentName.IndexOf(":");
            return pos > -1 ? componentName.Substring(pos + 1) : componentName;
        }

    }
}
