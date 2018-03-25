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

using CodeArt.WPF;
using CodeArt.Concurrent.Sync;

namespace CodeArt.WPF.Controls.Playstation
{
    /// <summary>
    /// 
    /// </summary>
    public class StaticCurtain : WorkScene
    {
        public static readonly DependencyProperty StageProperty = DependencyProperty.Register("Stage", typeof(UIElement), typeof(StaticCurtain));

        public UIElement Stage
        {
            get
            {
                return (UIElement)GetValue(StageProperty);
            }
            set
            {
                SetValue(StageProperty, value);
            }
        }



        public static readonly DependencyProperty PropsProperty = DependencyProperty.Register("Props", typeof(UIElement), typeof(StaticCurtain));

        public UIElement Props
        {
            get
            {
                return (UIElement)GetValue(PropsProperty);
            }
            set
            {
                SetValue(PropsProperty, value);
            }
        }

        public static readonly DependencyProperty ShowPropsProperty = DependencyProperty.Register("ShowProps", typeof(bool), typeof(StaticCurtain), new PropertyMetadata(true));

        /// <summary>
        /// 
        /// </summary>
        public bool ShowProps
        {
            get { return (bool)GetValue(ShowPropsProperty); }
            set { SetValue(ShowPropsProperty, value); }
        }

        public StaticCurtain()
        {
            this.DefaultStyleKey = typeof(StaticCurtain);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Work.Current.ShowTitleBar = false; //使用幕布不显示标题栏
        }

        public override void Exited()
        {
            Work.Current.ShowTitleBar = true;
        }

    }
}

