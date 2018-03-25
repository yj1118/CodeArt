using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.Data;
using CodeArt.ModuleNest;
using CodeArt.Concurrent;
using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.Util;

namespace CodeArt.Web.XamlControls.Metronic
{
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Input.List.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class List : Input
    {
        /// <summary>
        /// 列定义
        /// </summary>
        public readonly static DependencyProperty ColumnsProperty = DependencyProperty.Register<UIElementCollection, List>("Columns", () => { return new UIElementCollection(); });

        public UIElementCollection Columns
        {
            get
            {
                return GetValue(ColumnsProperty) as UIElementCollection;
            }
            set
            {
                SetValue(ColumnsProperty, value);
            }
        }

        public static readonly DependencyProperty MinLengthProperty = DependencyProperty.Register<int, List>("MinLength", () => { return 0; });

        public int MinLength
        {
            get
            {
                return (int)GetValue(MinLengthProperty);
            }
            set
            {
                SetValue(MinLengthProperty, value);
            }
        }

        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register<int, List>("MaxLength", () => { return 0; });

        public int MaxLength
        {
            get
            {
                return (int)GetValue(MaxLengthProperty);
            }
            set
            {
                SetValue(MaxLengthProperty, value);
            }
        }


        public static readonly DependencyProperty LengthProperty = DependencyProperty.Register<int, List>("Length", () => { return 1; });

        /// <summary>
        /// 
        /// </summary>
        public int Length
        {
            get
            {
                return (int)GetValue(LengthProperty);
            }
            set
            {
                SetValue(LengthProperty, value);
            }
        }


        public override DependencyObject GetChild(string childName)
        {
            return base.GetChild(childName) ?? this.Columns.GetChild(childName);
        }
    }
}
