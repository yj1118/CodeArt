using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace CodeArt.Web.WebPages.Static
{
    /// <summary>
    /// 
    /// </summary>
    internal static class StaticParser
    {
        public static (string Code,bool Parsed) Parse(string virtualPath, string code)
        {
            if (string.IsNullOrEmpty(code)) return (string.Empty, false);

            const string header = "/*<!DOCTYPE static>*/";
            if (!code.StartsWith(header)) return (code, false);  //如果没有标记头，那么不解析
            code = code.Substring(header.Length);
            code = ReplacerSection.Instance.Execute(virtualPath, code);
            return (code, true);
        }

    }


}
