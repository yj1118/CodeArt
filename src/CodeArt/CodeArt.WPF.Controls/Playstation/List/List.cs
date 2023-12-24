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
using System.Collections.ObjectModel;

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
            if (ncc != null)
            {
                ncc.CollectionChanged += Ncc_CollectionChanged;
            }

            if (ItemsSourceChanged != null)
                ItemsSourceChanged(this, EventArgs.Empty);
        }

        private void Ncc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.IsNoData = (sender as IEnumerable).IsEmpty();
            CalculateWidth();
        }

        private void CalculateWidth()
        {
            if (this.RowItemCount == 0) return; //不是多列，不启用
            if (this.FixedWidth)
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
            if (value)
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

        public static readonly DependencyProperty VirtualizedProperty = DependencyProperty.Register("Virtualized", typeof(bool), typeof(List), new PropertyMetadata(false));

        /// <summary>
        /// 是否虚拟化列表
        /// </summary>
        public bool Virtualized
        {
            get
            {
                return (bool)GetValue(VirtualizedProperty);
            }
            set
            {
                SetValue(VirtualizedProperty, value);
            }
        }

        private ScrollViewerPro _scrollViewer;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _scrollViewer = this.GetChilds<ScrollViewerPro>().FirstOrDefault();
            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollChanged -= OnScrollChanged;
                _scrollViewer.ScrollChanged += OnScrollChanged;
            }
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!this.Virtualized) return;
            if (_ignoreOnceScrollChanged)
            {
                //忽略一次
                _ignoreOnceScrollChanged = false;
                return;
            }

            int pageSize = (int)(_scrollViewer.ViewportHeight / _averageItemHeight);
            if (pageSize == 0) return;

            UpdateItemsVisible();

            if (e.VerticalChange > 0)
            {
                //滚动条向下移动
                var bottomOffset = _scrollViewer.VerticalBottomOffset; //由于当删除元素还有，VerticalBottomOffset不会及时更新，所以我们自己更新
                var verticalOffset = _scrollViewer.VerticalOffset;
                double decreaseHeight = 0;
                if (verticalOffset > (_averageItemHeight * pageSize))
                {
                    //如果距离顶部大于1个页，那么尝试删除一次
                    decreaseHeight = RemoveTopPages();
                    bottomOffset -= decreaseHeight;
                }

                if (bottomOffset < (_averageItemHeight * pageSize))
                {
                    //如果距离底部小于1个页，那么加载一次
                    LoadNextPage();
                }

                if (decreaseHeight > 0)
                {
                    //修正滚动条
                    _scrollViewer.ScrollToVerticalOffset(verticalOffset - decreaseHeight);
                    _ignoreOnceScrollChanged = true;
                }

            }
            else if (e.VerticalChange < 0)
            {
                //滚动条向上移动
                var verticalOffset = _scrollViewer.VerticalOffset;
                var bottomOffset = _scrollViewer.VerticalBottomOffset;
                double increaseHeight = 0;

                if (bottomOffset > (_averageItemHeight * pageSize))
                {
                    //如果距离底部大于1个页，那么尝试删除一次
                    RemoveBottomPages();
                }

                if (verticalOffset < (_averageItemHeight * pageSize))
                {
                    //如果距离顶部小于1个页，那么加载一次
                    increaseHeight += LoadPrevPage();
                }

                if (increaseHeight != 0)
                {
                    //修正滚动条
                    _scrollViewer.ScrollToVerticalOffset(verticalOffset + increaseHeight);
                    _ignoreOnceScrollChanged = true;
                }
            }
        }

        private bool _ignoreOnceScrollChanged = false;


        private void UpdateItemsVisible()
        {
            foreach (IVirtualizable item in this.Items)
            {
                var listBoxItem = (FrameworkElement)this.ItemContainerGenerator.ContainerFromItem(item);
                item.IsVisible = listBoxItem.IsChildVisibleInParent(_scrollViewer);
            }
        }

        private double RemovePages(Func<int,List<VirtualizableItem>> getPage)
        {
            double decreaseHeight = 0;
            while (true)
            {
                //先得到0页的内容
                var items = getPage(0);
                if (items == null) break;
                if (!IsAllInvisible(items)) break;

                //如果0页的内容为全部不可见，那么再看1页是否也是全部不可见，如果是，就真正删除0页，否则保留0页
                //这样做，主要是为了让滚动条之外的内容，始终保留1页,有足够的可移动空间让我们定位，进而平滑过渡
                var nextItems = getPage(1);
                if (nextItems == null) break;
                if (!IsAllInvisible(nextItems)) break;

                var existItems = this.ItemsSource as ObservableCollection<VirtualizableItem>;
                foreach (var item in items)
                {
                    if (existItems.Remove(item))
                    {
                        decreaseHeight += _averageItemHeight;
                    }
                }
            }

            return decreaseHeight;
        }


        /// <summary>
        /// 移除顶部不可见的页
        /// </summary>
        private double RemoveTopPages()
        {
            return RemovePages(GetPositivePage);
        }

        private double RemoveBottomPages()
        {
            return RemovePages(GetReversePage);
        }

        /// <summary>
        /// 判断对象集合是否全部都不可见
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private bool IsAllInvisible(IEnumerable<VirtualizableItem> items)
        {
            foreach(var item in items)
            {
                if (item.IsVisible) return false;
            }
            return true;
        }


        /// <summary>
        /// 从正向获取页信息
        /// </summary>
        private List<VirtualizableItem> GetPositivePage(int pageIndex)
        {
            var startIndex = pageIndex * _pageSize;
            var count = startIndex + _pageSize;

            var existItems = this.ItemsSource as ObservableCollection<VirtualizableItem>;
            List<VirtualizableItem> items = null;
            for (var i = startIndex; i < count; i++)
            {
                if (i < existItems.Count)
                {
                    if (items == null) items = new List<VirtualizableItem>(_pageSize);
                    items.Add(existItems[i]);
                }
                else
                    break;
            }
            return items;
        }

        /// <summary>
        /// 从反向获取页信息
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        private List<VirtualizableItem> GetReversePage(int pageIndex)
        {
            var existItems = this.ItemsSource as ObservableCollection<VirtualizableItem>;
            if (existItems.Count == 0) return null;
            var lastItem = existItems[existItems.Count - 1];

            var lastIndex = lastItem.Index;
            var lastPageIndex = lastIndex / _pageSize;
            if (lastIndex % _pageSize > 0) lastPageIndex++;

            var targetPageIndex = lastPageIndex - pageIndex;

            var startIndex = (targetPageIndex -1) * _pageSize;
            var count = startIndex + _pageSize;

            List<VirtualizableItem> items = null;
            for (var i = startIndex; i < count; i++)
            {
                if (i < existItems.Count)
                {
                    if (items == null) items = new List<VirtualizableItem>(_pageSize);
                    items.Add(existItems[i]);
                }
                else
                    break;
            }
            return items;
        }


        private void LoadNextPage()
        {
            if (_pageSize == 0) return;
            if (this.ItemsSource == null) InitItemsSource();

            var existItems = this.ItemsSource as ObservableCollection<VirtualizableItem>;
            int pageIndex = 0;
            if (existItems.Count > 0)
            {
                var lastItem = existItems.Last();
                var lastIndex = lastItem.Index + 1;
                pageIndex = lastIndex / _pageSize;
                if (lastIndex % _pageSize > 0) pageIndex++;
            }
            var items = _getItems(pageIndex, _pageSize);
            if (items == null || items.Count() == 0) return;
            foreach (var item in items)
            {
                existItems.Add(item);
            }
        }

        private double LoadPrevPage()
        {
            double increaseHeight = 0;
            if (_pageSize == 0) return increaseHeight;
            if (this.ItemsSource == null) InitItemsSource();

            var existItems = this.ItemsSource as ObservableCollection<VirtualizableItem>;
            int pageIndex = 0;
            if (existItems.Count > 0)
            {
                var firstItem = existItems[0] as VirtualizableItem;
                var index = firstItem.Index;
                pageIndex = index / _pageSize - 1;
            }
            if (pageIndex < 0) return increaseHeight;
            var items = _getItems(pageIndex, _pageSize);
            if (items == null || items.Count() == 0) return increaseHeight;
            var itemIndex = 0;
            foreach (var item in items)
            {
                existItems.Insert(itemIndex, item);
                itemIndex++;
                increaseHeight += _averageItemHeight;
            }
            return increaseHeight;
        }

        private void InitItemsSource()
        {
            this.ItemsSource = null;
            this.ItemsSource = new ObservableCollection<VirtualizableItem>();
        }

        private double _averageItemHeight;

        private Func<int, int, IEnumerable<VirtualizableItem>> _getItems;

        private int _pageSize = 0;

        /// <summary>
        /// 设置翻页模式，该模式会启动虚拟化提高性能
        /// </summary>
        /// <param name="averageItemHeight"></param>
        /// <param name="getItems">int pageIndex(从0为下标),int pageSize</param>
        public void SetPageMode(double averageItemHeight, Func<int, int, IEnumerable<VirtualizableItem>> getItems)
        {
            this.Virtualized = true;
            //因为ScrollViewer.CanContentScroll = "False"为物理单元，即像素滚动，当数据很多时，即使开启了虚化化，因计算太耗性能，界面一样卡顿。
            //因此设置ScrollViewer.CanContentScroll = "True"
            _scrollViewer.CanContentScroll = true;

            _averageItemHeight = averageItemHeight;
            _getItems = getItems;
            _pageSize = (int)(_scrollViewer.ViewportHeight / _averageItemHeight) + 1;//加1的原因是有可能可以容纳2.5个，这时候2个就不够，需要加载3个
            LoadNextPage();
        }

        /// <summary>
        /// 刷新列表，会以pageIndex来刷新
        /// </summary>
        public void Load(int pageIndex)
        {
            if (_pageSize == 0) return;
            if (this.ItemsSource == null) InitItemsSource();

            var items = _getItems(pageIndex, _pageSize);
            if (items == null || items.Count() == 0) return;

            var existItems = this.ItemsSource as ObservableCollection<VirtualizableItem>;
            existItems.Clear();

            foreach (var item in items)
            {
                existItems.Add(item);
            }

            if (pageIndex == 0)
                _scrollViewer.ScrollToTop();
        }

        /// <summary>
        /// 当前页码，从0开始
        /// </summary>
        public (int Min, int Max) PageRange
        {
            get
            {
                var existItems = this.ItemsSource as ObservableCollection<VirtualizableItem>;
                if (existItems != null && existItems.Count > 0)
                {
                    var firstItem = existItems.First();
                    var firstIndex = firstItem.Index;
                    var min = firstIndex / _pageSize;

                    var lastItem = existItems.Last();
                    var lastIndex = lastItem.Index;
                    var max = lastIndex / _pageSize;

                    return (min, max);
                }

                return (0, 0);
            }


        }

    }
}