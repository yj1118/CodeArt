using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.XamlControls.Metronic;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.Concurrent;
using CodeArt.Web.WebPages.Xaml.Script;

namespace CodeArt.Web.XamlControls.Metronic
{
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Portlet.PortletSlim.html,CodeArt.Web.XamlControls.Metronic")]
    public class PortletSlim : PortletBase
    {
        public PortletSlim()
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        static PortletSlim()
        { }
    }
}