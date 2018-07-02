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
using CodeArt.AOP;
using HtmlAgilityPack;

namespace CodeArt.Web.WebPages.Xaml
{
    internal class XamlPageLocator : IWebPageLocator
    {
        private XamlPageLocator()
        {
        }

        public IHttpHandler GetDefaultHandler()
        {
            return XamlPage.Instance;
        }

        /// <summary>
        /// 全局只需要一个xamlPage
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public IHttpHandler GetHandler(string virtualPath)
        {
            return XamlPage.Instance;
        }

        public IAspect[] GetAspects(string virtualPath)
        {
            return _getAspects(virtualPath);
        }

        #region aspect

        public IAspect[] GetDefaultAspects()
        {
            return AspectAttribute.GetAspects(XamlPage.Instance.GetType());
        }

        private static Func<string, IAspect[]> _getAspects = LazyIndexer.Init<string, IAspect[]>((virtualPath) =>
       {
           Type componentType = XPCFactory.GetType(virtualPath);
           if (componentType == null) return null;
           return AspectAttribute.GetAspects(componentType);
       },(result)=>
       {
           return result != null; //不为null时才缓存
       });

        #endregion

        public static readonly XamlPageLocator Instance = new XamlPageLocator();
    }
}
