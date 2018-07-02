using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Markup;

using CodeArt.WPF.UI;
using CodeArt.DTO;


namespace CodeArt.WPF.Controls.Playstation
{
    [ContentProperty("Children")]
    public class StackForm : FormBase
    {
        /// <summary>
        /// 表单内部组件的标签宽度
        /// </summary>
        public static readonly DependencyProperty LabelWidthProperty = DependencyProperty.Register("LabelWidth", typeof(double), typeof(StackForm), new PropertyMetadata((double)200));

        public double LabelWidth
        {
            get
            {
                return (double)GetValue(LabelWidthProperty);
            }
            set
            {
                SetValue(LabelWidthProperty, value);
            }
        }


        public static readonly DependencyProperty TipProperty = DependencyProperty.Register("Tip", typeof(string), typeof(StackForm),new PropertyMetadata(string.Empty));

        public string Tip
        {
            get
            {
                return (string)GetValue(TipProperty);
            }
            set
            {
                SetValue(TipProperty, value);
            }
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(object), typeof(StackForm));

        public object Command
        {
            get
            {
                return (object)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        /// <summary>
        /// 操作处理完毕的提示信息
        /// </summary>
        public static readonly DependencyProperty ChildrenProperty = DependencyProperty.Register("Children", typeof(UIElementCollection), typeof(StackForm), new PropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public UIElementCollection Children
        {
            get
            {
                var children = GetValue(ChildrenProperty);
                if (children == null)
                {
                    children = new UIElementCollection(this, this);
                    SetValue(ChildrenProperty, children);
                }
                return (UIElementCollection)GetValue(ChildrenProperty);
            }
            set { SetValue(ChildrenProperty, value); }
        }

        private StackPanel content;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            content = GetTemplateChild("content") as StackPanel;

            UIElement[] temp = new UIElement[this.Children.Count];
            this.Children.CopyTo(temp, 0);
            foreach(var child in temp)
            {
                this.Children.Remove(child);
                content.Children.Add(child);
            }
        }

        public StackForm()
        {
            this.DefaultStyleKey = typeof(StackForm);
        }
    }
}