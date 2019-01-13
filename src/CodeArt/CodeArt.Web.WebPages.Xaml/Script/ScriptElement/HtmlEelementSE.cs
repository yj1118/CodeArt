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
    }
}
