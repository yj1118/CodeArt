using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// css样式形式的代码基类
    /// </summary>
    public abstract class StyleCode : CodeAsset
    {
        protected override string GetCode()
        {
            StringBuilder code = new StringBuilder();
            code.AppendLine("<style type=\"text/css\">");
            FillCode(code);
            code.AppendLine();
            code.AppendLine("</style>");
            return code.ToString();
        }

        protected abstract void FillCode(StringBuilder code);

    }
}
