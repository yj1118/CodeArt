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

using CodeArt.WPF;

namespace CodeArt.WPF.Controls.Playstation
{
    public class Pages : ViewOwner
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(Pages));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty HeaderRightProperty = DependencyProperty.Register("HeaderRight", typeof(object), typeof(Pages));

        public object HeaderRight
        {
            get { return (object)GetValue(HeaderRightProperty); }
            set { SetValue(HeaderRightProperty, value); }
        }

        public Pages()
        {
            this.DefaultStyleKey = typeof(Pages);
        }

        protected override ViewContainer GetContainer()
        {
            return GetTemplateChild("container") as ViewContainer;
        }

        protected override ViewMenu GetMenu()
        {
            return GetTemplateChild("menu") as ViewMenu;
        }

    }
}
