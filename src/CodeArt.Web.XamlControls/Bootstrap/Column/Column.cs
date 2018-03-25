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
    [ComponentLoader(typeof(ColumnLoader))]
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Bootstrap.Column.Template.html,CodeArt.Web.XamlControls")]
    public class Column : ContentControl
    {

    }
}
