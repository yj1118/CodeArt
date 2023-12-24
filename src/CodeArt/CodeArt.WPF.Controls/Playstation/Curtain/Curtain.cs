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
    public class Curtain : WorkScene
    {
        public static readonly DependencyProperty StageProperty = DependencyProperty.Register("Stage", typeof(UIElement), typeof(Curtain));

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

        public static readonly DependencyProperty ShowStageProperty = DependencyProperty.Register("ShowStage", typeof(bool), typeof(Curtain), new PropertyMetadata(false));

        /// <summary>
        /// 显示舞台区域
        /// </summary>
        public bool ShowStage
        {
            get
            {
                return (bool)GetValue(ShowStageProperty);
            }
            set
            {
                SetValue(ShowStageProperty, value);
            }
        }

        public static readonly DependencyProperty ShowPropsProperty = DependencyProperty.Register("ShowProps", typeof(bool), typeof(Curtain), new PropertyMetadata(false, ShowPropsChanged));

        /// <summary>
        /// 显示道具区
        /// </summary>
        public bool ShowProps
        {
            get
            {
                return (bool)GetValue(ShowPropsProperty);
            }
            set
            {
                SetValue(ShowPropsProperty, value);
            }
        }

        private static void ShowPropsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as Curtain;
            var showProps = (bool)e.NewValue;
            if (showProps)
            {
                c.InitAutoHiddenProps();
            }
            else
            {
                c.DisposeAutoHiddenProps();
            }
        }


        public static readonly DependencyProperty PropsProperty = DependencyProperty.Register("Props", typeof(UIElement), typeof(Curtain));

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

        public Curtain()
        {
            this.DefaultStyleKey = typeof(Curtain);
            this.MouseMove -= OnMouseMove;
            this.MouseMove += OnMouseMove;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            this.ShowProps = true;
            DelayAutoHiddenProps();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Work.Current.ShowTitleBar = false; //使用幕布不显示标题栏
            this.ShowProps = true;
        }

        public override void Exited()
        {
            Work.Current.ShowTitleBar = true;
            DisposeAutoHiddenProps();
        }


        #region 自动隐藏工具区域

        private TimeoutMonitor _timeout;

        private void OnTimeout()
        {
            this.Dispatcher.Invoke(()=>
            {
                this.ShowProps = false;
            });
        }

        private object _syncObject = new object();

        private void InitAutoHiddenProps()
        {
            lock(_syncObject)
            {
                _timeout = new TimeoutMonitor(OnTimeout);
                _timeout.Start(5000);
            }
        }

        private void DelayAutoHiddenProps()
        {
            if (_timeout != null)
            {
                _timeout.TryCancel();
                _timeout.Start(5000);
            }
        }

        private void DisposeAutoHiddenProps()
        {
            lock (_syncObject)
            {
                if (_timeout == null) return;
                _timeout.Dispose();
                _timeout = null;
            }
        }

        #endregion

    }
}
