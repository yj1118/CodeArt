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
    /// 提示框脚本元素
    /// </summary>
    public static class SweetAlert
    {
        public static void SweetAlertWarning(this ScriptView view,string title, string text, Action ok = null)
        {
            view.WriteCode(string.Format("$$metronic.sweetAlert.warning({0},{1},function()", JSON.GetCode(title), JSON.GetCode(text)));
            view.WriteCode("{");
            if (ok != null)
            {
                ok();
            }
            view.WriteCode("});");
        }

        public static void SweetAlertWarning(this ScriptView view, string title, Action ok = null)
        {
            SweetAlertWarning(view, title, string.Empty, ok);
        }

        public static void SweetAlertSucess(this ScriptView view, string title, string text)
        {
            view.WriteCode(string.Format("$$metronic.sweetAlert.success({0},{1});", JSON.GetCode(title), JSON.GetCode(text)));
        }

        public static void SweetAlertSucess(this ScriptView view, string title)
        {
            SweetAlertSucess(view, title, string.Empty);
        }


        public static void SweetAlertConfirm(this ScriptView view, string title, string text, Action ifTrue, Action ifFalse = null)
        {
            view.WriteCode(string.Format("$$metronic.sweetAlert.confirm({0},{1},function()", JSON.GetCode(title), JSON.GetCode(text)));
            view.WriteCode("{");
            ifTrue();
            view.WriteCode("}");
            if (ifFalse != null)
            {
                view.WriteCode(",function(){");
                ifFalse();
                view.WriteCode("}");
            }
            view.WriteCode(");");
        }

        public static void SweetAlertConfirm(this ScriptView view, string title, Action ifTrue, Action ifFalse = null)
        {
            SweetAlertConfirm(view, title, string.Empty, ifTrue, ifFalse);
        }

        /// <summary>
        /// 在删除数据之前做提示的视图事件
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        public static Action<ViewEventProcessor> GetDeleteConfirmEvent()
        {
            return _deleteConfirmEvent;
        }

        private static Action<ViewEventProcessor> _deleteConfirmEvent = (p) =>
        {
            p.On(ViewEvent.Validate, (view) =>
            {
                view.SweetAlertConfirm(Strings.AreYouSureDelete, () =>
                {
                    view.Submit(true);
                });

                view.Return(false);
            });
        };

        public static Action<ViewEventProcessor> GetConfirmEvent(string message)
        {
            return (p) => {
                p.On(ViewEvent.Validate, (view) =>
                {
                    view.SweetAlertConfirm(message, () =>
                    {
                        view.Submit(true);
                    });

                    view.Return(false);
                });
            };
        }

    }
}
