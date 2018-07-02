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
    public class InputTagsSE : ScriptElement
    {
        public InputTagsSE() { }

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
        public void SetValue(DTObject value)
        {
            //由于tag是下拉框，数据必须使用value,text格式，否则无效
            if (value["value"] != null && value["text"] != null)
            {
                this.View.WriteCode("var ds = [];ds.push({ value:\"" + value["value"].ToString() + "\", text:\"" + value["text"].ToString() + "\"});" + this.Id + ".proxy().set(ds[0]);");
            }
        }

        public void AddValue(DTObject[] datas)
        {
            StringBuilder code = new StringBuilder();

            code.Append("var ds = [];");

            foreach (DTObject d in datas)
            {
                //由于tag是下拉框，数据必须使用value,text格式，否则无效
                if (d["value"] != null && d["text"] != null)
                {
                    code.Append("ds.push({");
                    code.Append("value:\"" + d["value"].ToString() + "\", text:\"" + d["text"].ToString() + "\"");
                    code.Append("}");
                    code.Append(");");
                }
            }

            code.AppendFormat("{0}.proxy().add(ds);", this.Id);

            this.View.WriteCode(code.ToString());
        }

        #endregion


    }
}
