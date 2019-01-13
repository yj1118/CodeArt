using CodeArt.WPF.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CodeArt.WPF
{
    public class MouseDrag
    {
        #region 静态变量

        /// <summary>
        /// 当前正在移动的对象，同一时间只能一个对象移动
        /// </summary>
        private static UIElement _current;


        #endregion

        private bool _isCanvas;

        private FrameworkElement _target;

        /// <summary>
        /// 移动所在的容器
        /// </summary>
        private UIElement _container;

        private Action<UIElement> _onDragStart;

        public bool Disabled
        {
            get;
            set;
        }

        public MouseDrag(UIElement container, FrameworkElement target, Action<UIElement> onDragStart)
        {
            _container = container;
            _isCanvas = container is Canvas;
            _target = target;
            _target.PreviewMouseLeftButtonDown += OnDragStart;
            _target.PreviewMouseMove += OnDraging;
            _target.PreviewMouseLeftButtonUp += OnDragEnd;

            _onDragStart = onDragStart;
        }


        private Point GetTargetPosition()
        {
            if (_isCanvas)
            {
                return new Point(Canvas.GetLeft(_target), Canvas.GetTop(_target));
            }
            return new Point(InkCanvas.GetLeft(_target), InkCanvas.GetTop(_target));
        }

        /// <summary>
        /// 获得鼠标当前位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private Point GetMousePosition(MouseEventArgs e)
        {
            //由于设置Left后，元素的绘制速度比不上程序运行速度，所以我们要根据固定的容器来计算鼠标的坐标，而不是根据当前card对象
            return e.GetPosition(_container);
        }

        private (Point Mouse, Point Target) _startContext;  //鼠标的位置、被移动的对象的位置

        private void OnDragStart(object sender, MouseButtonEventArgs e)
        {
            if (this.Disabled) return;
            _current = _target;
            _startContext = (GetMousePosition(e), GetTargetPosition());
            _onDragStart(_target);
        }

        private void OnDraging(object sender, MouseEventArgs e)
        {
            if (this.Disabled) return;
            if (_current != sender) return;

            if ((e.LeftButton != MouseButtonState.Pressed))
            {
                return;
            }

            var target = sender as UIElement;
            var currentMouse = GetMousePosition(e);
            var offset = currentMouse - _startContext.Mouse;

            var position = _startContext.Target;
            var newPosition = new Point(position.X + offset.X, position.Y + offset.Y);

            if (_isCanvas)
            {
                Canvas.SetLeft(target, newPosition.X);
                Canvas.SetTop(target, newPosition.Y);
            }
            else
            {
                InkCanvas.SetLeft(target, newPosition.X);
                InkCanvas.SetTop(target, newPosition.Y);
            }

            _startContext.Mouse = currentMouse;
            _startContext.Target = newPosition;
        }

        private static void OnDragEnd(object sender, MouseButtonEventArgs e)
        {
            if (_current != sender) return;
            _current = null;
        }

        #region 对外API

        /// <summary>
        /// 获取对象目前的坐标
        /// </summary>
        /// <returns></returns>
        public Point GetPosition()
        {
            return GetTargetPosition();
        }

        public void To(Point position)
        {
            if (_isCanvas)
            {
                Canvas.SetLeft(_target, position.X);
                Canvas.SetTop(_target, position.Y);
            }
            else
            {
                InkCanvas.SetLeft(_target, position.X);
                InkCanvas.SetTop(_target, position.Y);
            }
        }

        #endregion

    }
}
