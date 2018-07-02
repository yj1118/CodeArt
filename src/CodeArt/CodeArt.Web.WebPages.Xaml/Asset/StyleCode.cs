using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 脚本形式的代码基类
    /// </summary>
    public abstract class ScriptCode : CodeAsset
    {
        protected override string GetCode()
        {
            StringBuilder code = new StringBuilder();
            code.AppendLine("<script>");
            FillCode(code);
            code.AppendLine();
            code.AppendLine("</script>");
            return code.ToString();
        }

        protected abstract void FillCode(StringBuilder code);

    }
}
