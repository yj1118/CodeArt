using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml.Controls
{
    [ContentProperty("Content")]
    public class ContentControl : Control
    {
        public readonly static DependencyProperty ContentProperty = DependencyProperty.Register<UIElementCollection, ContentControl>("Content", () => { return new UIElementCollection(); });

        public UIElementCollection Content
        {
            get
            {
                return GetValue(ContentProperty) as UIElementCollection;
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }

        public override DependencyObject GetChild(string childName)
        {
            return base.GetChild(childName) ?? this.Content.GetChild(childName);
        }

        protected override void Draw(PageBrush brush)
        {
            if (this.Template == null)
                this.Content.Render(brush);
            else
                this.Template.Render(brush);
        }

        public override void OnLoad()
        {
            this.Content.OnLoad();
            base.OnLoad();
        }

    }
}
