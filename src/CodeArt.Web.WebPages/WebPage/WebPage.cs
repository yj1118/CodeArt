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

namespace CodeArt.Web.WebPages
{
    /// <summary>
    /// 该对象是线程安全的，状态无关
    /// </summary>
    [SafeAccess()]
    [Aspect(typeof(WebPageInitializer))]
    public class WebPage : IHttpHandler
    {
        public WebPageContext PageContext
        {
            get
            {
                return WebPageContext.Current;
            }
        }

        /// <summary>
        /// 当前页面的虚拟路径
        /// </summary>
        public string VirtualPath
        {
            get
            {
                return this.PageContext.VirtualPath;
            }
        }

        /// <summary>
        /// 指示页面是一个错误页
        /// </summary>
        public bool IsError
        {
            get
            {
                return this.PageContext.IsErrorPage;
            }
        }


        #region Http信息

        /// <summary>
        /// Http请求上下文
        /// </summary>
        public HttpContext Context
        {
            get { return this.PageContext.HttpContext; }
        }

        /// <summary>
        /// Http请求对象
        /// </summary>
        public HttpRequest Request
        {
            get { return this.Context.Request; }
        }

        /// <summary>
        /// Http响应对象
        /// </summary>
        public HttpResponse Response
        {
            get { return this.Context.Response; }
        }

        /// <summary>
        /// 请求类型
        /// </summary>
        public string RequestType
        {
            get
            {
                return this.PageContext.RequestType;
            }
        }

        #endregion

        public bool IsReusable
        {
            get { return false; }
        }

        protected WebPage()
        {
            _getKeyMethod = LazyIndexer.Init<string, MethodInfo>((methodName) =>
            {
                return this.GetType().ResolveMethod(methodName);
            });
        }

        protected virtual void InitHandler()
        {
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                InitHandler();
                if (this.RequestType == "GET")
                    ProcessGET();
                else if (this.RequestType == "POST")
                    ProcessPOST();
                //除此之外可能还会有OPTIONS
            }
            catch(TargetInvocationException ex)
            {
                //TargetInvocationException一般是由动态调用某个反射方法导致的错误的包装错误，所以需要脱去包装
                var e = ex.InnerException ?? ex;
                LogWrapper.Default.Fatal(e);
                ProcessError(e);
            }
            catch (Exception ex)
            {
                LogWrapper.Default.Fatal(ex);
                ProcessError(ex);
            }
        }

        #region POST

        protected virtual void ProcessPOST() { }

        #endregion

        #region GET

        /// <summary>
        /// 处理GET请求
        /// </summary>
        protected virtual void ProcessGET()
        {
            AspectInvoke("PageInit");//PageInit();

            using (Stream content = LoadContent())
            {
                if (content != null)
                    OutputContent(content);
            }

            AspectInvoke("PageRender");//PageRender();
        }

        /// <summary>
        /// 准备加载
        /// </summary>
        protected virtual void PreLoad() { }

        protected virtual Stream LoadContent() { return null; }

        protected virtual void OutputContent(Stream content)
        {
            WebPageWriter.Instance.Write(this.PageContext, content);
        }


        #endregion


        protected virtual void ProcessError(Exception ex)
        {
            if (ex is RedirectException) throw ex; //如果是跳转，抛出给更高级别的错误处理来处理
            this.Response.Clear();
            WebPageError.Process(ex);
        }

        #region 参数

        public T GetQueryValue<T>(string name, T defaultValue)
        {
            return this.PageContext.GetQueryValue<T>(name, defaultValue);
        }

        protected virtual NameValueCollection GetQueryValues()
        {
            return this.PageContext.GetQueryValues();
        }

        #endregion

        #region 线程安全的变量操作

        protected void SetSafeVariable(string name, object value)
        {
            this.PageContext.SetItem(name, value);
        }

        protected T GetSafeVariable<T>(string name)
        {
            return this.PageContext.GetItem<T>(name);
        }

        protected T GetSafeVariable<T>(string name,Func<T> createValue)
        {
            return this.PageContext.GetItem<T>(name, createValue);
        }

        #endregion

        #region 手机支持

        public bool IsMobileDevice
        {
            get
            {
                return this.PageContext.IsMobileDevice;
            }
            set
            {
                this.PageContext.IsMobileDevice = value;
            }
        }

        #endregion

        #region 页面跳转

        public void Redirect(string url)
        {
            throw new RedirectException(url);
        }

        #endregion

        #region 事件

        private Func<string, MethodInfo> _getKeyMethod;

        protected void AspectInvoke(string methodName)
        {
            var method = _getKeyMethod(methodName);
            if (method == null) throw new WebException("没有找到页面方法 " + methodName + "的信息");
            AspectAttribute.Invoke(method, this);
        }

        protected virtual void PageInit() { }

        protected virtual void PageRender() { }

        #endregion

    }
}
