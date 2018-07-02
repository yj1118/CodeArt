using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.ModuleNest;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class DataTableColumn : ContentControl
    {
        public static readonly DependencyProperty FieldProperty = DependencyProperty.Register<string, DataTableColumn>("Field", () => { return string.Empty; });

        public string Field
        {
            get
            {
                return GetValue(FieldProperty) as string;
            }
            set
            {
                SetValue(FieldProperty, value);
            }
        }

        public static readonly DependencyProperty SelectorProperty = DependencyProperty.Register<bool, DataTableColumn>("Selector", () => { return false; });

        public bool Selector
        {
            get
            {
                return (bool)GetValue(SelectorProperty);
            }
            set
            {
                SetValue(SelectorProperty, value);
            }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register<string, DataTableColumn>("Title", () => { return string.Empty; });

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

        public static readonly DependencyProperty SortableProperty = DependencyProperty.Register<bool, DataTableColumn>("Sortable", () => { return false; });

        public bool Sortable
        {
            get
            {
                return (bool)GetValue(SortableProperty);
            }
            set
            {
                SetValue(SortableProperty, value);
            }
        }

        public static readonly DependencyProperty TextAlignProperty = DependencyProperty.Register<string, DataTableColumn>("TextAlign", () => { return "left"; });

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

        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register<string, DataTableColumn>("Width", () => { return string.Empty; });

        public string Width
        {
            get
            {
                return GetValue(WidthProperty) as string;
            }
            set
            {
                SetValue(WidthProperty, value);
            }
        }

        public static readonly DependencyProperty ResponsiveProperty = DependencyProperty.Register<string, DataTableColumn>("Responsive", () => { return string.Empty; });

        public string Responsive
        {
            get
            {
                return GetValue(ResponsiveProperty) as string;
            }
            set
            {
                SetValue(ResponsiveProperty, value);
            }
        }

        //public static readonly DependencyProperty FormatProperty = DependencyProperty.Register<string, DataTableColumn>("Format", () => { return string.Empty; });

        //public string Format
        //{
        //    get
        //    {
        //        return GetValue(FormatProperty) as string;
        //    }
        //    set
        //    {
        //        SetValue(FormatProperty, value);
        //    }
        //}

        /// <summary>
        /// 数据类型，number,date
        /// </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register<string, DataTableColumn>("Type", () => { return string.Empty; });

        public string Type
        {
            get
            {
                return GetValue(TypeProperty) as string;
            }
            set
            {
                SetValue(TypeProperty, value);
            }
        }

        public static readonly DependencyProperty GetTemplateProperty = DependencyProperty.Register<string, DataTableColumn>("GetTemplate", () => { return string.Empty; });

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

        /// <summary>
        /// 指示该列在行被选择时，是否作为文本输出
        /// </summary>
        public static readonly DependencyProperty TextFieldProperty = DependencyProperty.Register<bool, DataTableColumn>("TextField", () => { return false; });

        public bool TextField
        {
            get
            {
                return (bool)GetValue(TextFieldProperty);
            }
            set
            {
                SetValue(TextFieldProperty, value);
            }
        }

        /// <summary>
        /// 内容超出行高度时是否显示，一些下拉效果需要将该属性设置为visible
        /// </summary>
        public static readonly DependencyProperty OverflowProperty = DependencyProperty.Register<string, DataTableColumn>("Overflow", () => { return string.Empty; });

        public string Overflow
        {
            get
            {
                return (string)GetValue(OverflowProperty);
            }
            set
            {
                SetValue(OverflowProperty, value);
            }
        }

    }
}
