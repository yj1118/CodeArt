using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web.Compilation;

using System.Web;
using System.IO;

using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.AOP;
using CodeArt.DTO;

namespace CodeArt.Web.WebPages
{
    /// <summary>
    /// 基于文本的页面
    /// </summary>
    public abstract class WebText : WebPage
    {
        protected WebText()
        {
            
        }

        protected override Stream LoadContent()
        {
            string error = null;
            if (this.IsError)
            {
                error = this.GetQueryValue<string>("error", string.Empty);
                error = error.Replace("\r\n","<br/>");
            }

            string text = GetTextContent(error);
            return CompressCode(text);
        }

        protected abstract string GetTextContent(string error);

        #region 压缩代码

        /// <summary>
        /// 压缩代码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        protected virtual Stream CompressCode(string code)
        {
            if (code == null) return null;
            //压缩
            byte[] codeBytes = Encoding.UTF8.GetBytes(code);
            MemoryStream source = new MemoryStream(codeBytes);

            var context = this.PageContext;

            ICompressor cpor = CompressorFactory.Create(context);
            if (cpor.IsAccepted(context))
            {
                //支持压缩
                MemoryStream target = new MemoryStream(codeBytes.Length);
                cpor.Compress(context, source, target);
                target.Position = 0;
                source = target;
            }
            return source;
        }

        #endregion

        /// <summary>
        /// 处理POST请求
        /// </summary>
        protected override void ProcessPOST()
        {
            var extractor = GetParameterExtractor();
            var args = extractor.ParseArguments(this.PageContext);//提取方法参数

            var result = CallWebMethod(args);

            if (result != null)
            {
                IResultSender sender = GetResultSender();
                sender.Send(this.PageContext, result);
            }
        }

        protected abstract string CallWebMethod(DTObject args);


        protected virtual IParameterExtractor GetParameterExtractor()
        {
            return WebMethodSupporterFactory.CreateExtractor();
        }

        protected IResultSender GetResultSender()
        {
            return WebMethodSupporterFactory.CreateSender();
        }



    }
}
