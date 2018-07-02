using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Runtime;
using CodeArt.Concurrent;

namespace CodeArt.Web.XamlControls.Metronic
{
    [SafeAccess]
    public class InputCoreTemplateCodeFactory : ITemplateCodeFactory
    {
        public string GetTemplateCode(object obj, string templatePropertyName)
        {
            var core = obj as InputCore;
            var type = core.Type;
            return AssemblyResource.LoadText(string.Format("CodeArt.Web.XamlControls.Metronic.Input.InputCore.{0}Template.html,CodeArt.Web.XamlControls.Metronic", type.FirstToUpper()));
        }
    }
}
