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

using CodeArt.WPF.UI;

namespace CodeArt.WPF.Controls.Playstation
{
    public class DesktopView : View
    {
        public static readonly DependencyProperty HeaderRightProperty = DependencyProperty.Register("HeaderRight", typeof(object), typeof(DesktopView));

        public object HeaderRight
        {
            get { return (object)GetValue(HeaderRightProperty); }
            set { SetValue(HeaderRightProperty, value); }
        }

        public static readonly DependencyProperty ShowHeaderProperty = DependencyProperty.Register("ShowHeader", typeof(bool), typeof(DesktopView), new PropertyMetadata(true));

        /// <summary>
        /// 是否显示头部
        /// </summary>
        public bool ShowHeader
        {
            get { return (bool)GetValue(ShowHeaderProperty); }
            set { SetValue(ShowHeaderProperty, value); }
        }


        private DesktopViewHeader header;

        public DesktopView()
        {
            this.DefaultStyleKey = typeof(DesktopView);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            header = GetTemplateChild("header") as DesktopViewHeader;
        }

    }
}
