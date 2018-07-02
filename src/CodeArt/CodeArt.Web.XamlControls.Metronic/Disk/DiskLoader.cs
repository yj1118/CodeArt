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
    public class DiskLoader : ComponentLoader
    {
        protected override void Load(object obj, HtmlNode objNode)
        {
            base.Load(obj, objNode);

            var element = obj as Disk;
            if (element == null) return;


            UIUtil.CheckProperties(objNode, "id");
            var target = objNode.GetAttributeValue("target", string.Empty);
            if (string.IsNullOrEmpty(target)) objNode.SetAttributeValue("getTarget", GetDiskGetTarget());

            if(string.IsNullOrEmpty(element.AssetsPath)) element.AssetsPath = GetAssetsPath();
            objNode.SetAttributeValue("assetsPath", element.AssetsPath);

            element.Class = GetClassName(element, objNode);
            element.ProxyCode = GetProxyCode(element, objNode);

        }

        private string GetClassName(Disk obj, HtmlNode objNode)
        {
            return UIUtil.GetClassName(objNode, "container-fluid disk");
        }

        private string GetProxyCode(object obj, HtmlNode node)
        {
            return UIUtil.GetProxyCode(obj as UIElement, node, "$$metronic.disk.create()",
                                        UIUtil.GetJSONMembers(node, "target:", "getTarget:", "height:", "assetsPath:", "extensions:"),
                                        UIUtil.GetJSONMembers(node));
        }

        internal static string GetAssetsPath()
        {
            var assetsPath = ConfigurationManager.AppSettings["metronic:diskAssetsPath"];
            if (string.IsNullOrEmpty(assetsPath)) throw new WebException("没有配置metronic:diskAssetsPath");
            if (assetsPath.EndsWith("/")) assetsPath = assetsPath.Substring(0, assetsPath.Length - 1);
            return assetsPath;
        }

        /// <summary>
        /// 获取“获取磁盘处理请求的页面路径，这是动态获取的”
        /// </summary>
        /// <returns></returns>
        internal static string GetDiskGetTarget()
        {
            var target = ConfigurationManager.AppSettings["metronic:diskGetTarget"];
            if (string.IsNullOrEmpty(target)) throw new WebException("没有配置metronic:diskGetTarget");
            return target;
        }

    }



}
