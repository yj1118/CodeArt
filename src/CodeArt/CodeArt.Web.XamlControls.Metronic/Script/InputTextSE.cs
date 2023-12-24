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
    /// input脚本元素
    /// </summary>
    public class InputTextSE : ScriptElement
    {
        public InputTextSE() { }

        private string _value;

        public string Value
        {
            get
            {
                if (_value == null) _value = this.Metadata.GetValue<string>("value", string.Empty);
                return _value;
            }
        }


        #region 发射命令

        /// <summary>
        /// 设置值
        /// </summary>
        public void SetValue(string value)
        {
            this.View.WriteCode(string.Format("{0}.proxy().set('{1}');", this.Id, value));
        }

        #endregion


    }
}
