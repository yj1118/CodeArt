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
    /// 可以点击的组件，使用该组件可以避免重复点击的问题
    /// </summary>
    public class ClickControl : Control
    {

        private EventProtector<object> _protector;


        public static readonly DependencyProperty ClickProperty = DependencyProperty.Register("Click", typeof(Action<object, Action>), typeof(ClickControl));

        public Action<object, Action> Click
        {
            get { return (Action<object, Action>)GetValue(ClickProperty); }
            set { SetValue(ClickProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.MouseUp += OnMouseUp;
            _protector = new EventProtector<object>(_OnClick);
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _protector.Start(sender);
        }


        private void _OnClick(object sender)
        {
            if (this.Click == null) _protector.End();
            else
            {
                try
                {
                    this.Click(sender, () =>
                    {
                        _protector.End();
                    });
                }
                catch(Exception ex)
                {
                    _protector.End();
                    throw ex;
                }
            }
        }
    }

    public class ClickContentControl : ContentControl
    {

        private EventProtector<object> _protector;

        public static readonly DependencyProperty ClickProperty = DependencyProperty.Register("Click", typeof(Action<object, Action>), typeof(ClickContentControl));

        public Action<object, Action> Click
        {
            get { return (Action<object, Action>)GetValue(ClickProperty); }
            set { SetValue(ClickProperty, value); }
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.MouseUp += OnMouseUp;
            _protector = new EventProtector<object>(_OnClick);
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _protector.Start(sender);
        }


        private void _OnClick(object sender)
        {
            if (this.Click == null) _protector.End();
            else
            {
                this.Click(sender, () =>
                {
                    _protector.End();
                });
            }
        }
    }

}