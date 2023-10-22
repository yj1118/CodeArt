using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using CodeArt.DTO;
using CodeArt.Web.RPC;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;
using CodeArt.Web.WebPages;
using CodeArt.Util;
using HtmlAgilityPack;
using CodeArt;
using CodeArt.Web;

namespace RPC.Common
{
    public static class Helper
    {

        /// <summary>
        /// 将格式化的内容转换为可以在页面上呈现的html代码
        /// </summary>
        /// <returns></returns>
        public static string FormatOutput(string html, bool lazy = true)
        {
            var imageHost = ConfigurationManager.AppSettings["thumbnailUrl"];

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            ParseOutput(doc.DocumentNode, imageHost, lazy);
            return doc.DocumentNode.OuterHtml;
        }

        private static void ParseOutput(HtmlNode node, string imageHost, bool lazy)
        {
            for (var i = 0; i < node.ChildNodes.Count; i++)
            {
                var child = node.ChildNodes[i];
                if (child.Name == "img" && child.GetAttributeValue("data-upload", string.Empty) == "true")
                {

                    var src = child.Attributes["src"].Value;
                    if (src.IndexOf("[imageHost]") > -1)
                    {
                        if (lazy)
                        {
                            //Lazy加载需要修改的相关属性
                            child.AddClass("img-lazyload");
                            src = src.Replace("[imageHost]", imageHost);
                            child.SetAttributeValue("data-src", src);
                            //child.SetAttributeValue("src", "javascript:void(0)");

                            //去除src属性，避免img发送多余的请求
                            var srcAttribute = child.Attributes.FirstOrDefault((t) => { return t.Name == "src"; });
                            child.Attributes.Remove(srcAttribute);
                        }
                        else
                        {
                            //重写地址
                            child.Attributes["src"].Value = src.Replace("[imageHost]", imageHost);
                        }

                    }
                }

                ParseOutput(child, imageHost, lazy);
            }
        }

        public static void ProcessImage(DTObject row, string imageKeyExp, ImageSize size, int cutType = 2, string name = "image", string category = "saleable", int quality = 30)
        {
            ProcessImage(row, imageKeyExp, size.Width, size.Height, cutType, name, category, quality);
        }

        public static void ProcessImage(DTObject row, string imageKeyExp, int width, int height, int cutType = 2, string name = "image", string category = "saleable", int quality = 30)
        {
            string key = row.GetValue<string>(imageKeyExp, string.Empty);
            //if (string.IsNullOrEmpty(key)) return;
            var url = ImageUtil.GetDynamicUrl(category, key, width, height, cutType, quality);
            row.Transform(_getTransformExp(imageKeyExp));
            row.SetValue(name, url);
        }

        private static Func<string, string> _getTransformExp = LazyIndexer.Init<string, string>((imageKeyExp) =>
        {
            return "!" + imageKeyExp;
        });

    }
}