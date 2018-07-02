using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

using CodeArt.WPF.UI;

namespace CodeArt.WPF.Controls.Playstation
{
    public class ActionBar : Control
    {
        private static readonly DependencyProperty LeftButtonsProperty = DependencyProperty.Register("LeftButtons", typeof(List<ActionBarButton>), typeof(ActionBar),new PropertyMetadata(new List<ActionBarButton>()));


        /// <summary>
        /// 
        /// </summary>
        public List<ActionBarButton> LeftButtons
        {
            get
            {
                return (List<ActionBarButton>)GetValue(LeftButtonsProperty);
            }
            set
            {
                SetValue(LeftButtonsProperty, value);
            }
        }

        private static readonly DependencyProperty RightButtonsProperty = DependencyProperty.Register("RightButtons", typeof(List<ActionBarButton>), typeof(ActionBar), new PropertyMetadata(new List<ActionBarButton>()));


        /// <summary>
        /// 
        /// </summary>
        public List<ActionBarButton> RightButtons
        {
            get
            {
                return (List<ActionBarButton>)GetValue(RightButtonsProperty);
            }
            set
            {
                SetValue(RightButtonsProperty, value);
            }
        }

        public ActionBar()
        {
            this.DefaultStyleKey = typeof(ActionBar);
            this.SetValue(LeftButtonsProperty, new List<ActionBarButton>());
            this.SetValue(RightButtonsProperty, new List<ActionBarButton>());
        }

        private StackPanel left;
        private StackPanel right;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            left = GetTemplateChild("left") as StackPanel;
            right = GetTemplateChild("right") as StackPanel;

            foreach(var btn in this.LeftButtons)
            {
                left.Children.Add(btn);
            }

            foreach (var btn in this.RightButtons)
            {
                right.Children.Insert(0, btn);
            }

            if(this.LeftButtons.Count() == 0 && this.RightButtons.Count() == 0)
            {
                this.Visibility = Visibility.Collapsed;
            }

        }



    }
}