using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;


namespace CodeArt.Web.WebPages.Xaml.Component
{
    [TemplateCode("Template", "CodeArt.Web.WebPages.Xaml.Component.Dropload.Template.html,CodeArt.Web.WebPages.Xaml")]
    [ContentProperty("RowTemplate")]
    public class Dropload : Control
    {
        public static DependencyProperty RowTemplateProperty = DependencyProperty.Register<UIElementCollection, Dropload>("RowTemplate", new PropertyMetadata(() => { return new UIElementCollection(); }));

        /// <summary>
        /// 每行的数据模板
        /// </summary>
        public UIElementCollection RowTemplate
        {
            get
            {
                return GetValue(RowTemplateProperty) as UIElementCollection;
            }
            set
            {
                SetValue(RowTemplateProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public readonly static DependencyProperty PageSizeProperty = DependencyProperty.Register<int, Dropload>("PageSize", () => { return 10; });

        public int PageSize
        {
            get
            {
                return (int)GetValue(PageSizeProperty);
            }
            set
            {
                SetValue(PageSizeProperty, value);
            }
        }

        public static DependencyProperty ActionProperty = DependencyProperty.Register<string, Dropload>("Action", new PropertyMetadata(() => { return string.Empty; }));

        /// <summary>
        /// 
        /// </summary>
        public string Action
        {
            get
            {
                return GetValue(ActionProperty) as string;
            }
            set
            {
                SetValue(ActionProperty, value);
            }
        }

        public static DependencyProperty EmptyTipProperty = DependencyProperty.Register<string, Dropload>("EmptyTip", new PropertyMetadata(() => { return "暂无数据"; }));

        /// <summary>
        /// 
        /// </summary>
        public string EmptyTip
        {
            get
            {
                return GetValue(EmptyTipProperty) as string;
            }
            set
            {
                SetValue(EmptyTipProperty, value);
            }
        }

        public readonly static DependencyProperty AutoProperty = DependencyProperty.Register<bool, Dropload>("Auto", () => { return true; });

        /// <summary>
        /// 是否自动加载
        /// </summary>
        public bool Auto
        {
            get
            {
                return (bool)GetValue(AutoProperty);
            }
            set
            {
                SetValue(AutoProperty, value);
            }
        }

        public Dropload()
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        static Dropload()
        { }
    }
}