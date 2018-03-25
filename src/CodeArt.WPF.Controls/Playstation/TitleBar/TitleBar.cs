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
    /// <summary>
    /// 
    /// </summary>
    public class TitleBar : Control
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(TitleBar));

        /// <summary>
        /// 
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty LogoProperty = DependencyProperty.Register("Logo", typeof(string), typeof(TitleBar));

        /// <summary>
        /// 
        /// </summary>
        public string Logo
        {
            get { return (string)GetValue(LogoProperty); }
            set { SetValue(LogoProperty, value); }
        }


        public static readonly DependencyProperty ShowCloseProperty = DependencyProperty.Register("ShowClose", typeof(bool), typeof(TitleBar), new PropertyMetadata(true));

        /// <summary>
        /// 
        /// </summary>
        public bool ShowClose
        {
            get { return (bool)GetValue(ShowCloseProperty); }
            set { SetValue(ShowCloseProperty, value); }
        }

        public static readonly DependencyProperty ShowKeyboardProperty = DependencyProperty.Register("ShowKeyboard", typeof(bool), typeof(TitleBar), new PropertyMetadata(false));

        /// <summary>
        /// 
        /// </summary>
        public bool ShowKeyboard
        {
            get { return (bool)GetValue(ShowKeyboardProperty); }
            set { SetValue(ShowKeyboardProperty, value); }
        }

        private Image min;
        private Image close;
        private Image keyboard;

        private StackPanel _status;

        public TitleBar()
        {
            this.DefaultStyleKey = typeof(TitleBar);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.min = GetTemplateChild("min") as Image;
            this.close = GetTemplateChild("close") as Image;
            this.keyboard = GetTemplateChild("keyboard") as Image;

            _status = GetTemplateChild("status") as StackPanel;

            this.min.MouseEnter += OnMouseEnter;
            this.min.MouseLeave += OnMouseLeave;
            this.min.MouseUp += Minimized;

            this.close.MouseEnter += OnMouseEnter;
            this.close.MouseLeave += OnMouseLeave;
            this.close.MouseUp += Close;

            this.keyboard.MouseEnter += OnMouseEnter;
            this.keyboard.MouseLeave += OnMouseLeave;
            this.keyboard.MouseUp += OnKeyboard;

        }

        private Window _root;

        public void Init(Window root)
        {
            _root = root;
            _root.SizeChanged += OnRootSizeChanged;

            Maximized();
        }

        private void OnRootSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_root.WindowState == WindowState.Maximized)
            {
                Maximized();
            }
        }

        private void Maximized()
        {
#if !DEBUG
            _root.Topmost = true; //这句话也很重要，否则不能盖住任务栏
#endif
            _root.Hide(); //先调用其隐藏方法 然后再显示出来,这样就会全屏,且任务栏不会出现.如果不加这句 可能会出现假全屏即任务栏还在下面.
            _root.Show();
        }


        private void Minimized(object sender, MouseButtonEventArgs e)
        {
            _root.WindowState = WindowState.Minimized;
        }

        private void OnKeyboard(object sender, MouseButtonEventArgs e)
        {
            TabTipBoard.ClickTabTip();
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            (sender as UIElement).Opacity = 0.5;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            (sender as UIElement).Opacity = 1;
        }

        public Action ExitAction
        {
            get;
            set;
        }


        private void Close(object sender, MouseButtonEventArgs e)
        {
            if(this.ExitAction != null)
            {
                //使用自定义离开的行为
                this.ExitAction();
                return;
            }
            //使用默认的行为
            Work.Current?.Exit();
        }

        private object _syncStatus = new object();

        /// <summary>
        /// 该方法是线程安全的
        /// </summary>
        /// <param name="status"></param>
        public void AddStatus(TitleBarStatus status)
        {
            lock (_syncStatus)
            {
                var g = GetImage(status);
                AddStatus(status, g);
            }
        }

        public void AddStatus(TitleBarStatus status, ImagePro g)
        {
            bool finded = false;
            foreach (FrameworkElement c in _status.Children)
            {
                var s = c.DataContext as TitleBarStatus;
                if (s.Name == status.Name)
                {
                    finded = true;
                    break;
                }
            }

            if (!finded)
                _status.Children.Insert(0, g);
        }

        /// <summary>
        /// 该方法是线程安全的
        /// </summary>
        /// <param name="targetName">需要被替换的状态名称</param>
        /// <param name="status"></param>
        public void ReplaceStatus(string targetName, TitleBarStatus status)
        {
            lock (_syncStatus)
            {
                var g = GetImage(status);

                for (var i = 0; i < _status.Children.Count; i++)
                {
                    var c = _status.Children[i] as FrameworkElement;
                    var s = c.DataContext as TitleBarStatus;
                    if (s.Name == targetName)
                    {
                        _status.Children[i] = g;
                        break;
                    }
                }
            }
        }

        private bool ReplaceStatus(string targetName, ImagePro g)
        {
            for (var i = 0; i < _status.Children.Count; i++)
            {
                var c = _status.Children[i] as FrameworkElement;
                var s = c.DataContext as TitleBarStatus;
                if (s.Name == targetName)
                {
                    _status.Children.RemoveAt(i);
                    _status.Children.Insert(i, g);
                    return true;
                }
            }
            return false;
        }


        private static ImagePro GetImage(TitleBarStatus status)
        {
            var g = new ImagePro()
            {
                Opacity = 0.5,
                Height = status.IconHeight,
                Width = status.IconWidth,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment= VerticalAlignment.Center,
                Margin = new Thickness(5,0,5,0),
                Source = status.Icon,
            };
            g.ToolTip = new ToolTipText() { Description = status.Description };
            g.DataContext = status;
            return g;
        }

        /// <summary>
        /// 该方法是线程安全的
        /// </summary>
        /// <param name="targetName"></param>
        /// <param name="status"></param>
        public void ReplaceOrAddStatus(string targetName, TitleBarStatus status)
        {
            lock (_syncStatus)
            {
                var g = GetImage(status);
                if (!ReplaceStatus(targetName, g))
                {
                    AddStatus(status, g);
                }
            }
        }

        /// <summary>
        /// 该方法是线程安全的
        /// </summary>
        /// <param name="targetName"></param>
        public void RemoveStatus(string targetName)
        {
            lock (_syncStatus)
            {
                for (var i = 0; i < _status.Children.Count; i++)
                {
                    var c = _status.Children[i] as FrameworkElement;
                    var s = c.DataContext as TitleBarStatus;
                    if (s.Name == targetName)
                    {
                        _status.Children.RemoveAt(i);
                    }
                }
            }
        }

        public void ClearStatus()
        {
            lock(_syncStatus)
            {
                _status.Children.Clear();
            }
        }

    }
}