using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Markup;
using HtmlAgilityPack;
using CodeArt.HtmlWrapper;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.Web.XamlControls.Bootstrap;
using CodeArt.Concurrent;

namespace CodeArt.Web.XamlControls.Metronic
{
    [SafeAccess]
    public class InputLoaderFactory : IComponentLoaderFactory
    {
        public ComponentLoader CreateLoader(object obj, HtmlNode objNode)
        {
            var type = objNode.GetAttributeValue("type", "text");
            switch (type)
            {
                case "text": return InputTextLoader.Instance;
                case "textarea": return InputTextareaLoader.Instance;
                case "upload": return InputUploadLoader.Instance;
            }
            throw new XamlException("没有找到input " + type + " 的组件加载器");
        }
    }
}
