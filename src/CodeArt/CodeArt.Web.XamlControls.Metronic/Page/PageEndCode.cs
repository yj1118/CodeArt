using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml.Controls;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class PageEndCode : ScriptCode
    {
        static PageEndCode()
        {
            
        }

        public PageEndCode()
        {
            this.Origin = DrawOrigin.Current;
        }
        
        protected override void FillCode(StringBuilder code)
        {
            var parent = this.GetTemplateParent() as Page;

            var menuCode = parent.GetMenuCode();
            if (string.IsNullOrEmpty(menuCode)) menuCode = "{}";

            if (!string.IsNullOrEmpty(parent.MenuRedirect))
            {
                code.AppendFormat("$$.page.menu.view.redirect=\"{0}\";", parent.MenuRedirect);
                code.AppendLine();
            }

            code.AppendFormat("$$.page.menu.view.data={0};", menuCode);
            code.AppendLine();

            //code.Append("var pageTip={AssetsPath:\"" + parent.AssetsPath + "\"};");
        }
    }
}