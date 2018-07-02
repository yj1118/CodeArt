using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.Concurrent;
using CodeArt.DTO;
using CodeArt.Web.WebPages.Xaml.Script;


namespace CodeArt.Web.XamlControls.Metronic
{
    public class SelectSE : ScriptElement
    {
        public SelectSE() { }

        /// <summary>
        /// 绑定选项，请确保<paramref name="data"/>里有成员为rows的集合作为绑定项
        /// </summary>
        /// <param name="data"></param>
        public void Options(DTObject data)
        {
            using (var temp = StringPool.Borrow())
            {
                var code = temp.Item;

                code.AppendFormat("{0}.proxy().options({1}.rows);", this.Id, data.GetCode(false, false));

                this.View.WriteCode(code.ToString());
            }
        }
    }
}
