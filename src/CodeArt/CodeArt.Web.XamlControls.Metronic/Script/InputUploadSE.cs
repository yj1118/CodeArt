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
    public class InputUploadSE : ScriptElement
    {
        public InputUploadSE() { }

        //private string _value;

        //public string Value
        //{
        //    get
        //    {
        //        if (_value == null) _value = this.Metadata.GetValue<string>("value", string.Empty);
        //        return _value;
        //    }
        //}


        #region 发射命令

        /// <summary>
        /// 设置目录编号（文件会被上传至指定编号的虚拟目录中）
        /// </summary>
        public void SetFolderID(string folderId)
        {
            this.View.WriteCode(string.Format("{0}.proxy().folderId('{1}');", this.Id, folderId));
        }

        public void SetValue(DTObjects value)
        {
            this.View.WriteCode(string.Format("{0}.proxy().set({1});", this.Id, value.GetCode(false)));
        }

        #endregion


    }
}
