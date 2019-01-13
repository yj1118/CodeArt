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
    /// 提示框脚本元素
    /// </summary>
    public static class ViewExtensions
    {
        public static void SetUserBadge(this ScriptView view, int count)
        {
            view.WriteCode(string.Format("$$.page.setUserBadge({0});", count));
        }

        public static void SetMenuBadge(this ScriptView view, string menuItemId, int count)
        {
            view.WriteCode(string.Format("$$.page.menu.setBadge('{0}',{1});", menuItemId, count));
        }

        public static void Info(this ScriptView view, string title, string message = null)
        {
            Notice(view, "info", title, message);
        }

        public static void Success(this ScriptView view, string title, string message = null)
        {
            Notice(view, "success", title, message);
        }

        public static void Warning(this ScriptView view, string title, string message = null)
        {
            Notice(view, "warning", title, message);
        }

        public static void Error(this ScriptView view, string type, string title, string message = null)
        {
            Notice(view, "error", title, message);
        }

        private static void Notice(this ScriptView view, string type,string title,string message)
        {
            using (var temp = StringPool.Borrow())
            {
                var code = temp.Item;
                code.Append("$$.page.notice({");
                code.AppendFormat("type:'{0}',",type);
                
                if(string.IsNullOrEmpty(message))
                {
                    code.AppendFormat("title:{0}", JSON.GetCode(title));
                }
                else
                {
                    code.AppendFormat("title:{0},", JSON.GetCode(title));
                    code.AppendFormat("message:{0}", JSON.GetCode(message));
                }
                code.Append("});");
                view.WriteCode(code.ToString());
            }
        }
    }
}
