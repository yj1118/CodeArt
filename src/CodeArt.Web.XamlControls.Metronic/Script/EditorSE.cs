using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.HtmlWrapper;

using HtmlAgilityPack;

namespace CodeArt.Web.XamlControls.Metronic
{
    /// <summary>
    /// 
    /// </summary>
    public class EditorSE : ScriptElement
    {
        public EditorSE() { }

        private string _value;
       
        public string Value
        {
            get
            {
                if (string.IsNullOrEmpty(_value))
                {
                    _value = this.Metadata.GetValue("value", string.Empty);
                }
                return _value;
            }
        }

        #region 发射命令

        /// <summary>
        /// 设置值
        /// </summary>
        public void SetValue(string value)
        {
            this.View.WriteCode(string.Format("{0}.proxy().set({1});", this.Id, JSON.GetCode(value)));
        }

        public void Reset()
        {
            this.View.WriteCode(string.Format("{0}.proxy().reset();", this.Id));
        }

        #endregion

    }
}
