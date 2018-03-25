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
using CodeArt.DTO;

namespace CodeArt.WPF.Controls.Playstation
{
    /// <summary>
    /// SimpleSelect.xaml 的交互逻辑
    /// </summary>
    public partial class SelectSlim : Input
    {
        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register("ItemWidth", typeof(double), typeof(SelectSlim),new PropertyMetadata(double.NaN));

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

        /// <summary>
        /// 最多选择几项，如果为1，则是单选
        /// </summary>
        public static readonly DependencyProperty MaxCountProperty = DependencyProperty.Register("MaxCount", typeof(int), typeof(SelectSlim), new PropertyMetadata(1));

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
        public static readonly DependencyProperty MinCountProperty = DependencyProperty.Register("MinCount", typeof(int), typeof(SelectSlim), new PropertyMetadata(0));

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

        public bool IsRadio
        {
            get
            {
                return this.MaxCount <= 1;
            }
        }


        public SelectSlim()
        {
            InitializeComponent();
        }

        private ListBox _list;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _list = GetTemplateChild("list") as ListBox;
        }

        public void SetItems(IEnumerable<TextItem> items)
        {
            _list.ItemsSource = null;
            _list.ItemsSource = items;
        }

        public void Set(TextItem value)
        {
            Set(new TextItem[] { value });
        }

        public void Set(object value)
        {
            Set(new TextItem(value));
        }

        private IEnumerable<TextItem> GetValues()
        {
            var items = _list.ItemsSource as IEnumerable<TextItem>;
            if (items == null) return Array.Empty<TextItem>();
            return items.Where((t) => t.IsSelected);
        }

        public bool ContainsValue(object value)
        {
            var target = GetValues().FirstOrDefault((t) =>
            {
                return t.Value.Equals(value);
            });

            return target != null;
        }


        public void Set(IEnumerable<TextItem> values)
        {
            this.ClearValue();

            var items = _list.ItemsSource as IEnumerable<TextItem>;
            foreach (var item in items)
            {
                var target = values.FirstOrDefault((t) =>
                {
                    return item.Value.Equals(t.Value);
                });

                if (target != null)
                {
                    item.IsSelected = true;
                    break;
                }
            }

            RaiseValueChanged();
        }

        private void RaiseValueChanged()
        {
            if (this.IsRadio)
            {
                RaiseValueChanged(this, new InputValueChangedEventArgs(GetValues().FirstOrDefault()));
            }
            else
            {
                RaiseValueChanged(this, new InputValueChangedEventArgs(GetValues()));
            }
        }

        private void Add(TextItem value)
        {
            var items = _list.ItemsSource as IEnumerable<TextItem>;
            var target = items.FirstOrDefault((t) =>
            {
                return value.Value.Equals(t.Value);
            });

            if (target == null) return;
            target.IsSelected = true;

            RaiseValueChanged();
        }

        private void Remove(TextItem value)
        {
            var items = _list.ItemsSource as IEnumerable<TextItem>;
            var target = items.FirstOrDefault((t) =>
            {
                return value.Value.Equals(t.Value);
            });

            if (target == null) return;
            target.IsSelected = false;

            RaiseValueChanged();
        }

        private void ClearValue()
        {
            var items = _list.ItemsSource as IEnumerable<TextItem>;
            foreach (var item in items)
            {
                item.IsSelected = false;
            }
        }

        public override void Clear()
        {
            this.Set(Array.Empty<TextItem>());
        }

        public override void Write(DTObject data)
        {
            var result = GetValues(); ;
            if (this.IsRadio)
            {
                var item = result.FirstOrDefault();
                if (item != null)
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

        private void Item_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var s = sender as FrameworkElement;
            var data = s.DataContext as TextItem;
            var isSelected = !data.IsSelected;

            if(this.IsRadio)
            {
                if (isSelected)
                    this.Set(data);
                else
                    this.Remove(data);
            }
            else
            {
                if (isSelected)
                    this.Add(data);
                else
                    this.Remove(data);
            }
            
        }

        public override void Validate(ValidationResult result)
        {
            var value = GetValues();
            if (this.Required && value.Count() == 0) result.AddError(string.Format("{0}{1}", Strings.PleaseEnter, this.Label));
            if (value.Count() < this.MinCount) result.AddError(string.Format(Strings.CanNotBeLessItems, this.Label, this.MinCount));
            if (value.Count() > this.MaxCount) result.AddError(string.Format(Strings.CanNotBeMoreItems, this.Label, this.MaxCount));
        }

    }
}
