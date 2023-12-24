using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.ModuleNest;
using HtmlAgilityPack;
using CodeArt.Web.XamlControls.Bootstrap;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class DynamicInputLoader : ComponentLoader
    {
        protected override void Load(object obj, HtmlNode objNode)
        {
            base.Load(obj, objNode);

            var element = obj as DynamicInput;
            if (element == null) return;

            element.Class = GetClassName(element, objNode);
            element.ProxyCode = GetProxyCode(element, objNode);
        }

        private string GetClassName(DynamicInput obj, HtmlNode objNode)
        {
            return LayoutUtil.GetClassName(objNode, "row dynamic");
        }

        private string GetProxyCode(DynamicInput obj, HtmlNode objNode)
        {
            return UIUtil.GetProxyCode(obj, objNode, "$$.wrapper.metronic.input.createDynamic()"
                               , UIUtil.GetJSONMembers(objNode, "url:","xaml:true")
                               , UIUtil.GetJSONMembers(objNode, "id:", "name:", "form::''"));
        }
    }



}
