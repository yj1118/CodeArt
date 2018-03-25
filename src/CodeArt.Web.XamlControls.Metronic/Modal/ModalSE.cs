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
    /// 表单脚本元素
    /// </summary>
    public class ModalSE : ScriptElement
    {
        public ModalSE() { }

        #region 发射命令

        public void Open()
        {
            this.View.WriteCode(string.Format("{0}.proxy().open();", this.Id));
        }

        public void Close()
        {
            this.View.WriteCode(string.Format("{0}.proxy().close();", this.Id));
        }

        public void SetTitle(string title)
        {
            this.View.WriteCode(string.Format("{0}.proxy().title({1});", this.Id, JSON.GetCode(title)));
        }

        #endregion


    }
}
