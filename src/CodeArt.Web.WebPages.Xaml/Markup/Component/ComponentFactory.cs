using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;
using CodeArt.HtmlWrapper;

using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    /// <summary>
    /// 创建组件,但并不加载组件信息
    /// </summary>
    internal class ComponentFactory : IComponentFactory
    {
        private ComponentFactory() { }

        public object CreateComponent(string componentName)
        {
            var type = ComponentTypeLocator.Locate(componentName) ?? HtmlElement.Type;
            if (type == null) throw new XamlException("没有找到" + componentName + "的定义");
            if(DataUtil.IsPrimitiveType(type))
            {
                return type; //如果是基础类型，那么直接返回类型本身
            }
            return SafeAccessAttribute.CreateInstance(type);
        }

        public static object Create(string componentName)
        {
            return Instance.CreateComponent(componentName);
        }

        private static ComponentFactory Instance = new ComponentFactory(); 

    }
}
