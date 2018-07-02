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
    public class ListSE : ScriptElement
    {
        public ListSE() { }


        public void Set(DTObjects values)
        {
            using (var temp = StringPool.Borrow())
            {
                var code = temp.Item;
                code.AppendFormat("{0}.proxy().set({1});", this.Id, values.GetCode(false));
                this.View.WriteCode(code.ToString());
            }
        }

        public void Reset()
        {
            using (var temp = StringPool.Borrow())
            {
                var code = temp.Item;
                code.AppendFormat("{0}.proxy().reset();", this.Id);
                this.View.WriteCode(code.ToString());
            }
        }

    }
}
