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

namespace CodeArt.WPF.Controls.Playstation
{
    /// <summary>
    /// MessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class MessageBox : WorkScene
    {
        public MessageBox()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public static readonly DependencyProperty OKTextProperty = DependencyProperty.Register("OKText", typeof(string), typeof(MessageBox), new PropertyMetadata(Strings.OK));

        public string OKText
        {
            get { return (string)GetValue(OKTextProperty); }
            set { SetValue(OKTextProperty, value); }
        }


        public static readonly DependencyProperty CancelTextProperty = DependencyProperty.Register("CancelText", typeof(string), typeof(MessageBox), new PropertyMetadata(Strings.Cancel));

        public string CancelText
        {
            get { return (string)GetValue(CancelTextProperty); }
            set { SetValue(CancelTextProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(MessageBox));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public Action CancelAction = null;

        private void OnCancel(object sender, MouseButtonEventArgs e)
        {
            Back();
        }

        private void Back()
        {
            if (this.CancelAction != null)
            {
                this.CancelAction();
                return;
            }
            Work.Current.Back(); //默认是返回
        }


        public Action OKAction = null;

        private void OnOK(object sender, MouseButtonEventArgs e)
        {
            if (this.OKAction != null)
            {
                this.OKAction();
                return;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.MakeChildsLoaded(page);
            this.page.BackAction = Back;
        }
    }
}
