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
using CodeArt.Web.WebPages;

namespace CodeArt.Web.RPC
{
    internal class RPCPageLocator : IWebPageLocator
    {
        private RPCPageLocator()
        {
        }

        public IHttpHandler GetDefaultHandler()
        {
            return RPCPage.Instance;
        }

        /// <summary>
        /// 全局只需要一个xamlPage
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public IHttpHandler GetHandler(string virtualPath)
        {
            return RPCPage.Instance;
        }

        public IAspect[] GetAspects(string virtualPath)
        {
            return AspectAttribute.GetAspects(RPCPage.Instance.GetType());
        }

        public IAspect[] GetDefaultAspects()
        {
            return AspectAttribute.GetAspects(RPCPage.Instance.GetType());
        }

        public static readonly RPCPageLocator Instance = new RPCPageLocator();
    }
}
