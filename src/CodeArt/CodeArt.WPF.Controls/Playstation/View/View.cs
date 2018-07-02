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
    public class View : ContentControl, IFadeInOut
    {
        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(ViewStatus), typeof(View), new PropertyMetadata(ViewStatus.Normal));

        /// <summary>
        /// 
        /// </summary>
        public ViewStatus Status
        {
            get { return (ViewStatus)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }


        public static readonly DependencyProperty LoadingMessageProperty = DependencyProperty.Register("LoadingMessage", typeof(string), typeof(View), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 加载时显示的消息内容
        /// </summary>
        public string LoadingMessage
        {
            get { return (string)GetValue(LoadingMessageProperty); }
            set { SetValue(LoadingMessageProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(View));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        private Loading loading;
        private UIElement content;

        public View()
        {
            this.DefaultStyleKey = typeof(View);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            loading = GetTemplateChild("loading") as Loading;
            content = GetTemplateChild("content") as UIElement;
        }

        private bool _isRendered = false;

        /// <summary>
        /// 第一次呈现的时候会触发该方法
        /// </summary>
        public virtual void Rendered() { }

        /// <summary>
        /// 视图被移除时触发
        /// </summary>
        public virtual void Exited() { }

        /// <summary>
        /// 视图被重置时触发
        /// </summary>
        public virtual void Reset() { }


        public void Open(Action complete)
        {
            this.Visibility = Visibility.Visible;
            FadeInOut.RaisePreFadeIn(this);
            Animations.OpacityOut(this, 500, EasingMode.EaseOut, () =>
            {
                if (!_isRendered)
                {
                    _isRendered = true;
                    Rendered();
                }
                FadeInOut.RaiseFadedIn(this);
                complete();
            });
        }

        public void Close(Action complete)
        {
            FadeInOut.RaisePreFadeOut(this);
            Animations.OpacityIn(this, 500, EasingMode.EaseOut, () =>
             {
                 this.Visibility = Visibility.Collapsed;
                 FadeInOut.RaiseFadedOut(this);
                 complete();
            });
        }

        /// <summary>
        /// 预加载视图使用该方法模拟打开时应该触发的事件
        /// </summary>
        internal void SimulateOpen()
        {
            FadeInOut.RaisePreFadeIn(this);
            if (!_isRendered)
            {
                _isRendered = true;
                Rendered();
            }
            FadeInOut.RaiseFadedIn(this);
        }

        public void SimulateClose()
        {
            FadeInOut.RaisePreFadeOut(this);
            this.Visibility = Visibility.Collapsed;
            FadeInOut.RaiseFadedOut(this);
        }

        public string ViewName
        {
            get;
            internal set;
        }

        public string ViewTextName
        {
            get;
            internal set;
        }


        //public event Action Opened;
        //private void RaiseOpened()
        //{
        //    if (this.Opened != null)
        //        this.Opened();
        //}


        //public event Action Closed;
        //private void RaiseClosed()
        //{
        //    if (this.Closed != null)
        //        this.Closed();
        //}


        public virtual void PreFadeOut()
        {
        }

        public virtual void FadedOut()
        {
        }

        public virtual void PreFadeIn()
        {
        }

        public virtual void FadedIn()
        {
        }

    }
}
