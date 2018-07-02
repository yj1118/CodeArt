using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Web;
using System.Text;

using CodeArt.AOP;

namespace CodeArt.Web.WebPages
{
    /// <summary>
    /// 基于虚拟路径的页面定位器
    /// </summary>
    public interface IWebPageLocator
    {
        /// <summary>
        /// 获取默认的处理器
        /// </summary>
        /// <returns></returns>
        IHttpHandler GetDefaultHandler();

        IHttpHandler GetHandler(string virtualPath);

        /// <summary>
        /// 获取页面关注点
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        IAspect[] GetAspects(string virtualPath);

        /// <summary>
        /// 获取默认的页面关注点
        /// </summary>
        /// <returns></returns>
        IAspect[] GetDefaultAspects();
    }

    internal sealed class WebPageTypeLocatorEmpty : IWebPageLocator
    {
        public IHttpHandler GetDefaultHandler()
        {
            return null;
        }

        public IHttpHandler GetHandler(string virtualPath)
        {
            return null;
        }

        private static IAspect[] _aspects = new IAspect[] { WebPageInitializer.Instance };

        public IAspect[] GetAspects(string virtualPath)
        {
            return _aspects;
        }

        public IAspect[] GetDefaultAspects()
        {
            return _aspects;
        }

        public static IWebPageLocator Instance = new WebPageTypeLocatorEmpty();

    }

}
