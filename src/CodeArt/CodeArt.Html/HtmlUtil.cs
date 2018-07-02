using System;
using System.Collections.Generic;
using System.Runtime;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using CodeArt.Util;

using HtmlAgilityPack;

namespace CodeArt.HtmlWrapper
{
    public static class HtmlUtil
    {
        /// <summary>
        /// 提取纯文本
        /// </summary>
        public static string ExtractText(string html, bool filterWrap = false)
        {
            if (string.IsNullOrEmpty(html)) return string.Empty;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            ProcessBr(doc.DocumentNode);
            ProcessForTrim(doc.DocumentNode);
            
            var text = doc.DocumentNode.InnerText;
            text = HttpUtility.HtmlDecode(text);
            if (filterWrap)
            {
                Regex reg = new Regex("\r\n|\n");
                text = reg.Replace(text, string.Empty);
            }
            return text.Trim();
        }

        public static void ProcessBr(HtmlNode node)
        {
            var items = node.SelectNodesEx("//br");
            foreach (var item in items)
            {
                item.ParentNode.ReplaceChild(HtmlNode.CreateNode("\r\n"), item);
            }
        }

        public static void ProcessForTrim(HtmlNode node)
        {
            foreach (var child in node.ChildNodes)
            {
                ProcessForTrim(child);
                child.InnerHtml = TrimInnerHtmlSpace(child.InnerHtml);
            }
        }

        private static string TrimInnerHtmlSpace(string innerHtml)
        {
            while((innerHtml.IndexOf("&nbsp;") == 0) 
                || (innerHtml.IndexOf(" ") == 0) 
                || (innerHtml.IndexOf("　") == 0))
            {
                if (innerHtml.IndexOf("&nbsp;") == 0)
                {
                    innerHtml = innerHtml.Remove(0, 6);
                }
                else
                {
                    innerHtml = innerHtml.Remove(0, 1);
                }
            }

            return innerHtml;
        }


        /// <summary>
        /// 将纯文本字符串，转义为html格式
        /// </summary>
        public static string FormatHtml(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            Regex reg = new Regex("\r\n|\n");
            string[] segs = reg.Split(text);
            int lastIndex = segs.Length - 1;
            StringBuilder html = new StringBuilder();
            for (var i = 0; i < segs.Length; i++)
            {
                var seg = segs[i];
                html.AppendFormat("<p>{0}</p>", HttpUtility.HtmlEncode(seg));
                if (i < lastIndex) html.AppendLine();
            }
            return html.ToString();
        }
    }
}
