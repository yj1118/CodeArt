using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web.Compilation;

using System.Web;
using System.IO;

using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.Log;

using CodeArt.AOP;
using CodeArt.Concurrent;
using CodeArt.DTO;

namespace CodeArt.Web.WebPages.MultiTenancy
{
    /// <summary>
    /// 该对象是线程安全的，状态无关
    /// </summary>
    [SafeAccess()]
    [Aspect(typeof(WebPageInitializer))]
    public class TenancyPage : WebText
    {
        private ITenancy GetTenancy()
        {
            return TenancyManager.GetTenancy(WebPageContext.Current);
        }

        protected override string GetTextContent(string error)
        {
            var tenancy = GetTenancy();
            var context = ContextConverter.GetCode(tenancy, WebPageContext.Current);
            return tenancy.GetPage().Process(context);
        }

        protected override string CallWebMethod(DTObject args)
        {
            var tenancy = GetTenancy();
            var context = ContextConverter.GetCode(tenancy, WebPageContext.Current);
            return tenancy.GetPage().Process(context);
        }

        public static readonly TenancyPage Instance = new TenancyPage();

    }
}
