using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.DTO;
using CodeArt.Web.WebPages.Xaml.Script;

namespace CodeArt.Web.XamlControls.Bootstrap
{
    public static class Bootbox
    {
        public static void BootboxConfirm(this ScriptView view, string message, Action ifTrue, Action ifFalse = null)
        {
            view.WriteCode(string.Format("bootbox.confirm({0}, function (r) ", JSON.GetCode(message)));
            view.WriteCode("{");
            view.WriteCode("        if (r) {");
            ifTrue();
            view.WriteCode("        }");
            if (ifFalse != null)
            {
                view.WriteCode("        else{");
                ifFalse();
                view.WriteCode("        }");
            }
            view.WriteCode("    });");
        }

        public static void BootboxAlert(this ScriptView view, string message)
        {
            view.WriteCode("bootbox.alert({buttons:{ok:{label: '确定',}},");
            view.WriteCode(string.Format(" message: {0}", JSON.GetCode(message)));
            view.WriteCode("});");
        }

    }
}
