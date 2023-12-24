using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;

using CodeArt.Concurrent;

namespace CodeArt.Web.XamlControls.Metronic
{
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Input.Vis.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class Vis : Input
    {
        public static readonly DependencyProperty VisHeightProperty = DependencyProperty.Register<string, Vis>("VisHeight", () => { return string.Empty; });

        public string VisHeight
        {
            get
            {
                return GetValue(VisHeightProperty) as string;
            }
            set
            {
                SetValue(VisHeightProperty, value);
            }
        }

        public static readonly DependencyProperty AddNodesModalIdProperty = DependencyProperty.Register<string, Vis>("AddNodesModalId", () => { return string.Empty; });

        public string AddNodesModalId
        {
            get
            {
                return GetValue(AddNodesModalIdProperty) as string;
            }
            set
            {
                SetValue(AddNodesModalIdProperty, value);
            }
        }


        public static readonly DependencyProperty SetEdgeModalIdProperty = DependencyProperty.Register<string, Vis>("SetEdgeModalId", () => { return string.Empty; });

        public string SetEdgeModalId
        {
            get
            {
                return GetValue(SetEdgeModalIdProperty) as string;
            }
            set
            {
                SetValue(SetEdgeModalIdProperty, value);
            }
        }

        public override void OnInit()
        {
            base.OnInit();
            this.AddNodesModalId = string.Format("Modal_{0}",Guid.NewGuid().ToString("N"));
            this.SetEdgeModalId = string.Format("Modal_{0}", Guid.NewGuid().ToString("N"));
        }

    }
}
