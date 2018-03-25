using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        public virtual void Process(WebPageContext context)
        {
            if (context.IsGET) ProcessGET();
            else ProcessPOST();
        }

        protected virtual void ProcessGET() { }

        protected virtual void ProcessPOST() { }
    }
}
