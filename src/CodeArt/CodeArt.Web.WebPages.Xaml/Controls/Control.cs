using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;

//注意，Control的命名空间不在Controls下
namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// Control类会用模板来呈现自己
    /// 但是模板的概念由整个xaml空间共享，而不是control类独有的
    /// </summary>
    public abstract class Control : FrameworkElement, IControl
    {
        public Control()
        {
        }

        public virtual void OnApplyTemplate()
        {
            //组件加载完毕后，固化模板属性
            if (this.Template == null)
                this.Template = TemplateFactory.Create<ControlTemplate>(this, Control.TemplateProperty);
        }

        public override void OnInit()
        {
            OnApplyTemplate();

            base.OnInit();
        }

        #region 依赖属性

        public static DependencyProperty TemplateProperty { get; private set; }

        static Control()
        {
            var templateMetadata = new PropertyMetadata(()=> { return null; }, OnTemplateChanged);
            TemplateProperty = DependencyProperty.Register<ControlTemplate, Control>("Template", templateMetadata);
        }

        private static void OnTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var target = obj as Control;
            target.OnTemplateChanged(e);
        }

        protected virtual void OnTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            var template = e.NewValue as ControlTemplate;
            if (template != null) template.TemplateParent = this;
        }


        /// <summary>
        /// 获取或设置控件模板。控件模板用于定义控件在 UI 中的视觉外观，并在 XAML 标记中进行定义。
        /// 定义 Control 的外观的模板。ControlTemplate 可以将多个根元素作为其内容（这点跟WPF不一样）。
        /// </summary>
        public ControlTemplate Template
        {
            get
            {
                return GetValue(TemplateProperty) as ControlTemplate;
            }
            set
            {
                SetValue(TemplateProperty, value);
            }
        }

        #endregion

        //public object DefaultStyleKey { get; set; }

        protected override void Draw(PageBrush brush)
        {
            if (this.Template == null) throw new XamlException("没有模板，无法绘制控件" + this.GetType().FullName);
            this.Template.Render(brush);
        }

        public DependencyObject GetTemplateChild(string childName)
        {
            return this.Template != null ? this.Template.GetChild(childName) : null;
        }

        public T GetTemplateChild<T>(string childName) where T : class
        {
            return GetTemplateChild(childName) as T;
        }

        public override DependencyObject GetChild(string childName)
        {
            return base.GetChild(childName) ?? this.GetTemplateChild(childName);
        }

        public override void OnLoad()
        {
            if(this.Template != null) this.Template.OnLoad();
            base.OnLoad();
        }
    }
}
