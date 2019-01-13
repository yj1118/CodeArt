using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.WebPages;
using CodeArt.Web.WebPages.Xaml.Script;

namespace CodeArt.Web.XamlControls.Metronic
{
    /// <summary>
    /// 
    /// </summary>
    public static class Menu
    {
        public static void MenuRedirect(this ScriptView view,string url)
        {
            if(!url.StartsWith("/"))
            {
                //不是从根目录开始的，要转换
                url = PageUtil.MapVirtualPath(WebPageContext.Current.VirtualPath, url);
            }
            view.WriteCode(string.Format("$$.page.menu.view.redirect = {0};", JSON.GetCode(url)));
        }
    }
}
