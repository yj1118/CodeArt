using CodeArt.Concurrent.Sync;
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
    /// ProgressBox.xaml 的交互逻辑
    /// </summary>
    public partial class ProgressBox : WorkScene
    {
        public static ProgressBox Current
        {
            get;
            private set;
        }


        public ProgressBox()
        {
            InitializeComponent();
            this.DataContext = this;
            Current = this;
        }


        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(ProgressBox));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }


        public static readonly DependencyProperty CancelTextProperty = DependencyProperty.Register("CancelText", typeof(string), typeof(ProgressBox), new PropertyMetadata(Strings.Cancel));

        public string CancelText
        {
            get { return (string)GetValue(CancelTextProperty); }
            set { SetValue(CancelTextProperty, value); }
        }

        public static readonly DependencyProperty CanBeCanceledProperty = DependencyProperty.Register("CanBeCanceled", typeof(bool), typeof(ProgressBox), new PropertyMetadata(true));

        /// <summary>
        /// 是否能够取消
        /// </summary>
        public bool CanBeCanceled
        {
            get { return (bool)GetValue(CanBeCanceledProperty); }
            set { SetValue(CanBeCanceledProperty, value); }
        }



        #region 取消

        public Action CancelAction = null;

        private void OnCancel(object sender, MouseButtonEventArgs e)
        {
            _OnCancel();
        }

        private void _OnCancel()
        {
            if (!Clear()) return;
            if (this.CancelAction != null)
            {
                this.CancelAction();
            }
        }

        #endregion

        #region 超时

        private TimeoutMonitor _timeout;

        public Action TimeoutAction = null;

        private void OnTimeout()
        {
            if (!Clear()) return;

            if (this.TimeoutAction != null)
            {
                this.TimeoutAction();
            }
        }



        #endregion

        public Func<ProgressBox,bool> ExecuteAction = null;


        private object _syncObject = new object();
        private bool _end = false;

        public override void Rendered()
        {
            if (ExecuteAction != null)
            {
                if(!ExecuteAction(this))
                {
                    //返回false，表示没有执行成功
                    return;
                }
            }

            if(this.CanBeCanceled)
            {
                this.cancel.IsEnabled = true;
            }

            if (this.UpdatedTimeout > 0)
            {
                _timeout = new TimeoutMonitor(OnTimeout);
                _timeout.Start(this.UpdatedTimeout * 1000);
            }
        }

        public Action CompletedAction = null;

        private void OnCompleted()
        {
            if (!Clear()) return;

            if (this.CompletedAction != null)
            {
                this.CompletedAction();
            }
        }

        /// <summary>
        /// 更新进度条信息
        /// </summary>
        /// <param name="value"></param>
        /// <param name="description"></param>
        public void UpdateProgress(double value, string description = null)
        {
            this.progress.Value = value;
            this.progress.Description = description ?? string.Empty;

            if (value >= 1)
            {
                OnCompleted();
            }
        }

        public void UpdateDescription(string description)
        {
            this.progress.Description = description;
        }

        public bool IsCancel
        {
            get;
            private set;
        }

        public void Cancel()
        {
            this.IsCancel = false;
            UpdateProgress(0, string.Empty);
        }


        /// <summary>
        /// 更新的超时时间，每次更新的间隔不能超过该时间，否则自动取消行为，单位：秒,0表示不超时
        /// </summary>
        public int UpdatedTimeout
        {
            get;
            set;
        }


        private bool Clear()
        {
            lock (_syncObject)
            {
                if (_end) return false;
                if (_timeout != null)
                {
                    _timeout.Dispose();
                    _timeout = null;
                }
                _end = true;
            }
            return true;
        }
    }
}
