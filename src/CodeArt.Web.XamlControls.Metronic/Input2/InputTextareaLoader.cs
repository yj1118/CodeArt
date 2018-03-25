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
using CodeArt.Web.XamlControls.Bootstrap;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class InputTextareaLoader : InputTextLoader
    {
        protected override void Load(object obj, HtmlNode objNode)
        {
            base.Load(obj, objNode);
        }


        protected override string GetType(HtmlNode node)
        {
            return "Textarea";
        }

        public new static readonly InputTextareaLoader Instance = new InputTextareaLoader();
    }
}
