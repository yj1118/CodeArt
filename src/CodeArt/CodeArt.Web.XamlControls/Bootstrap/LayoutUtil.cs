using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

using HtmlAgilityPack;
using CodeArt.Web.WebPages.Xaml;

namespace CodeArt.Web.XamlControls.Bootstrap
{
    public static class LayoutUtil
    {
        private static string[] _widths = { "xs", "sm", "md", "lg" }; //顺序不能变
        private static string[] _offsets = { "xs-offset", "sm-offset", "md-offset", "lg-offset" }; //顺序不能变
        private static string[] _pushs = { "xs-push", "sm-push", "md-push", "lg-push" };
        private static string[] _pulls = { "xs-pull", "sm-pull", "md-pull", "lg-pull" };
        private static string[] _all = null;


        static LayoutUtil()
        {
            var temp = new List<string>();
            temp.AddRange(_widths);
            temp.AddRange(_offsets);
            temp.AddRange(_pushs);
            temp.AddRange(_pushs);
            _all = temp.ToArray();
        }


        public static string GetClassName(HtmlNode node, string currentClassName = null)
        {
            StringBuilder code = new StringBuilder();
            foreach (var widthName in _widths)
            {
                var width = node.GetAttributeValue(widthName, string.Empty);
                if (!string.IsNullOrEmpty(width))
                    code.AppendFormat("col-{0}-{1} ", widthName, width);
            }
            if (IsSetOffset(node))
            {
                foreach (var offsetName in _offsets)
                {
                    var offset = node.GetAttributeValue(offsetName, string.Empty);
                    //if (string.IsNullOrEmpty(offset)) offset = "0";
                    //code.AppendFormat("col-{0}-{1} ", offsetName, offset);
                    if (!string.IsNullOrEmpty(offset))
                        code.AppendFormat("col-{0}-{1} ", offsetName, offset);
                }
            }

            if (IsSetOrder(node))
            {
                for (var i = 0; i < _pushs.Length; i++)
                {
                    var pushName = _pushs[i];
                    var pullName = _pulls[i];

                    var push = node.GetAttributeValue(pushName, string.Empty);
                    var pull = node.GetAttributeValue(pullName, string.Empty);
                    if (string.IsNullOrEmpty(push) && string.IsNullOrEmpty(pull)) continue; //没有设置任何一个，所以不追加代码

                    if (string.IsNullOrEmpty(push)) push = "0";
                    if (string.IsNullOrEmpty(pull)) pull = "0";

                    code.AppendFormat("col-{0}-{1} col-{2}-{3} ", pushName, push, pullName, pull);
                }
            }
            string layoutClassName = code.ToString().Trim();
            if (string.IsNullOrEmpty(currentClassName)) return layoutClassName;
            return string.Format("{0} {1}", layoutClassName, currentClassName).Trim();
        }

        /// <summary>
        /// 设置了偏移量
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static bool IsSetOffset(HtmlNode node)
        {
            foreach (var offsetName in _offsets)
            {
                var offset = node.GetAttributeValue(offsetName, string.Empty);
                if (!string.IsNullOrEmpty(offset)) return true;
            }
            return false;
        }

        /// <summary>
        /// 设置了排序
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static bool IsSetOrder(HtmlNode node)
        {
            for (var i = 0; i < _pushs.Length; i++)
            {
                var pushName = _pushs[i];
                var pullName = _pulls[i];

                var push = node.GetAttributeValue(pushName, string.Empty);
                if (!string.IsNullOrEmpty(push)) return true;

                var pull = node.GetAttributeValue(pullName, string.Empty);
                if (!string.IsNullOrEmpty(pull)) return true;
            }
            return false;
        }

        /// <summary>
        /// 获取布局的属性字符串
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string GetProperiesCode(HtmlNode node)
        {
            return UIUtil.GetProperiesCode(node, _all);
        }

    }
}
