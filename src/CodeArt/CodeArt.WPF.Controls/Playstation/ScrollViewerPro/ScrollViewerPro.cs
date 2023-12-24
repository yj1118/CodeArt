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
using System.Windows.Controls.Primitives;

namespace CodeArt.WPF.Controls.Playstation
{
    public class ScrollViewerPro : ScrollViewer
    {
        public ScrollBar VerticalScrollBar
        {
            get;
            private set;
        }

        public Thumb VerticalScrollThumb
        {
            get;
            private set;
        }


        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ScrollByMouseMoveProperty = DependencyProperty.Register("ScrollByMouseMove", typeof(bool), typeof(ScrollViewer), new PropertyMetadata(false));

        public bool ScrollByMouseMove
        {
            get
            {
                return (bool)GetValue(ScrollByMouseMoveProperty);
            }
            set
            {
                SetValue(ScrollByMouseMoveProperty, value);
            }
        }

        public ScrollViewerPro()
        {
            this.DefaultStyleKey = typeof(ScrollViewerPro);
        }

        public bool IsVerticalScrollBarAtBottom
        {
            get
            {
                bool isAtButtom = false;

                // get the vertical scroll position
                double dVer = this.VerticalOffset;

                //get the vertical size of the scrollable content area
                double dViewport = this.ViewportHeight;

                //get the vertical size of the visible content area
                double dExtent = this.ExtentHeight;

                if (dVer != 0)
                {
                    if (dVer + dViewport == dExtent)
                    {
                        isAtButtom = true;
                    }
                    else
                    {
                        isAtButtom = false;
                    }
                }
                else
                {
                    isAtButtom = false;
                }

                if (this.VerticalScrollBarVisibility == ScrollBarVisibility.Disabled
                    || this.VerticalScrollBarVisibility == ScrollBarVisibility.Hidden)
                {
                    isAtButtom = true;
                }

                return isAtButtom;
            }
        }

        /// <summary>
        /// 滚动条底部的偏移量
        /// </summary>
        public double VerticalBottomOffset
        {
            get
            {
                return this.ExtentHeight - this.ViewportHeight - this.VerticalOffset;
            }
        }


        public ScrollContentPresenter content;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.VerticalScrollBar = GetTemplateChild("PART_VerticalScrollBar") as ScrollBar;
            this.MakeChildsLoaded(this.VerticalScrollBar);
            this.VerticalScrollThumb = this.VerticalScrollBar.GetChilds<Thumb>().First();

            content = GetTemplateChild("PART_ScrollContentPresenter") as ScrollContentPresenter;

            if (this.ScrollByMouseMove)
            {
                content.MouseLeftButtonDown += OnContentMouseLeftButtonDown;
                content.MouseLeftButtonUp += OnContentMouseLeftButtonUp;
                content.MouseMove += OnContentMouseMove;
            }

            base.ManipulationBoundaryFeedback += ScrollViewerPro_ManipulationBoundaryFeedback;

        }

        private void ScrollViewerPro_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        #region 鼠标拖动也可以改变滚动条

        private bool _enableMove = false;
        private double _distanceX = 0;
        private double _distanceY = 0;

        private double _verticalOffset = 0;
        private double _horizontalOffset = 0;

        private void OnContentMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Mouse.Capture(content);
            _enableMove = true;

            var position = e.GetPosition(content);
            _distanceX = position.X;
            _distanceY = position.Y;

            _verticalOffset = this.VerticalOffset;
            _horizontalOffset = this.HorizontalOffset;
        }

        private void OnContentMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //content.ReleaseMouseCapture();
            _enableMove = false;
        }

        private void OnContentMouseMove(object sender, MouseEventArgs e)
        {
            if(_enableMove)
            {
                var position = e.GetPosition(content);
                var left = position.X - _distanceX;
                var top = position.Y - _distanceY;

                this.ScrollToVerticalOffset(_verticalOffset - top);
                this.ScrollToHorizontalOffset(_horizontalOffset - left);

            }
        }

        #endregion

    }
}