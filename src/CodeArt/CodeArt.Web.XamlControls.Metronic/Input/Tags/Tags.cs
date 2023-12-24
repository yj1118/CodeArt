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
using CodeArt.Concurrent;
using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.Util;

namespace CodeArt.Web.XamlControls.Metronic
{
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Input.Tags.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class Tags : Input
    {
        public static readonly DependencyProperty ColumnProperty = DependencyProperty.Register<int, Tags>("Column", () => { return 4; });

        /// <summary>
        /// 一行显示几列
        /// </summary>
        public int Column
        {
            get
            {
                return (int)GetValue(ColumnProperty);
            }
            set
            {
                SetValue(ColumnProperty, value);
            }
        }

        public static readonly DependencyProperty MinLengthProperty = DependencyProperty.Register<int, Tags>("MinLength", () => { return 0; });

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

        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register<int, Tags>("MaxLength", () => { return 0; });

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


        public static readonly DependencyProperty LengthProperty = DependencyProperty.Register<int, Tags>("Length", () => { return 1; });

        /// <summary>
        /// 默认长度
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

        public static readonly DependencyProperty AllowAddProperty = DependencyProperty.Register<bool, Tags>("AllowAdd", () => { return true; });

        /// <summary>
        /// 是否允许新增行
        /// </summary>
        public bool AllowAdd
        {
            get
            {
                return (bool)GetValue(AllowAddProperty);
            }
            set
            {
                SetValue(AllowAddProperty, value);
            }
        }

        public static readonly DependencyProperty AllowRemoveProperty = DependencyProperty.Register<bool, Tags>("AllowRemove", () => { return true; });

        /// <summary>
        /// 是否允许删除行
        /// </summary>
        public bool AllowRemove
        {
            get
            {
                return (bool)GetValue(AllowRemoveProperty);
            }
            set
            {
                SetValue(AllowRemoveProperty, value);
            }
        }

        public static readonly DependencyProperty ItemMinLengthProperty = DependencyProperty.Register<int, Tags>("ItemMinLength", () => { return 0; });

        public int ItemMinLength
        {
            get
            {
                return (int)GetValue(ItemMinLengthProperty);
            }
            set
            {
                SetValue(ItemMinLengthProperty, value);
            }
        }

        public static readonly DependencyProperty ItemMaxLengthProperty = DependencyProperty.Register<int, Tags>("ItemMaxLength", () => { return 0; });

        public int ItemMaxLength
        {
            get
            {
                return (int)GetValue(ItemMaxLengthProperty);
            }
            set
            {
                SetValue(ItemMaxLengthProperty, value);
            }
        }

        public static readonly DependencyProperty ItemLabelProperty = DependencyProperty.Register<string, Input>("ItemLabel", () => { return string.Empty; });

        public string ItemLabel
        {
            get
            {
                return GetValue(ItemLabelProperty) as string;
            }
            set
            {
                SetValue(ItemLabelProperty, value);
            }
        }

    }
}
