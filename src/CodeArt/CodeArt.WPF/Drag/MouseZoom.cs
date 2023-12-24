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
    public class MouseZoom
    {
        #region 静态变量

        /// <summary>
        /// 当前正在移动的对象，同一时间只能一个对象移动
        /// </summary>
        private static UIElement _current;


        #endregion

        private bool _isCanvas;

        private UIElement _target;

        private FrameworkElement _zoomTarget;

        /// <summary>
        /// 缩放所在的容器
        /// </summary>
        private UIElement _container;

        public bool Disabled
        {
            get;
            set;
        }


        public MouseZoom(UIElement container, UIElement target, FrameworkElement zoomTarget)
        {
            _container = container;
            _isCanvas = container is Canvas;
            _target = target;
            _zoomTarget = zoomTarget;
            _target.PreviewMouseLeftButtonDown += OnZoomStart;
            _target.MouseWheel += OnZooming;
            this.Disabled = false;
        }

        private void OnZoomStart(object sender, MouseButtonEventArgs e)
        {
            if (this.Disabled) return;
            _current = _target;
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
        /// 缩放对象目前的大小
        /// </summary>
        /// <returns></returns>
        public (double Width, double Height) GetSize()
        {
            return (_zoomTarget.Width, _zoomTarget.Height);
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

        private void OnZooming(object sender, MouseWheelEventArgs e)
        {
            if (this.Disabled) return;
            if (_current != sender) return;

            var scale = (e.Delta > 0 ? 1.2 : 1 / 1.2);

            var rawWidth = _zoomTarget.ActualWidth;
            var rawHeight = _zoomTarget.ActualHeight;

            var width = rawWidth * scale;
            var height = rawHeight * scale;

            var deltaWidth = width - rawWidth;
            var deltaHeight = height - rawHeight;

            var position = GetTargetPosition();

            _zoomTarget.Width = width;
            _zoomTarget.Height = height;

            if(_isCanvas)
            {
                Canvas.SetLeft(_target, position.X - deltaWidth / 2);
                Canvas.SetTop(_target, position.Y - deltaHeight / 2);
            }
            else
            {
                InkCanvas.SetLeft(_target, position.X - deltaWidth / 2);
                InkCanvas.SetTop(_target, position.Y - deltaHeight / 2);
            }
        }

        private static void OnDragEnd(object sender, MouseButtonEventArgs e)
        {
            if (_current != sender) return;
            _current = null;
        }
    }
}
