using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CodeArt.Web.WebPages.Static
{
    internal sealed class ReplacerSection : ReplacerBase
    {
        protected override string GetPattern()
        {
            return "/\\*section[ ]+src=\"(.*)?\"[ ]*\\*/";
        }

        protected override string GetReplaceText(string virtualPath, Match mc)
        {
            GroupCollection gs = mc.Groups;
            string src = gs[1].Value;
            string srcVirtualPath = PageUtil.MapVirtualPath(virtualPath, src);
            string code = PageUtil.GetRawCode(srcVirtualPath);
            return ContinueReplace(srcVirtualPath, code);
        }

        private string ContinueReplace(string virtualPath, string code)
        {
            return ReplacerSection.Instance.Execute(virtualPath, code);
        }

        public static readonly ReplacerSection Instance = new ReplacerSection();

    }
}
