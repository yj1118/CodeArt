using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Compilation;

using System.Web;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

using CodeArt.Log;
using CodeArt.AppSetting;

namespace CodeArt.Web.WebPages
{
    internal class WebPageHttpModule : IHttpModule
    {
        private static readonly object _hasBeenRegisteredKey = new object();

        public void Dispose()
        {
        }

        public void Init(HttpApplication application)
        {
            var items = application.Context.Items;
            if (items[_hasBeenRegisteredKey] == null)
            {
                items[_hasBeenRegisteredKey] = true;
                InitApplication(application);
            }
        }

        internal static void InitApplication(HttpApplication application)
        {
            application.BeginRequest += application_BeginRequest;
            application.ResolveRequestCache += application_ResolveRequestCache;
            application.PostResolveRequestCache += application_PostResolveRequestCache;
            application.UpdateRequestCache += application_UpdateRequestCache;
            application.EndRequest += application_EndRequest;
            application.Error += application_Error;
        }

        static void application_Error(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            var context = app.Context;
            Exception ex = context.Server.GetLastError();
            if (ex is RedirectException)
            {
                context.Response.Redirect((ex as RedirectException).Url); 
                return;
            }
            if (ex != null)
                Logger.Fatal(ex);
        }

        static void application_BeginRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            var context = app.Context;
            string pathExtension = context.Request.CurrentExecutionFilePathExtension;
            if (!WebPageExtensions.IsValid(pathExtension)) return;  //验证扩展名是否有效
            var current = WebPageContext.Current = new WebPageContext(context);

            try
            {
                //初始化回话
                AppSession.Initialize();
                //执行页面关注点
                var aspects = current.PageAspects;
                if (aspects != null)
                {
                    foreach (var aspect in current.PageAspects)
                    {
                        aspect.Before();
                    }
                }
            }
            catch (Exception ex)
            {
                current.SetError(ex);
            }
        }

        static void application_ResolveRequestCache(object sender, EventArgs e)
        {
            var current = WebPageContext.Current;
            if (current == null) return;
            if (current.AnErrorOccurred || !current.IsGET) return;
            try
            {
                //检查缓存
                var cache = current.Cache;
                if (cache.LoadFromCache() || cache.RemoveCache())
                {
                    HttpApplication application = (HttpApplication)sender;
                    application.CompleteRequest();
                }
            }
            catch (Exception ex)
            {
                current.SetError(ex);
            }
        }

        static void application_PostResolveRequestCache(object sender, EventArgs e)
        {
            var current = WebPageContext.Current;
            if (current == null) return;
            var context = current.HttpContext;
            IHttpHandler handler = null;
            if (current.AnErrorOccurred) handler = current.Page;//错误页
            else
            {
                try
                {
                    //路由请求
                    handler = WebPageRouter.GetHandler(current);
                }
                catch (Exception ex)
                {
                    current.SetError(ex);
                    handler = current.Page;
                }
            }
            if (handler != null) context.RemapHandler(handler);
        }

        static void application_UpdateRequestCache(object sender, EventArgs e)
        {
            //var current = WebPageContext.Current;
            //if (current == null) return;
            //if (current.AnErrorOccurred || current.IsPostBack) return;
            //try
            //{
            //    //更新缓存
            //    var context = ((HttpApplication)sender).Context;
            //    var cache = context.Items["__cache"] as ResolveRequestCache;
            //    cache.SetCache(context.Response.OutputStream); 由于OutputStream不可读，因此改用别的办法更新缓存
            //}
            //catch (Exception ex)
            //{
            //    //写log
            //    throw ex;
            //}
        }

        static void application_EndRequest(object sender, EventArgs e)
        {
            var current = WebPageContext.Current;
            if (current == null) return;
            try
            {
                if (!current.AnErrorOccurred)
                {
                    //执行页面舞台
                    var aspects = current.PageAspects;
                    if (aspects != null)
                    {
                        foreach (var aspect in current.PageAspects)
                        {
                            aspect.After();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //写log
                throw ex;
            }
            finally
            {
                AppSession.Dispose();
            }
        }

    }
 

}
