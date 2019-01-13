using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.Util;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class Lazy : ContentControl
    {
        public readonly static DependencyProperty ContextProperty = DependencyProperty.Register<UIElementCollection, Lazy>("Context", () => { return new UIElementCollection(); });

        public UIElementCollection Context
        {
            get
            {
                return GetValue(ContextProperty) as UIElementCollection;
            }
            set
            {
                SetValue(ContextProperty, value);
            }
        }

        public override DependencyObject GetChild(string childName)
        {
            return base.GetChild(childName) ?? this.Context.GetChild(childName);
        }

        public override IEnumerable<UIElement> GetActionElement(string actionName)
        {
            return this.Combine(base.GetActionElement(actionName), this.Context.GetActionElement(actionName));
        }

        public Lazy()
        {
        }

        protected override void Draw(PageBrush brush)
        {
            brush.DrawLine("<div class='_lazyContent'>");
            this.Content.Render(brush);
            brush.DrawLine("</div>");

            if(WebPageContext.Current.GetQueryValue<int>("loadContent",0) == 0)
            {
                brush.DrawLine("<div class='_lazyContext'>");
                this.Context.Render(brush);
                brush.DrawLine("</div>");
            }
        }
    }
}
