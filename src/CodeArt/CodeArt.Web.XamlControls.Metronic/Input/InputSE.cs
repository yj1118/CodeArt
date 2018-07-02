using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.WebPages.Xaml.Script;


namespace CodeArt.Web.XamlControls.Metronic
{
    public class InputSE : ScriptElement
    {
        public InputSE() { }

        #region 发射命令

        /// <summary>
        /// 绑定数据
        /// </summary>
        /// <param name="data"></param>
        public void Set(object value)
        {
            this.View.WriteCode(string.Format("{0}.proxy().set({1});", this.Id, JSON.GetCode(value)));
        }


        #endregion


    }
}
