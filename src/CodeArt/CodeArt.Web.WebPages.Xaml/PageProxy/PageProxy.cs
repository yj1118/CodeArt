using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using HtmlAgilityPack;
using CodeArt.HtmlWrapper;

using CodeArt.DTO;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml.Script;


namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 页面代理
    /// xaml页面的根组件如果是页面代理，那么意味着由页面代理全权处理请求，而不走xaml机制
    /// </summary>
    public class PageProxy : DependencyObject, IPageProxy, IHtmlElementCore
    {
        public static DependencyProperty AttributesProperty { get; private set; }

        static PageProxy()
        {
            var attributesMetadata = new PropertyMetadata(() => { return new CustomAttributeCollection(); });
            AttributesProperty = DependencyProperty.Register<CustomAttributeCollection, PageProxy>("Attributes", attributesMetadata);
        }

        /// <summary>
        /// 自定义属性
        /// </summary>
        public CustomAttributeCollection Attributes
        {
            get
            {
                return GetValue(AttributesProperty) as CustomAttributeCollection;
            }
            set
            {
                SetValue(AttributesProperty, value);
            }
        }

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



        public virtual void Process(WebPageContext context)
        {
            if (context.IsGET) ProcessGET();
            else ProcessPOST();
        }

        protected virtual void ProcessGET() { }

        protected virtual void ProcessPOST() { }
    }
}
