using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.ModuleNest;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class TodoStyleRepeaterItem : FrameworkElement
    {
        public static DependencyProperty BorderColorProperty { get; private set; }
        public static DependencyProperty TitleProperty { get; private set; }
        public static DependencyProperty ContentsProperty { get; private set; }
        public static DependencyProperty TimeProperty { get; private set; }
        public static DependencyProperty TagsProperty { get; private set; }

        public static DependencyProperty UrlProperty { get; private set; }
        public static DependencyProperty ColumnClassProperty { get; private set; }

        static TodoStyleRepeaterItem()
        {
            var borderColorMetadata = new PropertyMetadata(() => { return string.Empty; });
            BorderColorProperty = DependencyProperty.Register<string, TodoStyleRepeaterItem>("BorderColor", borderColorMetadata);

            var titleMetadata = new PropertyMetadata(() => { return string.Empty; });
            TitleProperty = DependencyProperty.Register<string, TodoStyleRepeaterItem>("Title", titleMetadata);

            var contentsMetadata = new PropertyMetadata(() => { return new string[0]; });
            ContentsProperty = DependencyProperty.Register<string[], TodoStyleRepeaterItem>("Contents", contentsMetadata);

            var timeMetadata = new PropertyMetadata(() => { return string.Empty; });
            TimeProperty = DependencyProperty.Register<string, TodoStyleRepeaterItem>("Time", timeMetadata);

            var tagsMetadata = new PropertyMetadata(() => { return new string[0]; });
            TagsProperty = DependencyProperty.Register<string[], TodoStyleRepeaterItem>("Tags", tagsMetadata);

            var urlMetadata = new PropertyMetadata(() => { return string.Empty; });
            UrlProperty = DependencyProperty.Register<string, TodoStyleRepeaterItem>("Url", urlMetadata);

            var columnClassMetadata = new PropertyMetadata(() => { return string.Empty; });
            ColumnClassProperty = DependencyProperty.Register<string, TodoStyleRepeaterItem>("ColumnClass", columnClassMetadata);
        }

        public string ColumnClass
        {
            get
            {
                return GetValue(ColumnClassProperty) as string;
            }
            set
            {
                SetValue(ColumnClassProperty, value);
            }
        }

        public string BorderColor
        {
            get
            {
                return GetValue(BorderColorProperty) as string;
            }
            set
            {
                SetValue(BorderColorProperty, value);
            }
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

        public string[] Contents
        {
            get
            {
                return (string[])GetValue(ContentsProperty);
            }
            set
            {
                SetValue(ContentsProperty, value);
            }
        }

        public string Time
        {
            get
            {
                return GetValue(TimeProperty) as string;
            }
            set
            {
                SetValue(TimeProperty, value);
            }
        }

        public string[] Tags
        {
            get
            {
                return (string[])GetValue(TagsProperty);
            }
            set
            {
                SetValue(TagsProperty, value);
            }
        }

        public string Url
        {
            get
            {
                return GetValue(UrlProperty) as string;
            }
            set
            {
                SetValue(UrlProperty, value);
            }
        }

        protected override void Draw(PageBrush brush)
        {
            brush.DrawFormat("<div class=\"{0}\" style=\"margin-bottom:20px;\">", this.ColumnClass);
            brush.DrawLine();

            brush.DrawFormat("<a href=\"{0}\" target=\"_blank\" style=\"text-decoration:none;\">", this.Url);
            brush.DrawLine();

            brush.DrawFormat("<div class=\"todo-tasklist-item todo-tasklist-item-border-{0}\">", this.BorderColor);
            brush.DrawLine();

            brush.DrawFormat("<div class=\"todo-tasklist-item-title\">{0}</div>", this.Title);
            brush.DrawLine();

            foreach (var content in Contents)
            {
                brush.DrawFormat("<div class=\"todo-tasklist-item-text\">{0}</div>", content);
                brush.DrawLine();
            }

            if (!string.IsNullOrEmpty(this.Time) || this.Tags.Length > 0)
            {

                brush.Draw("<div class=\"todo-tasklist-item-text\" style=\"margin-top:5px\">");
                brush.DrawLine();

                if (!string.IsNullOrEmpty(this.Time))
                {
                    brush.DrawFormat("<span class=\"todo-tasklist-date\"><i class=\"fa fa-calendar\"></i> {0} </span>", this.Time);
                    brush.DrawLine();
                }

                foreach (string tag in Tags)
                {
                    brush.DrawFormat("<span class=\"todo-tasklist-badge badge badge-roundless\">{0}</span>", tag);
                    brush.DrawLine();
                }

                brush.Draw("</div>");
                brush.DrawLine();
            }
            brush.Draw("</div>");
            brush.DrawLine();

            brush.Draw("</a>");
            brush.DrawLine();

            brush.Draw("</div>");
            brush.DrawLine();
        }

    }
}
