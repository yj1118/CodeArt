using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Markup;
using HtmlAgilityPack;
using CodeArt.HtmlWrapper;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.Web.XamlControls.Bootstrap;
using CodeArt.Concurrent;

namespace CodeArt.Web.XamlControls.Metronic
{
    [SafeAccess]
    public class InputCoreLoaderFactory : IComponentLoaderFactory
    {
        public ComponentLoader CreateLoader(object obj, HtmlNode objNode)
        {
            var core = obj as InputCore;
            var type = core.Type;
            switch (type)
            {
                default:
                    return InputCoreLoader.Instance;
            }
        }
    }
}
