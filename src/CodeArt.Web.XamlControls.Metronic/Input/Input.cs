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
    public class Input : Control
    {
        public static readonly DependencyProperty FieldProperty = DependencyProperty.Register<string, Input>("Field", () => { return string.Empty; });

        /// <summary>
        /// 数据字段的名称
        /// </summary>
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

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register<string, Input>("Label", () => { return string.Empty; });

        public string Label
        {
            get
            {
                return GetValue(LabelProperty) as string;
            }
            set
            {
                SetValue(LabelProperty, value);
            }
        }

        public static readonly DependencyProperty HelpProperty = DependencyProperty.Register<string, Input>("Help", () => { return string.Empty; });

        public string Help
        {
            get
            {
                return GetValue(HelpProperty) as string;
            }
            set
            {
                SetValue(HelpProperty, value);
            }
        }

        /// <summary>
        /// 是否单行显示
        /// </summary>
        public static readonly DependencyProperty LayoutProperty = DependencyProperty.Register<Layout, Input>("Layout", () => { return Layout.Inline; });

        public Layout Layout
        {
            get
            {
                return (Layout)GetValue(LayoutProperty);
            }
            set
            {
                SetValue(LayoutProperty, value);
            }
        }

        /// <summary>
        /// 必填的
        /// </summary>
        public static readonly DependencyProperty RequiredProperty = DependencyProperty.Register<bool, Input>("Required", () => { return false; });

        public bool Required
        {
            get
            {
                return (bool)GetValue(RequiredProperty);
            }
            set
            {
                SetValue(RequiredProperty, value);
            }
        }

        /// <summary>
        /// 前置内容
        /// </summary>
        public readonly static DependencyProperty PrependProperty = DependencyProperty.Register<UIElementCollection, Input>("Prepend", () => { return new UIElementCollection(); });

        public UIElementCollection Prepend
        {
            get
            {
                return GetValue(PrependProperty) as UIElementCollection;
            }
            set
            {
                SetValue(PrependProperty, value);
            }
        }

        /// <summary>
        /// 后置内容
        /// </summary>
        public readonly static DependencyProperty AppendProperty = DependencyProperty.Register<UIElementCollection, Input>("Append", () => { return new UIElementCollection(); });

        public UIElementCollection Append
        {
            get
            {
                return GetValue(AppendProperty) as UIElementCollection;
            }
            set
            {
                SetValue(AppendProperty, value);
            }
        }

        public override void OnInit()
        {
            //输入组件默认就会加入到表单中，写了该属性，就会打印 data-form=''
            if(!this.Attributes.Contains("form"))
            {
                this.Attributes.SetValue(this, "form", "");
            }
            base.OnInit();
        }

        public override DependencyObject GetChild(string childName)
        {
            return base.GetChild(childName) ?? this.Prepend.GetChild(childName) ?? this.Append.GetChild(childName);
        }

    }
}