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

using CodeArt.WPF.UI;
using CodeArt.Util;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace CodeArt.WPF.Controls.Playstation
{
    /// <summary>
    /// ListText.xaml 的交互逻辑
    /// </summary>
    public partial class ListText : UserControl
    {
        public ListText()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ImageHeightProperty = DependencyProperty.Register("ImageHeight", typeof(double), typeof(ListText));

        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        public static readonly DependencyProperty ImageWidthProperty = DependencyProperty.Register("ImageWidth", typeof(double), typeof(ListText));

        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        public static readonly DependencyProperty TextMarginProperty = DependencyProperty.Register("TextMargin", typeof(Thickness), typeof(ListText), new PropertyMetadata(new Thickness(15, 0, 0, 0)));

        public Thickness TextMargin
        {
            get { return (Thickness)GetValue(TextMarginProperty); }
            set { SetValue(TextMarginProperty, value); }
        }


        public static readonly DependencyProperty ShowModeProperty = DependencyProperty.Register("ShowMode", typeof(RangeMode), typeof(ListText), new PropertyMetadata(RangeMode.Border));

        /// <summary>
        /// 区域的显示模式
        /// </summary>
        public RangeMode ShowMode
        {
            get { return (RangeMode)GetValue(ShowModeProperty); }
            set { SetValue(ShowModeProperty, value); }
        }

        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register("ItemWidth", typeof(double), typeof(ListText), new PropertyMetadata(double.NaN));

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


        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register("ItemHeight", typeof(double), typeof(ListText), new PropertyMetadata(double.NaN));

        public double ItemHeight
        {
            get
            {
                return (double)GetValue(ItemHeightProperty);
            }
            set
            {
                SetValue(ItemHeightProperty, value);
            }
        }


        public ObservableCollection<ListTextItem> ItemsSource
        {
            get
            {
                return this.list.ItemsSource as ObservableCollection<ListTextItem>;
            }
            set
            {
                if (this.list.ItemsSource != null)
                {
                    (this.list.ItemsSource as ObservableCollection<ListTextItem>).CollectionChanged -= ValueCollectionChanged;
                }
                this.list.ItemsSource = value;
                value.CollectionChanged -= ValueCollectionChanged;
                value.CollectionChanged += ValueCollectionChanged;
            }
        }

        private void ValueCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_selected == null) return;

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ListTextItem removedItem in e.OldItems)
                {
                    if (removedItem.Value.Equals(_selected.Value))
                    {
                        this.Value = null; //清空
                        return;
                    }
                }
            }
            //其他操作待完善
        }

        private ListTextItem _selected;

        public object Value
        {
            get
            {
                return _selected?.Value;
            }
            set
            {
                if (value == null)
                {
                    //当赋值value为null时，需要清空选项
                    if (_selected == null) return;
                    var old = _selected;
                    if (old != null) old.IsSelected = false;
                    _selected = null;
                    if (this.SelectChanged != null)
                    {
                        this.SelectChanged(this, new ListTextSelectChangedEventArgs(null, old));
                    }
                }
                else
                {
                    //设置值
                    if (value.Equals(_selected?.Value)) return;
                    var @new = FindItem(value);
                    if (@new == null) throw new InvalidOperationException(string.Format(Strings.NoItemWithValue, value));
                    @new.IsSelected = true;

                    var old = _selected;
                    if (old != null) old.IsSelected = false;
                    _selected = @new;

                    if (this.SelectChanged != null)
                    {
                        this.SelectChanged(this, new ListTextSelectChangedEventArgs(@new, old));
                    }
                }
            }
        }

        public event ListTextSelectChangedEventHandler SelectChanged;


        private ListTextItem FindItem(object value)
        {
            if (this.ItemsSource == null) return null;
            return this.ItemsSource.FirstOrDefault((t) =>
            {
                return t.Value.Equals(value);
            });
        }

        private void SelectItem(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext as ListTextItem;
            this.Value = item.Value;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
    }
}
