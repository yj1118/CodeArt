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
    public class InputListSE : ScriptElement
    {
        public InputListSE() { }

        private DTObject[] _value;

        public DTObject[] Value
        {
            get
            {
                if (_value == null) _value = this.Metadata.GetList("value").ToArray();

                return _value;
            }
        }

        #region 发射命令

        /// <summary>
        /// 设置值
        /// </summary>
        public void SetValue(DTObject[] value)
        {
            DTObjects list = new DTObjects(value);
            this.View.WriteCode(string.Format("{0}.proxy().set({1});", this.Id, list.GetCode(false)));
        }

        public void SetValue(DTObjects value)
        {
            this.View.WriteCode(string.Format("{0}.proxy().set({1});", this.Id, value.GetCode(false)));
        }

        #endregion


    }
}
