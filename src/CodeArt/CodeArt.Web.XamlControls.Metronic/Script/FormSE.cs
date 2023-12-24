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
    /// 表单脚本元素的扩展
    /// </summary>
    public static class FormSEExtension
    {
        public static void Alert(this FormSE form, string message, AlertSE.Type type, bool closable, int closeInSeconds, string icon)
        {
            form.View.WriteCode(AlertSE.GetAlertCode(form.Id, message, type, closable, closeInSeconds, icon));
        }

        public static void Alert(this FormSE form, string message, AlertSE.Type type, bool closable, int closeInSeconds)
        {
            form.View.WriteCode(AlertSE.GetAlertCode(form.Id, message, type, closable, closeInSeconds, string.Empty));
        }

        public static void Alert(this FormSE form, string message, AlertSE.Type type, bool closable)
        {
            form.View.WriteCode(AlertSE.GetAlertCode(form.Id, message, type, closable, 0, string.Empty));
        }

    }
}
