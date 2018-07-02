using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Util;

using HtmlAgilityPack;

namespace Module.WebUI
{
    /// <summary>
    /// 
    /// </summary>
    public static class EditorUtil
    {
        /// <summary>
        /// 获取文本编辑器提交到服务端存放的内容信息
        /// </summary>
        /// <param name="editor"></param>
        /// <returns></returns>
        public static InputContent GetInput(string source)
        {
            InputContent info = new InputContent() { OriginalCode = source, ImageKeys = new Stack<string>() };
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(source);
            ParseInput(doc.DocumentNode, info);
            info.Code = doc.DocumentNode.OuterHtml;
            return info;
        }

        /// <summary>
        /// 获取文本编辑器
        /// </summary>
        /// <returns></returns>
        public static OutputInfo GetOutput(string source)
        {
            OutputInfo info = new OutputInfo() { OriginalCode = source };

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(source);
            ParseOutput(doc.DocumentNode, info);
            info.Code = doc.DocumentNode.OuterHtml;
            return info;
        }



        public struct InputContent
        {
            /// <summary>
            /// 原始代码
            /// </summary>
            public string OriginalCode { get; internal set; }

            /// <summary>
            /// 上传的图片的key值
            /// </summary>
            public Stack<string> ImageKeys { get; internal set; }

            /// <summary>
            /// 处理后的代码
            /// </summary>
            public string Code { get; internal set; }
        }

        #region 输入

        private static void ParseInput(HtmlNode node, InputContent info)
        {
            for (var i = 0; i < node.ChildNodes.Count; i++)
            {
                var child = node.ChildNodes[i];
                if (child.Name == "img")
                {
                    var src = child.Attributes["src"].Value;
                    var imageHost = DomainUtil.GetDomain("image");
                    if (src.IndexOf(imageHost) > -1)
                    {
                        //提取key
                        info.ImageKeys.Push(ImageUtil.ParseKey(src));
                        //重写地址
                        child.Attributes["src"].Value = src.Replace(imageHost, "[imageHost]");

                    }
                    //(?<=http://)[\w\.]+[^/]
                }
                ParseInput(child, info);
            }

        }

        #endregion

        #region 输出

        public struct OutputInfo
        {
            /// <summary>
            /// 原始文本
            /// </summary>
            public string OriginalCode { get; internal set; }

            /// <summary>
            /// 解析后的结果文本
            /// </summary>
            public string Code { get; internal set; }
        }

        private static void ParseOutput(HtmlNode node, OutputInfo info)
        {
            for (var i = 0; i < node.ChildNodes.Count; i++)
            {
                var child = node.ChildNodes[i];
                if (child.Name == "img")
                {
                    var src = child.Attributes["src"].Value;
                    if (src.IndexOf("[imageHost]") > -1)
                    {
                        var imageHost = DomainUtil.GetDomain("image");
                        //重写地址
                        child.Attributes["src"].Value = src.Replace("[imageHost]", imageHost);
                    }
                }
                ParseOutput(child, info);
            }
        }

        #endregion


    }
}
