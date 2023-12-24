using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.Concurrent;
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


        public static void PageBarAddItems(this ScriptView view, IEnumerable<PageBarItem> items)
        {
            view.WriteCode("$$.page.bar.addItems([");
            using (var temp = StringPool.Borrow())
            {
                var code = temp.Item;
                foreach (var item in items)
                {
                    code.Append("{");
                    code.AppendFormat("text:{0},url:{1}", JSON.GetCode(item.Text), JSON.GetCode(item.Url));
                    code.Append("},");
                }
                if (code.Length > 0) code.Length--;
                view.WriteCode(code.ToString());
            }
                
            view.WriteCode("]);");
        }

        public static void PageBarAddItems(this ScriptView view, DTObjects items)
        {
            view.WriteCode(string.Format("$$.page.bar.addItems({0});", items.GetCode(false)));
        }

    }


    public struct PageBarItem
    {
        public string Text
        {
            get;
            private set;
        }

        public string Url
        {
            get;
            private set;
        }

        public PageBarItem(string text,string url)
        {
            this.Text = text;
            this.Url = url;
        }
    }

}
