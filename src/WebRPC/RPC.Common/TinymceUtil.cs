using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using CodeArt.DTO;
using CodeArt;
using CodeArt.Util;

using HtmlAgilityPack;

namespace RPC.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class TinymceUtil
    {
        private static string _host;

        static TinymceUtil()
        {
            _host = ConfigurationManager.AppSettings["tinymce-host"];
            if (string.IsNullOrEmpty(_host)) throw new UserUIException("没有配置tinymce-host");
        }

        //public static string GetBrief(string html,int length = 50)
        //{
        //    if (string.IsNullOrEmpty(html)) return string.Empty;

        //    HtmlDocument doc = new HtmlDocument();
        //    doc.LoadHtml(html);

        //    return doc.Text.Substr(0, length);
        //}


        /// <summary>
        /// 将代码转为可以存储的通用格式的html代码，注意，对于上传的文件，一共不能超过50个
        /// </summary>
        /// <param name="html"></param>
        /// <param name="imageHost"></param>
        /// <returns></returns>
        public static (string Content, string Brief, IEnumerable<string> FileIds) FormatInput(string html, int length = 50)
        {
            if (string.IsNullOrEmpty(html)) return (null, string.Empty, null);

            List<string> fileIds = new List<string>();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            string brief = string.Empty;
            if (length > 0)
            {
                string innerText = doc.DocumentNode.InnerText;
                //brief = doc.Text.Substr(0, length);
                brief = innerText.Substr(0, length);
            }

            ParseInput(doc.DocumentNode, fileIds);

            if (fileIds.Count > 50) throw new UserUIException("文本编辑器里上传的文件数量不能超过50个");

            return (doc.DocumentNode.OuterHtml, brief, fileIds);
        }

        private static void ParseInput(HtmlNode node, List<string> fileIds)
        {
            for (var i = 0; i < node.ChildNodes.Count; i++)
            {
                var child = node.ChildNodes[i];
                if (child.Name == "img" && child.GetAttributeValue("data-upload", string.Empty) == "true")
                {
                    var src = child.Attributes["src"].Value;
                    if (src.IndexOf(_host) > -1)
                    {
                        //重写地址
                        child.Attributes["src"].Value = src.Replace(_host, "[imageHost]");
                    }

                    var fileId = child.GetAttributeValue("data-fileId", string.Empty);
                    if (!string.IsNullOrEmpty(fileId))
                    {
                        if (!fileIds.Contains(fileId))
                            fileIds.Add(fileId);
                    }

                }
                ParseInput(child, fileIds);
            }

        }

        public static string FormatOutput(string html)
        {
            return FormatOutput(html, null);
        }

        /// <summary>
        /// 将格式化的内容转换为可以在页面上呈现的html代码
        /// </summary>
        /// <returns></returns>
        public static string FormatOutput(string html, Action<HtmlNode> action)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            ParseOutput(doc.DocumentNode, action);
            return doc.DocumentNode.OuterHtml;
        }

        private static void ParseOutput(HtmlNode node, Action<HtmlNode> action)
        {
            for (var i = 0; i < node.ChildNodes.Count; i++)
            {
                var child = node.ChildNodes[i];
                if (child.Name == "img" && child.GetAttributeValue("data-upload", string.Empty) == "true")
                {
                    var src = child.Attributes["src"].Value;
                    if (src.IndexOf("[imageHost]") > -1)
                    {
                        //重写地址
                        child.Attributes["src"].Value = src.Replace("[imageHost]", _host);
                    }
                }

                if(child.NodeType== HtmlNodeType.Element)
                {
                    child.AddClass($"c-{child.Name}");
                }

                if (action != null) action(child);
                ParseOutput(child, action);
            }
        }
    }
}
