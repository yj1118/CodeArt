using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.Web.WebPages.Xaml.Script;


namespace CodeArt.Web.XamlControls.Metronic
{
    [ComponentLoader(typeof(DynamicInputLoader))]
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.DynamicInput.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class DynamicInput : Control
    {
    }
}
