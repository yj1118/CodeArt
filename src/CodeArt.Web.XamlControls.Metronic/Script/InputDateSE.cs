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
    public class InputDateSE : ScriptElement
    {
        public InputDateSE() { }

        private DateTime _value = DateTime.MinValue;

        public DateTime Value
        {
            get
            {
                if (_value == DateTime.MinValue)
                    _value = this.Metadata.GetValue<DateTime>("value", DateTime.MinValue);
                return _value;
            }
        }

        public bool IsValueEmpty
        {
            get
            {
                return this.Value == DateTime.MinValue;
            }
        }


        #region 发射命令

        /// <summary>
        /// 设置值
        /// </summary>
        public void SetValue(DateTime value)
        {
            this.View.WriteCode(string.Format("{0}.proxy().set({1});", this.Id, JSON.GetCode(value)));
        }

        #endregion


    }
}
