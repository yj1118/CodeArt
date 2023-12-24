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

        public void Disable(bool diabled)
        {
            //disable
            this.View.WriteCode(string.Format("{0}.proxy().disable({0});", this.Id, JSON.GetCode(diabled)));
        }

        public void Help(string message)
        {
            this.View.WriteCode(string.Format("{0}.proxy().help({1});", this.Id, JSON.GetCode(message)));
        }

        #endregion


    }
}
