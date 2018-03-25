using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Util;

namespace CodeArt.Web.WebPages.Xaml.Script
{
    /// <summary>
    /// 表单脚本元素
    /// </summary>
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

    }
}
