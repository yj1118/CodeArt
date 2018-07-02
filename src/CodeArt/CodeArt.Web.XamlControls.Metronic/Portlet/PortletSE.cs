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
    public class PortletSE : ScriptElement
    {
        public PortletSE() { }

        public void Title(string title, string subtitle = null)
        {
            if (string.IsNullOrEmpty(subtitle))
            {
                this.View.WriteCode(string.Format("{0}.proxy().title({1});", this.Id, JSON.GetCode(title)));
            }
            else
            {
                this.View.WriteCode(string.Format("{0}.proxy().title({1},{2});", this.Id, JSON.GetCode(title), JSON.GetCode(subtitle)));
            }
        }
    }
}
