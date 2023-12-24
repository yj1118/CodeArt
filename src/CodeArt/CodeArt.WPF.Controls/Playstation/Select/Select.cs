using System;
using System.Collections.ObjectModel;
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
using CodeArt.DTO;

namespace CodeArt.WPF.Controls.Playstation
{
    public class Select : Input
    {
        public Select()
        {
            this.DefaultStyleKey = typeof(Select);    
        }

        /// <summary>
        /// 按钮上的提示
        /// </summary>
        public static readonly DependencyProperty TipProperty = DependencyProperty.Register("Tip", typeof(string), typeof(Select));

        public string Tip
        {
            get
            {
                return (string)GetValue(TipProperty);
            }
            set
            {
                SetValue(TipProperty, value);
            }
        }


        public static readonly DependencyProperty ItemImageSrcProperty = DependencyProperty.Register("ItemImageSrc", typeof(string), typeof(Select),new PropertyMetadata("/Playstation/Select/Images/default.png"));

        public string ItemImageSrc
        {
            get
            {
                return (string)GetValue(ItemImageSrcProperty);
            }
            set
            {
                SetValue(ItemImageSrcProperty, value);
            }
        }

        public static readonly DependencyProperty CountProperty = DependencyProperty.Register("Count", typeof(int), typeof(Select));

        /// <summary>
        /// 被选中的项的数量
        /// </summary>
        public int Count
        {
            get
            {
                return (int)GetValue(CountProperty);
            }
            private set
            {
                SetValue(CountProperty, value);
            }
        }

        /// <summary>
        /// 被选后显示的文本格式
        /// </summary>
        public static readonly DependencyProperty CountFormatProperty = DependencyProperty.Register("CountFormat", typeof(string), typeof(Select));

        public string CountFormat
        {
            get
            {
                return (string)GetValue(CountFormatProperty);
            }
            set
            {
                SetValue(CountFormatProperty, value);
            }
        }


        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(ControlTemplate), typeof(Select));

        public ControlTemplate ItemTemplate
        {
            get
            {
                return (ControlTemplate)GetValue(ItemTemplateProperty);
            }
            set
            {
                SetValue(ItemTemplateProperty, value);
            }
        }


        public static readonly DependencyProperty LoadItemsProperty = DependencyProperty.Register("LoadItems", typeof(Func<IEnumerable<TextItem>>), typeof(Select));

        /// <summary>
        /// 加载项的方法
        /// </summary>
        public Func<IEnumerable<TextItem>> LoadItems
        {
            get
            {
                return (Func<IEnumerable<TextItem>>)GetValue(LoadItemsProperty);
            }
            set
            {
                SetValue(LoadItemsProperty, value);
            }
        }

        /// <summary>
        /// 最多选择几项，如果为1，则是单选
        /// </summary>
        public static readonly DependencyProperty MaxCountProperty = DependencyProperty.Register("MaxCount", typeof(int), typeof(Select),new PropertyMetadata(1));

        /// <summary>
        /// 最多选择几项，如果为1，则是单选
        /// </summary>
        public int MaxCount
        {
            get
            {
                return (int)GetValue(MaxCountProperty);
            }
            set
            {
                SetValue(MaxCountProperty, value);
            }
        }


        /// <summary>
        /// 最多选择几项，如果为1，则是单选
        /// </summary>
        public static readonly DependencyProperty MinCountProperty = DependencyProperty.Register("MinCount", typeof(int), typeof(Select), new PropertyMetadata(0));

        /// <summary>
        /// 至少选几项
        /// </summary>
        public int MinCount
        {
            get
            {
                return (int)GetValue(MinCountProperty);
            }
            set
            {
                SetValue(MinCountProperty, value);
            }
        }

        /// <summary>
        /// 是否为单选
        /// </summary>
        public bool IsRadio
        {
            get
            {
                return this.MaxCount <= 1;
            }
        }

        /// <summary>
        /// 选择时的容器宽度，与ItemWidth配合，可以达到 repeater 的效果
        /// </summary>
        public static readonly DependencyProperty ItemContainerWidthProperty = DependencyProperty.Register("ItemContainerWidth", typeof(double), typeof(Select), new PropertyMetadata(double.NaN));

        public double ItemContainerWidth
        {
            get
            {
                return (double)GetValue(ItemContainerWidthProperty);
            }
            set
            {
                SetValue(ItemContainerWidthProperty, value);
            }
        }


        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register("ItemWidth", typeof(double), typeof(Select), new PropertyMetadata(double.NaN));

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

        private ListBox _list;
        private Button _button;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _list = GetTemplateChild("list") as ListBox;
            _list.DataContext = this;

            _button = GetTemplateChild("button") as Button;
            _button.MouseUp += OnOpenSelectItems;
        }

        private SelectItemsPage _items;

        private void OnOpenSelectItems(object sender, MouseButtonEventArgs e)
        {
            if (Work.Current == null) return;
            _items = new SelectItemsPage();
            _items.Owner = this;
            _items.Content = this.ItemTemplate;
            _items.LoadItems = this.LoadItems;
            _items.ItemContainerWidth = this.ItemContainerWidth;
            _items.ItemWidth = this.ItemWidth;
            Work.Current.Go(_items, () =>
            {
                _items.MaxCount = this.MaxCount;
                _items.MinCount = this.MinCount;
                _items.Set(this.Get());
            });
        }

        #region 选择控制区



        //internal void ClearItems()
        //{
        //    _items?.ClearItems();
        //}

        //internal void AddItem(TextItem data)
        //{
        //    _items?.AddItem(data);
        //}

        //internal void RemoveItem(TextItem data)
        //{
        //    _items?.RemoveItem(data);
        //}

        public void Set(TextItem data)
        {
            var datas = new ObservableCollection<TextItem>() { data };
            Set(datas);
        }

        public void Set(IEnumerable<TextItem> datas)
        {
            _list.ItemsSource = datas;
            this.Count = datas.Count();

            RaiseValueChanged(datas, new InputValueChangedEventArgs(datas));
        }

        public IEnumerable<TextItem> Get()
        {
            return _list.ItemsSource as IEnumerable<TextItem> ?? Array.Empty<TextItem>();
        }

        public override void Clear()
        {
            Set(Array.Empty<TextItem>());
        }


        //internal bool ContainsItem(TextItem data)
        //{
        //    if (_list == null || data == null) return false;
        //    var datas = _list.ItemsSource as IEnumerable<TextItem>;
        //    if (datas == null) return false;
        //    return datas.FirstOrDefault((t) =>
        //    {
        //        return t.Value.Equals(data.Value);
        //    }) != null;
        //}

        #endregion




        public override void Write(DTObject data)
        {
            var result = this.Get();
            if(this.IsRadio)
            {
                var item = result.FirstOrDefault();
                if(item != null)
                {
                    DTObject d = DTObject.Create();
                    d["text"] = item.Text;
                    d["value"] = item.Value;
                    data.SetObject(this.MemberName, d);
                }
            }
            else
            {
                data.Push(this.MemberName, result, (d, item) =>
                 {
                     d["text"] = item.Text;
                     d["value"] = item.Value;
                 });
            }
        }

        public override void Read(DTObject data)
        {
            //todo
            throw new NotImplementedException();
        }


        public override void Validate(ValidationResult result)
        {
            var value = this.Get();
            if (this.Required && value.Count() == 0) result.AddError(string.Format("{0}{1}", Strings.PleaseEnter, this.Label));
            if (value.Count() < this.MinCount) result.AddError(string.Format(Strings.CanNotBeLessItems, this.Label, this.MinCount));
            if (this.MaxCount > 0 && value.Count() > this.MaxCount) result.AddError(string.Format(Strings.CanNotBeMoreItems, this.Label, this.MaxCount));
        }

    }
}