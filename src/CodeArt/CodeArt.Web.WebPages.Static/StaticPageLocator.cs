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

namespace CodeArt.Web.WebPages.Static
{
    internal class StaticPageLocator : IWebPageLocator
    {
        private StaticPageLocator()
        {
        }

        public IHttpHandler GetDefaultHandler()
        {
            return StaticPage.Instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public IHttpHandler GetHandler(string virtualPath)
        {
            return StaticPage.Instance;
        }

        public IAspect[] GetAspects(string virtualPath)
        {
            return _aspects;
        }

        public IAspect[] GetDefaultAspects()
        {
            return _aspects;
        }

        private static IAspect[] _aspects = new IAspect[] { new WebPageInitializer() };

        public static StaticPageLocator Instance = new StaticPageLocator();
    }
}
