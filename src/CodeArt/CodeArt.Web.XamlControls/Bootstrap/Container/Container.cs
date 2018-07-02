using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.XamlControls.Bootstrap
{
    [ComponentLoader(typeof(ContainerLoader))]
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Bootstrap.Container.Template.html,CodeArt.Web.XamlControls")]
    public class Container : ContentControl
    {

    }
}
