using CodeArt.WPF.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CodeArt.WPF
{
    public class MouseDragMatrix
    {
        #region 静态变量

        /// <summary>
        /// 当前正在移动的对象，同一时间只能一个对象移动
        /// </summary>
        private static UIElement _current;


        #endregion

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

        public MouseDragMatrix(UIElement container, FrameworkElement target, Action<UIElement> onDragStart)
        {
            _container = container;
            _target = target;
            _target.PreviewMouseLeftButtonDown += OnDragStart;
            _target.PreviewMouseMove += OnDraging;
            _target.PreviewMouseLeftButtonUp += OnDragEnd;

            _onDragStart = onDragStart;
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
            _startContext = (GetMousePosition(e), new Point(0,0));
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

            Matrix matrix = ((MatrixTransform)target.RenderTransform).Matrix;

            matrix.Translate(offset.X, offset.Y);

            ((MatrixTransform)target.RenderTransform).Matrix = matrix;

            _startContext.Mouse = currentMouse;
        }

        private static void OnDragEnd(object sender, MouseButtonEventArgs e)
        {
            if (_current != sender) return;
            _current = null;
        }
    }
}
