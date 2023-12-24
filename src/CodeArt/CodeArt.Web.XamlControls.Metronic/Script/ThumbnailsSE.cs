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
    public class ThumbnailsSE : ScriptElement
    {
        public ThumbnailsSE() { }

        //private DateTime _value = DateTime.MinValue;

        //public DateTime Value
        //{
        //    get
        //    {
        //        if (_value == DateTime.MinValue)
        //            _value = this.Metadata.GetValue<DateTime>("value", DateTime.MinValue);
        //        return _value;
        //    }
        //}


        #region 发射命令

        /// <summary>
        /// 设置值
        /// </summary>
        public void SetConfig(DTObjects config)
        {
            this.View.WriteCode(string.Format("{0}.proxy().setConfig({1});", this.Id, config.GetCode(false)));
        }

        #endregion


    }
}
