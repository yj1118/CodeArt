using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Collections;

namespace CodeArt.WPF.Controls.Playstation
{
    public class SelectItems : ContentControl
    {
        public SelectItems()
        {
            this.DefaultStyleKey = typeof(SelectItems);
        }

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(ControlTemplate), typeof(SelectItems));

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



        /// <summary>
        /// 最多选择几项，如果为1，则是单选
        /// </summary>
        public static readonly DependencyProperty MaxCountProperty = DependencyProperty.Register("MaxCount", typeof(int), typeof(SelectItems), new PropertyMetadata(1));

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
        public static readonly DependencyProperty MinCountProperty = DependencyProperty.Register("MinCount", typeof(int), typeof(SelectItems), new PropertyMetadata(0));

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
                return this.MaxCount == 1;
            }
        }

        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register("ItemWidth", typeof(double), typeof(SelectItems), new PropertyMetadata(double.NaN));

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

        public static readonly DependencyProperty ItemContainerWidthProperty = DependencyProperty.Register("ItemContainerWidth", typeof(double), typeof(SelectItems), new PropertyMetadata(double.NaN));

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

        public static readonly DependencyProperty NoDataTextProperty = DependencyProperty.Register("NoDataText", typeof(string), typeof(SelectItems), new PropertyMetadata(Strings.NoData));

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

        private TextBlock _empty;
        private List _list;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _list = GetTemplateChild("list") as List;
            _list.DataContext = this;
            _empty = GetTemplateChild("empty") as TextBlock;
        }

        public IEnumerable<TextItem> ItemsSource
        {
            get
            {
                return _list.ItemsSource as IEnumerable<TextItem>;
            }
            set
            {
                _list.ItemsSource = value;
            }
        }



        public IEnumerable<TextItem> Get()
        {
            var items = this.ItemsSource;
            List<TextItem> datas = new List<TextItem>();
            foreach (var item in items)
            {
                if (item.IsSelected)
                {
                    datas.Add(item);
                }
            }
            return datas;
        }

        public void Set(IEnumerable<TextItem> value)
        {
            ClearValues();
            if (value == null) return;
            foreach (var data in value)
            {
                AddValue(data);
            }
        }

        /// <summary>
        /// 被选中的项是否包含<paramref name="data"/>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        internal bool ContainsValue(TextItem data)
        {
            if (data == null) return false;
            var value = this.Get();
            var item = value.FirstOrDefault((t)=>
            {
                return t.Value.Equals(data.Value);
            });
            return item != null;
        }



        internal void AddValue(TextItem data)
        {
            var item = FindItem(data);
            if (item != null) item.IsSelected = true;
        }

        internal void RemoveValue(TextItem data)
        {
            if (data == null) return;
            var item = FindItem(data);
            if (item != null) item.IsSelected = false;
        }

        internal void ClearValues()
        {
            var items = _list.GetChilds<SelectItem>();
            foreach(var item in items)
            {
                item.Selected = false;
            }
        }

        private TextItem FindItem(TextItem data)
        {
            if (data == null) return null;
            var items = this.ItemsSource;
            return items.FirstOrDefault((item) =>
            {
                var value = item.Value;
                if (value == null) return false;
                return value.Equals(data.Value);
            });
        }
    }
}