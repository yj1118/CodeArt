using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CodeArt.Web.WebPages.Static
{
    internal abstract class ReplacerBase
    {
        protected ReplacerBase()
        {
        }

        protected abstract string GetPattern();

        protected abstract string GetReplaceText(string virtualPath, Match mc);

        public string Execute(string virtualPath, string text)
        {
            Regex reg = new Regex(GetPattern());
            MatchCollection mcs = reg.Matches(text);
            if (mcs.Count == 0) return text;

            int offset = 0;
            StringBuilder source = new StringBuilder(text);
            foreach (Match mc in mcs)
            {
                string content = GetReplaceText(virtualPath, mc);
                offset = Replace(offset, source, content, mc.Index, mc.Length);
            }
            return source.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="source"></param>
        /// <param name="newText">用于覆盖的文本</param>
        /// <param name="startIndex">被替换的区域的开始位置</param>
        /// <param name="length">被替换的区域的长度</param>
        /// <returns></returns>
        private static int Replace(int offset, StringBuilder source, string newText, int startIndex, int length)
        {
            int position = startIndex + offset;
            source.Remove(position, length);
            source.Insert(position, newText);
            offset += newText.Length - length;
            return offset;
        }


    }
}
