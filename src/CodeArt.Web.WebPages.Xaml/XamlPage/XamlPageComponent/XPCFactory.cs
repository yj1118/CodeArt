using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Web;
using System.Text;
using System.IO;

using CodeArt.Util;
using HtmlAgilityPack;
using CodeArt.HtmlWrapper;
using CodeArt.Runtime;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml.Controls;

namespace CodeArt.Web.WebPages.Xaml
{
    internal class XPCFactory : IXPCFactory
    {
        private XPCFactory()
        {
        }

        /// <summary>
        /// 根据页面路径，创建组件
        /// 每个页面路径对应唯一一组的组件，不会重复创建
        /// 但是页面与页面的组件树是不同的
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public object CreateComponent(string virtualPath)
        {
            return _getComponent(virtualPath);
            //if (_isSingleton(virtualPath)) return _getComponentBySingleton(virtualPath);
            //Type componentType = _getComponentType(virtualPath);
            //if (componentType == null) return null;
            //return Activator.CreateInstance(componentType);
        }

        public Type GetComponentType(string virtualPath)
        {
            return _getComponentType(virtualPath);
        }

        ///// <summary>
        ///// 判断虚拟路径对应的对象是否为单例的
        ///// </summary>
        //private static Func<string, bool> _isSingleton = LazyIndexer.Init<string, bool>((virtualPath) =>
        //{
        //    Type componentType = _getComponentType(virtualPath);
        //    if (componentType == null) return false;
        //    return SingletonAttribute.GetAttribute(componentType).IsSingleton;
        //});

        #region componentType

        private static Func<string, Type> _getComponentType = LazyIndexer.Init<string, Type>((virtualPath) =>
        {
            var code = PageUtil.GetRawCode(virtualPath);
            if (string.IsNullOrEmpty(code) || !XamlUtil.IsDeclareXaml(code)) return null;
            var node = XamlUtil.GetNode(code);
            if (node == null) return null;
            var type = GetXamlPageComponentType(node, virtualPath);
            if (type == null) type = GetComponentTypeByTagName(node);
            return type;
        });

        /// <summary>
        /// 获得xaml页面组件的类型
        /// 所谓xaml页面组件，指的是站点自己创建的，与某个xaml页面相关的组件
        /// 该组件一般会基于一个xaml组件做扩展
        /// </summary>
        /// <returns></returns>
        private static Type GetXamlPageComponentType(HtmlNode node, string virtualPath)
        {
            var className = node.GetAttributeValue("x:Class", XPCCoder.GetClassName(virtualPath));
            //var className = XPCCoder.GetClassName(virtualPath);
            if (string.IsNullOrEmpty(className)) return null;
            if (className.IndexOf(".") == -1) className = string.Format("App_Code.{0}", className); //没有指定命名空间，那么默认为App_Code的命名空间
            var assembly = XPCAssembly.Load(virtualPath);
            if (assembly == null) return null;
            return assembly.GetType(className, false, true);
            //className = string.Format("{0},{1}", className, XPCCoder.App_CodeAssemblyName);
            //return Type.GetType(className, false, true);
        }

        /// <summary>
        /// 根据节点名称得到类型
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static Type GetComponentTypeByTagName(HtmlNode node)
        {
            var xmlns = XmlnsDictionary.Collect(node);
            var xamlNamespace = xmlns.GetXamlNamespace(node.Name);
            return ComponentTypeLocator.Locate(xamlNamespace, node.Name) ?? HtmlElement.Type;
        }


        #endregion

        private static Func<string, object> _getComponent = LazyIndexer.Init<string, object>((virtualPath) =>
        {
            Type componentType = _getComponentType(virtualPath);
            if (componentType == null) return null;
            var component = Activator.CreateInstance(componentType);

            if (!(component is IComponentConnector))
            {
                //不是连接器，表示没有后台代码，那么需要加载组件信息
                var xaml = PageUtil.GetRawCode(virtualPath);
                XamlReader.LoadComponent(component, xaml);
            }

            return component;
        });


        private static XPCFactory Instance = new XPCFactory();

        /// <summary>
        /// 每个页面路径对应唯一一组逻辑树，也就是说，对于每个页面路径，多人访问得到的组件是相同的
        /// 不同页面路径的逻辑树是独立的
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public static object Create(string virtualPath)
        {
            return XPCFactory.Instance.CreateComponent(virtualPath);
        }

        public static Type GetType(string virtualPath)
        {
            return _getComponentType(virtualPath);
        }
    }
}
