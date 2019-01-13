using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Compilation;

using System.Web;

using CodeArt.Web.WebPages;
using CodeArt.Concurrent;
using CodeArt.AOP;

namespace CodeArt.Web.WebPages.MultiTenancy
{
    [SafeAccess]
    internal class TenancyPageRouter : IWebPageRouter
    {
        public IHttpHandler CreateHandler(WebPageContext context)
        {
            var tenancy = TenancyManager.GetTenancy(context);
            if (tenancy == null) return VirtualPathRouter.Instance.CreateHandler(context);
            return TenancyPage.Instance;
        }

        public IAspect[] CreateAspects(WebPageContext context)
        {
            var tenancy = TenancyManager.GetTenancy(context);
            if (tenancy == null) return VirtualPathRouter.Instance.CreateAspects(context);
            return Array.Empty<IAspect>();
        }

        //private static IHttpHandler CreateFromVirtualPath(WebPageContext context, string path)
        //{
        //    WebPage page = CreatePage(context);
        //    context.Page = page;
        //    return page;
        //}

        //private static WebPage CreatePage(WebPageContext context)
        //{
        //    IWebPageLocator locator = WebPageLocatorFactory.CreateLocator(context.PathExtension);
        //    IHttpHandler handler = locator.GetHandler(context.VirtualPath);//利用资源定位器得到资源的Handler类型
        //    if (handler == null)
        //    {
        //        if (!context.IsValidPath())
        //        {
        //            //如果既没有后台文件，也没有前台文件，那么资源不存在
        //            throw new HttpException(404, string.Format(Strings.RequestResourceNotExist, context.VirtualPath));
        //        }
        //        handler = locator.GetDefaultHandler(); //如果有前台文件那么用默认的处理
        //    }
        //    WebPage page = handler as WebPage;
        //    if (page == null)
        //        throw new TypeMismatchException(handler.GetType(), typeof(WebPage));
        //    return page;
        //}


        //public IAspect[] CreateAspects(WebPageContext context)
        //{
        //    IWebPageLocator locator = WebPageLocatorFactory.CreateLocator(context.PathExtension);
        //    var aspects = locator.GetAspects(context.VirtualPath);
        //    if (aspects == null)
        //    {
        //        if (!context.IsValidPath())
        //        {
        //            //如果既没有后台文件，也没有前台文件，那么资源不存在
        //            throw new HttpException(404, string.Format(Strings.RequestResourceNotExist, context.VirtualPath));
        //        }
        //        aspects = locator.GetDefaultAspects(); //如果有前台文件那么用默认的处理
        //    }
        //    return aspects;
        //}

        public static TenancyPageRouter Instance = new TenancyPageRouter();
    }
}
