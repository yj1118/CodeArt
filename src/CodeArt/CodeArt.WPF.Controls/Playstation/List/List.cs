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
using System.Collections;
using System.Collections.Specialized;

using CodeArt.WPF.UI;
using CodeArt.Util;


namespace CodeArt.WPF.Controls.Playstation
{
    /// <summary>
    /// 当一行显示多列时，会根据单项宽度和最大列数进行自动计算宽度
    /// </summary>
    public class List : ListBox
    {
        public List()
        {
            this.DefaultStyleKey = typeof(List);
        }

        public static readonly DependencyProperty FixedWidthProperty = DependencyProperty.Register("FixedWidth", typeof(bool), typeof(List), new PropertyMetadata(false, OnFixedWidthChanged));

        /// <summary>
        /// 是否为固定宽度，在一行显示多个列的情况下，如果不希望动态计算宽度，可以将该选项设为true
        /// </summary>
        public bool FixedWidth
        {
            get
            {
                return (bool)GetValue(FixedWidthProperty);
            }
            set
            {
                SetValue(FixedWidthProperty, value);
            }
        }

        private static void OnFixedWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as List).CalculateWidth();
        }


        public static readonly DependencyProperty RowItemCountProperty = DependencyProperty.Register("RowItemCount", typeof(int), typeof(List), new PropertyMetadata(0, OnRowItemCountChanged));

        /// <summary>
        /// 每行最大显示的个数
        /// </summary>
        public int RowItemCount
        {
            get
            {
                return (int)GetValue(RowItemCountProperty);
            }
            set
            {
                SetValue(RowItemCountProperty, value);
            }
        }

        private static void OnRowItemCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as List).CalculateWidth();
        }


        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register("ItemWidth", typeof(double), typeof(List), new PropertyMetadata(double.NaN, OnItemWidthChanged));

        public double ItemWidth
        {
            get
            {
                return (double)GetValue(ItemWidthProperty);
            }
            set
            {
                SetValue(ItemWidthProperty, value);
            }
        }

        private static void OnItemWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as List).CalculateWidth();
        }

        public static readonly DependencyProperty NoDataTextProperty = DependencyProperty.Register("NoDataText", typeof(string), typeof(List), new PropertyMetadata(Strings.NoData));

        public string NoDataText
        {
            get
            {
                return (string)GetValue(NoDataTextProperty);
            }
            set
            {
                SetValue(NoDataTextProperty, value);
            }
        }


        public static readonly DependencyProperty IsNoDataProperty = DependencyProperty.Register("IsNoData", typeof(bool), typeof(List), new PropertyMetadata(true));

        public bool IsNoData
        {
            get
            {
                return (bool)GetValue(IsNoDataProperty);
            }
            set
            {
                SetValue(IsNoDataProperty, value);
            }
        }

        public static readonly DependencyProperty AutoShowNoDataProperty = DependencyProperty.Register("AutoShowNoData", typeof(bool), typeof(List), new PropertyMetadata(true));

        public bool AutoShowNoData
        {
            get
            {
                return (bool)GetValue(AutoShowNoDataProperty);
            }
            set
            {
                SetValue(AutoShowNoDataProperty, value);
            }
        }

        public event EventHandler ItemsSourceChanged;


        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            CalculateWidth();

            this.IsNoData = newValue.IsEmpty();
            var ncc = newValue as INotifyCollectionChanged;
            if(ncc != null)
            {
                ncc.CollectionChanged += Ncc_CollectionChanged;
            }

            if (ItemsSourceChanged != null)
                ItemsSourceChanged(this, EventArgs.Empty);
        }

        private void Ncc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.IsNoData = (sender as IEnumerable).IsEmpty();
        }

        private void CalculateWidth()
        {
            if (this.RowItemCount == 0) return; //不是多列，不启用
            if(this.FixedWidth)
            {
                this.Width = this.RowItemCount * this.ItemWidth;
            }
            else
            {
                var items = this.ItemsSource;
                if (items == null) return;
                var count = items.GetCount();
                this.Width = count < this.RowItemCount ? count * this.ItemWidth : this.RowItemCount * this.ItemWidth;
            }
        }


        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty
            = DependencyProperty.Register("VerticalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(List), new PropertyMetadata(ScrollBarVisibility.Auto));

        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get
            {
                return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty);
            }
            set
            {
                SetValue(VerticalScrollBarVisibilityProperty, value);
            }
        }

        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty
           = DependencyProperty.Register("HorizontalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(List), new PropertyMetadata(ScrollBarVisibility.Auto));

        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get
            {
                return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty);
            }
            set
            {
                SetValue(HorizontalScrollBarVisibilityProperty, value);
            }
        }

        /// <summary>
        /// 启用外部滚动，这个属性用于当list嵌套在ScrollViewer内时，解决外部滚动条不动的错误
        /// </summary>
        public static readonly DependencyProperty EnableExternalScrollBarProperty
           = DependencyProperty.Register("EnableExternalScrollBar", typeof(bool), typeof(List), new PropertyMetadata(false, EnableExternalScrollBarChanged));

        public bool EnableExternalScrollBar
        {
            get
            {
                return (bool)GetValue(EnableExternalScrollBarProperty);
            }
            set
            {
                SetValue(EnableExternalScrollBarProperty, value);
            }
        }

        private static void EnableExternalScrollBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var list = d as List;
            var value = (bool)e.NewValue;
            if(value)
            {
                list.PreviewMouseWheel += OnMouseWheel;
            }
            else
            {
                list.PreviewMouseWheel -= OnMouseWheel;
            }
        }

        private static void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var list = sender as List;
            var args = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            args.RoutedEvent = UIElement.MouseWheelEvent;
            args.Source = sender;
            list.RaiseEvent(args);
        }
    }
}