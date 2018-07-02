using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.ModuleNest;
using HtmlAgilityPack;


namespace CodeArt.Web.XamlControls.Metronic
{
    public class CropperLoader : ComponentLoader
    {
        protected override void Load(object obj, HtmlNode objNode)
        {
            base.Load(obj, objNode);

            var element = obj as Cropper;
            if (element == null) return;


            UIUtil.CheckProperties(objNode, "id");

            var target = objNode.GetAttributeValue("target", string.Empty);
            if (string.IsNullOrEmpty(target)) objNode.SetAttributeValue("getTarget", DiskLoader.GetDiskGetTarget());

            element.Class = GetClassName(element, objNode);
            element.ProxyCode = GetProxyCode(element, objNode);
        }

        private string GetClassName(Cropper obj, HtmlNode objNode)
        {
            return UIUtil.GetClassName(objNode, "cropper");
        }

        private string GetProxyCode(Cropper obj, HtmlNode node)
        {
            return UIUtil.GetProxyCode(obj as UIElement, node, "$$metronic.cropper.create()", 
                UIUtil.GetJSONMembers(node, "target:", "getTarget:", "onupload", "disk", "maxfilesize", "allowMode"), 
                UIUtil.GetJSONMembers(node));
        }
    }



}
