using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.WebPages.Xaml.Script;


namespace CodeArt.Web.XamlControls.Metronic
{
    public class FormSE : ScriptElement
    {
        public FormSE() { }

        private DTObject _data;

        public DTObject Data
        {
            get
            {
                if (_data == null)
                {
                    _data = this.Metadata.GetObject("data");
                }
                return _data;
            }
        }


        #region 发射命令

        public void AlertSuccess(string message)
        {
            this.View.WriteCode(string.Format("{0}.proxy().alertSuccess({1});", this.Id,JSON.GetCode(message)));
        }

        public void AlertDanger(string message)
        {
            this.View.WriteCode(string.Format("{0}.proxy().alertDanger({1});", this.Id, JSON.GetCode(message)));
        }

        /// <summary>
        /// 重置表单
        /// </summary>
        public void Reset()
        {
            this.View.WriteCode(string.Format("{0}.proxy().reset();", this.Id));
        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        /// <param name="data"></param>
        public void Set(DTObject data)
        {
            this.View.WriteCode(string.Format("{0}.proxy().set({1});", this.Id, data.GetCode(false, false)));
        }

        public void Disable(bool disabled)
        {
            this.View.WriteCode(string.Format("{0}.proxy().disable({1});", this.Id, disabled ? "true" : "false"));
        }

        #endregion


    }
}
