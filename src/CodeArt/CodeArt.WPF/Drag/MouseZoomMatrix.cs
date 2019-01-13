using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CodeArt.WPF
{
    public class MouseZoomMatrix
    {
        #region 静态变量

        /// <summary>
        /// 当前正在移动的对象，同一时间只能一个对象移动
        /// </summary>
        private static FrameworkElement _current;


        #endregion

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


        public MouseZoomMatrix(UIElement container, FrameworkElement zoomTarget)
        {
            _container = container;
            _zoomTarget = zoomTarget;
            _current = _zoomTarget;

            _zoomTarget.MouseWheel += OnZooming;
            this.Disabled = false;
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

            Matrix matrix = ((MatrixTransform)_zoomTarget.RenderTransform).Matrix;

            Point center = new Point(_zoomTarget.ActualWidth / 2, _zoomTarget.ActualHeight / 2);
            center = matrix.Transform(center);

            // 缩放图片
            matrix.ScaleAt(scale, scale, center.X, center.Y);

            ((MatrixTransform)_zoomTarget.RenderTransform).Matrix = matrix;
        }

        private static void OnDragEnd(object sender, MouseButtonEventArgs e)
        {
            if (_current != sender) return;
            _current = null;
        }
    }
}
