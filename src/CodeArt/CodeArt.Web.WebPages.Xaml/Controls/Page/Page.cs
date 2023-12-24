using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml.Controls
{
    [TemplateCode("Template", "CodeArt.Web.WebPages.Xaml.Controls.Page.Template.html,CodeArt.Web.WebPages.Xaml")]
    [ContentProperty("Body")]
    public class Page : Control
    {
        public static DependencyProperty TitleProperty { get; private set; }

        public static DependencyProperty BodyProperty { get; private set; }

        public static DependencyProperty ShortcutIconProperty { get; private set; }

        public static DependencyProperty KeywordsProperty { get; private set; }

        public static DependencyProperty DescriptionProperty { get; private set; }

        public static DependencyProperty CopyrightProperty { get; private set; }


        static Page()
        {
            var titleMetadata = new PropertyMetadata(() => { return string.Empty; });
            TitleProperty = DependencyProperty.Register<string, Page>("Title", titleMetadata);

            var bodyMetadata = new PropertyMetadata(() => { return new UIElementCollection(); });
            BodyProperty = DependencyProperty.Register<UIElementCollection, Page>("Body", bodyMetadata);

            var shortcutIconMetadata = new PropertyMetadata(() => { return "javascript:;"; });
            ShortcutIconProperty = DependencyProperty.Register<string, Page>("ShortcutIcon", shortcutIconMetadata);

            var keywordsMetadata = new PropertyMetadata(() => { return string.Empty; });
            KeywordsProperty = DependencyProperty.Register<string, Page>("Keywords", keywordsMetadata);

            var descriptionMetadata = new PropertyMetadata(() => { return string.Empty; });
            DescriptionProperty = DependencyProperty.Register<string, Page>("Description", descriptionMetadata);

            var copyrightMetadata = new PropertyMetadata(() => { return string.Empty; });
            CopyrightProperty = DependencyProperty.Register<string, Page>("Copyright", copyrightMetadata);
        }

        public string Title
        {
            get
            {
                return GetValue(TitleProperty) as string;
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public UIElementCollection Body
        {
            get
            {
                return GetValue(BodyProperty) as UIElementCollection;
            }
            set
            {
                SetValue(BodyProperty, value);
            }
        }

        public string ShortcutIcon
        {
            get
            {
                return GetValue(ShortcutIconProperty) as string;
            }
            set
            {
                SetValue(ShortcutIconProperty, value);
            }
        }

        public string Keywords
        {
            get
            {
                return GetValue(KeywordsProperty) as string;
            }
            set
            {
                SetValue(KeywordsProperty, value);
            }
        }

        public string Description
        {
            get
            {
                return GetValue(DescriptionProperty) as string;
            }
            set
            {
                SetValue(DescriptionProperty, value);
            }
        }

        public string Copyright
        {
            get
            {
                return GetValue(CopyrightProperty) as string;
            }
            set
            {
                SetValue(CopyrightProperty, value);
            }
        }

        protected override void Draw(PageBrush brush)
        {
            brush.DrawLine("<!doctype html>");
            base.Draw(brush);
        }

        public override DependencyObject GetChild(string childName)
        {
            return base.GetChild(childName) ?? this.Body.GetChild(childName);
        }

        public override IEnumerable<UIElement> GetActionElement(string actionName)
        {
            return this.Combine(base.GetActionElement(actionName) , this.Body.GetActionElement(actionName));
        }

        public override void OnLoad()
        {
            this.Body.OnLoad();
            base.OnLoad();
        }
    }
}
