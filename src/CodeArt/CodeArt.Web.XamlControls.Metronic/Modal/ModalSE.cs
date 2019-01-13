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
    /// 
    /// </summary>
    public class ModalSE : ScriptElement
    {
        public ModalSE() { }

        #region 发射命令

        /// <summary>
        /// 设置场景组件，这意味着窗口内部的提交操作都会交由该组件处理
        /// </summary>
        /// <param name="componentName"></param>
        public void SetStageComponent(string componentName)
        {
            this.View.WriteCode(string.Format("{0}.attr('data-stage-component','{1}');", this.Id, componentName));
        }


        public void Open()
        {
            this.View.WriteCode(string.Format("{0}.proxy().open();", this.Id));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback">当模态窗完全显示后执行</param>
        public void Open(Action callback)
        {
            this.View.WriteCode(string.Format("{0}.proxy().open(", this.Id));
            this.View.WriteCode("function(){");
            callback();
            this.View.WriteCode("});");
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
