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
using System.Threading;

using CodeArt.Util;

namespace CodeArt.WPF.Controls.Playstation
{
    public class Desktop : ViewOwner
    {
        public static readonly DependencyProperty MenuFloatProperty = DependencyProperty.Register("MenuFloat", typeof(bool), typeof(Desktop), new PropertyMetadata(false));

        public bool MenuFloat
        {
            get
            {
                return (bool)GetValue(MenuFloatProperty);
            }
            set
            {
                SetValue(MenuFloatProperty, value);
            }
        }

        public Desktop()
        {
            this.DefaultStyleKey = typeof(Desktop);
        }

        protected override ViewMenu GetMenu()
        {
            return GetTemplateChild("menu") as DesktopMenu;
        }

        protected override ViewContainer GetContainer()
        {
            return GetTemplateChild("container") as ViewContainer;
        }

    }
}
