using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.WebPages.Xaml.Script;

namespace CodeArt.Web.XamlControls.Metronic
{
    /// <summary>
    /// 页面路径提示部分
    /// </summary>
    public static class PageBar
    {
        public static void PageBarTitle(this ScriptView view,string title)
        {
            view.WriteCode(string.Format("$$.page.bar.title({0});", JSON.GetCode(title)));
        }

     
    }
}
