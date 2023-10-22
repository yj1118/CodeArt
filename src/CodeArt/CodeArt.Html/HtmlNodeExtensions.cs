using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

using HtmlAgilityPack;

namespace CodeArt.HtmlWrapper
{
    public static class HtmlNodeExtensions
    {
        public static HtmlNodeCollection SelectNodesEx(this HtmlNode node, string xpath)
        {
            HtmlNodeCollection nodes = new HtmlNodeCollection(null);
            XPathNodeIterator iterator = new HtmlNodeNavigator(node.OwnerDocument, node).Select(xpath);
            while (iterator.MoveNext())
            {
                HtmlNodeNavigator current = (HtmlNodeNavigator)iterator.Current;
                nodes.Add(current.CurrentNode);
            }
            return nodes;
        }

        public static HtmlNode SelectSingleNodeEx(this HtmlNode node, string xpath)
        {
            if (xpath == null)
            {
                throw new ArgumentNullException("xpath");
            }
            XPathNodeIterator iterator = new HtmlNodeNavigator(node.OwnerDocument, node).Select(xpath);
            if (!iterator.MoveNext())
            {
                return null;
            }
            HtmlNodeNavigator current = (HtmlNodeNavigator)iterator.Current;
            return current.CurrentNode;
        }

        /// <summary>
        /// 在子节点集合中，自动过滤注释节点、空白节点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="ignoreBlank">指示是否忽略空白节点</param>
        /// <returns></returns>
        public static IList<HtmlNode> TrimChildNodes(this HtmlNode node, bool ignoreBlank)
        {
            var childs = node.ChildNodes;
            List<HtmlNode> nodes = new List<HtmlNode>(childs.Count);
            nodes.AddRange(childs);

            //移除前导空白符、(注释节点)
            for (var i = 0; i < nodes.Count; i++)
            {
                var temp = nodes[i];
                if ((temp.NodeType == HtmlNodeType.Comment && !temp.IsConditionalComment())
                    || (ignoreBlank && temp.IsBlank()))
                {
                    nodes.RemoveAt(i);
                    i--;
                }
            }
            return nodes;
        }

        /// <summary>
        /// 是否为条件注释
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool IsConditionalComment(this HtmlNode node)
        {
            var content = node.OuterHtml;
            return content.IndexOf("[if") > -1 || content.IndexOf("[endif]") > -1;
        }

        /// <summary>
        /// 是否为空白节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool IsBlank(this HtmlNode node)
        {
            return (node.NodeType == HtmlNodeType.Text && string.IsNullOrEmpty(node.InnerText.Trim()));
        }

        /// <summary>
        /// 删除节点上所有的属性
        /// </summary>
        /// <param name="node"></param>
        public static void RemoveAttributes(this HtmlNode node)
        {
            HtmlAttributeCollection attrs = node.Attributes;
            for (var i = 0; i < attrs.Count; i++)
            {
                node.Attributes.Remove(attrs[i]);
            }
        }

        public static void RemoveAttribute(this HtmlNode node,string attrName)
        {
            HtmlAttributeCollection attrs = node.Attributes;
            for (var i = 0; i < attrs.Count; i++)
            {
                var attr = attrs[i];
                if (string.Equals(attr.Name, attrName, StringComparison.OrdinalIgnoreCase))
                {
                    node.Attributes.Remove(attr);
                    break;
                }
            }
        }

        /// <summary>
        /// 不带命名空间的节点名称
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string LocalName(this HtmlNode node)
        {
            if (node.Name.Length == 0) return string.Empty;
            var p = node.Name.IndexOf(":");
            if (p == -1) return node.Name;
            return node.Name.Substring(p + 1);
        }

        public static string LocalOriginalName(this HtmlNode node)
        {
            if (node.OriginalName.Length == 0) return string.Empty;
            var p = node.OriginalName.IndexOf(":");
            if (p == -1) return node.OriginalName;
            return node.OriginalName.Substring(p + 1);
        }

        /// <summary>
        /// 获取命名空间的名称
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string PrefixName(this HtmlNode node)
        {
            if (node.Name.Length == 0) return string.Empty;
            var p = node.Name.IndexOf(":");
            if (p == -1) return string.Empty;
            return node.Name.Substring(0, p);
        }

        public static HtmlNodeCollection ChildElements(this HtmlNode node)
        {
            var childNodes = node.ChildNodes;
            HtmlNodeCollection elems = new HtmlNodeCollection(node);
            foreach (var child in childNodes)
            {
                if (child.NodeType == HtmlNodeType.Element) elems.Add(child);
            }
            return elems;
        }

        public static void InsertAfter(this HtmlNode node, HtmlNode newNode)
        {
            node.ParentNode.InsertAfter(newNode, node);
        }

        public static void InsertBefore(this HtmlNode node, HtmlNode newNode)
        {
            node.ParentNode.InsertBefore(newNode, node);
        }

        public static int NodeIndex(this HtmlNode node)
        {
            var parent = node.ParentNode;
            if (parent == null) return 0;
            var childs = parent.ChildNodes;
            for (var i = 0; i < childs.Count; i++)
            {
                if (childs[i] == node) return i;
            }
            return -1;
        }

        public static string GetFrontCode(this HtmlNode node)
        {
            var html = node.OuterHtml;
            int pos = html.IndexOf(">");
            var code = html.Substring(0, pos + 1);
            if (node.IsSingleNode() && !code.EndsWith("/>"))
            {
                code = code.Insert(code.Length - 1, "/");
            }
            return code;
        }

        public static string GetBehindCode(this HtmlNode node)
        {
            if (node.IsSingleNode()) return string.Empty;
            var html = node.OuterHtml;
            int pos = html.LastIndexOf("</");
            return pos == -1 ? string.Empty : html.Substring(pos);
        }

        public static bool IsSingleNode(this HtmlNode node)
        {
            return !node.OuterHtml.EndsWith(string.Format("</{0}>", node.OriginalName));
        }

    }
}