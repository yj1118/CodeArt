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

namespace CodeArt.WPF.Controls.Playstation
{
    public class Page : WorkScene
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(Page));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty FooterCommandProperty = DependencyProperty.Register("FooterCommand", typeof(object), typeof(Page));

        public object FooterCommand
        {
            get { return (object)GetValue(FooterCommandProperty); }
            set { SetValue(FooterCommandProperty, value); }
        }

        public static readonly DependencyProperty ShowBackProperty = DependencyProperty.Register("ShowBack", typeof(bool), typeof(Page),new PropertyMetadata(true));

        public bool ShowBack
        {
            get
            {
                return (bool)GetValue(ShowBackProperty);
            }
            set
            {
                SetValue(ShowBackProperty, value);
            }
        }

        public static readonly DependencyProperty ShowFooterProperty = DependencyProperty.Register("ShowFooter", typeof(bool), typeof(Page), new PropertyMetadata(true));

        public bool ShowFooter
        {
            get
            {
                return (bool)GetValue(ShowFooterProperty);
            }
            set
            {
                SetValue(ShowFooterProperty, value);
            }
        }

        public static readonly DependencyProperty ShowHeaderProperty = DependencyProperty.Register("ShowHeader", typeof(bool), typeof(Page), new PropertyMetadata(true));

        public bool ShowHeader
        {
            get
            {
                return (bool)GetValue(ShowHeaderProperty);
            }
            set
            {
                SetValue(ShowHeaderProperty, value);
            }
        }



        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(PageStatus), typeof(Page), new PropertyMetadata(PageStatus.Normal));

        /// <summary>
        /// 
        /// </summary>
        public PageStatus Status
        {
            get { return (PageStatus)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        public static readonly DependencyProperty ContainerMarginProperty = DependencyProperty.Register("ContainerMargin", typeof(Thickness), typeof(Page), new PropertyMetadata(new Thickness(150, 0, 150, 0)));

        public Thickness ContainerMargin
        {
            get
            {
                return (Thickness)GetValue(ContainerMarginProperty);
            }
            set
            {
                SetValue(ContainerMarginProperty, value);
            }
        }

        public static readonly DependencyProperty LoadingMessageProperty = DependencyProperty.Register("LoadingMessage", typeof(string), typeof(Page), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 加载时显示的消息内容
        /// </summary>
        public string LoadingMessage
        {
            get { return (string)GetValue(LoadingMessageProperty); }
            set { SetValue(LoadingMessageProperty, value); }
        }


        private Loading loading;
        private UIElement content;
        private Footer footer;

        public Page()
        {
            this.DefaultStyleKey = typeof(Page);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            loading = GetTemplateChild("loading") as Loading;
            content = GetTemplateChild("content") as UIElement;
            footer = GetTemplateChild("footer") as Footer;

            this.MakeChildsLoaded(footer);

            _fixedShowBack = this.ShowBack == false;

            UpdateShowBack();
        }

        private bool _fixedShowBack = false;


        public override void Rendered()
        {
            UpdateShowBack();
        }

        private void UpdateShowBack()
        {
            if (_fixedShowBack) return;
            if (Work.Current != null)
            {
                this.ShowBack = Work.Current.ElementCount > 1;
            }
        }


        public Action BackAction
        {
            get
            {
                return footer.BackAction;
            }
            set
            {
                footer.BackAction = value;
            }
        }
    }
}
