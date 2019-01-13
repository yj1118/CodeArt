using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml.Controls;

namespace Module.WebUI.Xaml.Pages
{
    public class LogoutCode : ScriptCode
    {
        static LogoutCode()
        {
            
        }

        public LogoutCode()
        {
            this.Origin = DrawOrigin.Current;
        }
        
        protected override void FillCode(StringBuilder code)
        {
            var parent = this.GetTemplateParent() as Logout;

            code.AppendFormat("window.location.href = '{0}';",parent.ReturnUrl);
        }
    }
}