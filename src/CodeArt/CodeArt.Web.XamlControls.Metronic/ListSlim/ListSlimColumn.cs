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
using FormSE = CodeArt.Web.XamlControls.Metronic.FormSE;
using CodeArt.Util;
using CodeArt.Web.WebPages;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class ListSlimColumn : ContentControl
    {
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register<string, ListSlimColumn>("Width", () => { return string.Empty; });
        public string Width
        {
            get
            {
                return (string)GetValue(WidthProperty);
            }
            set
            {
                SetValue(WidthProperty, value);
            }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register<string, ListSlimColumn>("Title", () => { return string.Empty; });
        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public static readonly DependencyProperty FieldProperty = DependencyProperty.Register<string, ListSlimColumn>("Field", () => { return string.Empty; });
        public string Field
        {
            get
            {
                return (string)GetValue(FieldProperty);
            }
            set
            {
                SetValue(FieldProperty, value);
            }
        }

        public static readonly DependencyProperty TextAlignProperty = DependencyProperty.Register<string, ListSlimColumn>("TextAlign", () => { return "left"; });

        public string TextAlign
        {
            get
            {
                return GetValue(TextAlignProperty) as string;
            }
            set
            {
                SetValue(TextAlignProperty, value);
            }
        }

        public static readonly DependencyProperty TextVerticalAlignProperty = DependencyProperty.Register<string, ListSlimColumn>("TextVerticalAlign", () => { return "top"; });

        public string TextVerticalAlign
        {
            get
            {
                return GetValue(TextVerticalAlignProperty) as string;
            }
            set
            {
                SetValue(TextVerticalAlignProperty, value);
            }
        }

        public static readonly DependencyProperty GetTemplateProperty = DependencyProperty.Register<string, ListSlimColumn>("GetTemplate", () => { return string.Empty; });

        public string GetTemplate
        {
            get
            {
                return GetValue(GetTemplateProperty) as string;
            }
            set
            {
                SetValue(GetTemplateProperty, value);
            }
        }

        public ListSlimColumn()
        { }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
        static ListSlimColumn()
        { }
    }
}