using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Markup;
using HtmlAgilityPack;
using CodeArt.HtmlWrapper;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.XamlControls.Bootstrap;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class InputUploadLoader : InputLoader
    {
        protected override void Load(object obj, HtmlNode objNode)
        {
            objNode.SetAttributeValue("type","upload");

            base.Load(obj, objNode);
        }

        protected override string GetProxyCode(object obj, HtmlNode node)
        {
            var target = node.GetAttributeValue("target", string.Empty);
            if (string.IsNullOrEmpty(target)) node.SetAttributeValue("getTarget", this.GetDiskGetTarget());

            var assetsPath = node.GetAttributeValue("assetsPath", string.Empty);
            if (string.IsNullOrEmpty(assetsPath)) node.SetAttributeValue("assetsPath", this.GetAssetsPath());

            return UIUtil.GetProxyCode(obj as UIElement, node, "$$.wrapper.metronic.input.createUpload()"
                                           , UIUtil.GetJSONMembers(node, "required", "min", "max", "validate", "extensions:", "pulsate", "call:", "disk", "maxFileSize", "target:", "getTarget:", "folderId:", "assetsPath:","xaml:true")
                                           , UIUtil.GetJSONMembers(node, "id:", "name:", "form::''"));
        }

        private string GetDiskGetTarget()
        {
            var target = ConfigurationManager.AppSettings["metronic:diskGetTarget"];
            if (string.IsNullOrEmpty(target)) throw new WebException("没有配置metronic:diskGetTarget");
            return target;
        }

        private string GetAssetsPath()
        {
            var assetsPath = ConfigurationManager.AppSettings["metronic:diskAssetsPath"];
            if (string.IsNullOrEmpty(assetsPath)) throw new WebException("没有配置metronic:diskAssetsPath");
            if (assetsPath.EndsWith("/")) assetsPath = assetsPath.Substring(0, assetsPath.Length - 1);
            return assetsPath;
        }

        public new static readonly InputUploadLoader Instance = new InputUploadLoader();

    }
}
