using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace CodeArt.Web.WebPages.Xaml.Script
{
    /// <summary>
    /// 常规的html元素
    /// </summary>
    public class HtmlEelementSE : ScriptElement
    {
        public HtmlEelementSE() { }

        private object _value = null;

        /// <summary>
        /// 通用的获取value的方法
        /// </summary>
        public object Value
        {
            get
            {
                if (_value == null)
                {
                    _value = this.Metadata.GetValue("value", null);
                    if (_value == null) _value = this.Metadata.GetObject("value", null);
                }
                return _value;
            }
        }

        private dynamic _data = null;

        public dynamic Data
        {
            get
            {
                if (_data == null)
                {
                    _data = this.Metadata.GetObject("data", null) ?? this.Metadata.GetObject("value");
                }
                return _data;
            }
        }

        public void Clear()
        {
            this.View.WriteCode(string.Format("{0}.proxy().clear();", this.Id));
        }

        public void Bind(DTObject data)
        {
            this.View.WriteCode(string.Format("{0}.proxy().bind({1});", this.Id, data.GetCode(false, false)));
        }

        public void Bind(string expression)
        {
            this.View.WriteCode(string.Format("{0}.proxy().bind({1});", this.Id, expression));
        }

        public void Hide()
        {
            this.View.WriteCode(string.Format("{0}.hide();", this.Id));
        }
        public void Show()
        {
            this.View.WriteCode(string.Format("{0}.show();", this.Id));
        }

    }
}
