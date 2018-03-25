using System;
using System.Collections;
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
using System.Collections.Specialized;

using CodeArt.WPF.UI;
using CodeArt.DTO;
using System.Collections.ObjectModel;

namespace CodeArt.WPF.Controls.Playstation
{
    public class Items : Input
    {
        public Items()
        {
            this.DefaultStyleKey = typeof(Items);
        }

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(ControlTemplate), typeof(Items));

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


        public static DependencyProperty MinLinesProperty = DependencyProperty.Register("MinLines", typeof(int), typeof(Items), new PropertyMetadata(1));

        public int MinLines
        {
            get
            {
                return (int)GetValue(MinLinesProperty);
            }
            set
            {
                SetValue(MinLinesProperty, value);
            }
        }

        public static DependencyProperty MaxLinesProperty = DependencyProperty.Register("MaxLines", typeof(int), typeof(Items), new PropertyMetadata(1));

        public int MaxLines
        {
            get
            {
                return (int)GetValue(MaxLinesProperty);
            }
            set
            {
                SetValue(MaxLinesProperty, value);
            }
        }

        /// <summary>
        /// 新增项的输入提示
        /// </summary>
        public static readonly DependencyProperty AddTipProperty = DependencyProperty.Register("AddTip", typeof(string), typeof(Items), new PropertyMetadata(Strings.NewItem));

        /// <summary>
        /// 新增项的输入提示
        /// </summary>
        public string AddTip
        {
            get
            {
                return (string)GetValue(AddTipProperty);
            }
            set
            {
                SetValue(AddTipProperty, value);
            }
        }


        public static readonly DependencyProperty CountProperty = DependencyProperty.Register("Count", typeof(int), typeof(Items), new PropertyMetadata(0));

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get
            {
                return (int)GetValue(CountProperty);
            }
            set
            {
                SetValue(CountProperty, value);
            }
        }


        private ListBox _list;
        private Button _addButton;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _list = GetTemplateChild("list") as List;
            _list.DataContext = this;

            _addButton = GetTemplateChild("addButton") as Button;
            _addButton.MouseUp += OnAddButtonClick;
        }

        private void OnAddButtonClick(object sender, MouseButtonEventArgs e)
        {
            AddItem();
        }

        /// <summary>
        /// 新增项
        /// </summary>
        internal void AddItem()
        {
            if (_newData == null) throw new InvalidOperationException(Strings.PleaseInitializeItemsComponent);
            if (_items.Count == this.MaxLines) return;
            var data = _newData();
            data.Index = _items.Count;
            _items.Add(data);
        }

        internal void InsertItem(int index)
        {
            if (index < 0 || index > _items.Count) return;
            for (var i = index; i < _items.Count; i++)
            {
                _items[i].Index++;
            }
            var data = _newData();
            data.Index = index;
            _items.Insert(index, data);
        }

        internal void MoveUpItem(int index)
        {
            if (index == 0) return;
            if (index < 0 || index >= _items.Count) return;

            var current = _items[index];
            var prv = _items[index - 1];

            _items[index - 1] = current;
            _items[index] = prv;

            current.Index = index - 1;
            prv.Index = index;
        }

        internal void MoveDownItem(int index)
        {
            if (index == _items.Count - 1) return;
            if (index < 0 || index >= _items.Count) return;

            var current = _items[index];
            var next = _items[index + 1];

            _items[index + 1] = current;
            _items[index] = next;

            current.Index = index + 1;
            next.Index = index;
        }

        public void RemoveItem(int index)
        {
            if (index < 0 || index >= _items.Count) return;
            if (_items.Count <= this.MinLines) return;

            for (var i = index + 1; i < _items.Count; i++)
            {
                _items[i].Index--;
            }
            _items.RemoveAt(index);
        }

        public override void Write(DTObject data)
        {
            foreach (var item in _items)
            {
                if (!item.IsNull())
                {
                    var d = data.CreateAndPush(this.MemberName);
                    d.Load(item);
                }
            }
        }

        private List<ItemsData> Get()
        {
            List<ItemsData> value = new List<ItemsData>(); 
            foreach (var item in _items)
            {
                if (!item.IsNull())
                {
                    value.Add(item);
                }
            }
            return value;
        }

        public override void Clear()
        {
            _items.Clear();
            InitMinLines();
        }

        private Func<ItemsData> _newData;

        private ObservableCollection<ItemsData> _items;

        public void Init(Func<ItemsData> newData)
        {
            _newData = newData;
            _list.ItemsSource = _items = new ObservableCollection<ItemsData>();
            _items.CollectionChanged += OnItemsCollectionChanged;
            InitMinLines();
        }

        private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.Count = _items.Count;
        }

        private void InitMinLines()
        {
            for (var i = 0; i < this.MinLines; i++)
            {
                AddItem();
            }
        }


        public override void Validate(ValidationResult result)
        {
            var value = this.Get();
            if (this.Required && value.Count == 0) result.AddError(string.Format("{0}{1}", Strings.PleaseEnter, this.Label));
            if (value.Count < this.MinLines) result.AddError(string.Format(Strings.CanNotBeLessItems, this.Label, this.MinLines));
            if (this.MaxLines > 0 && value.Count > this.MaxLines) result.AddError(string.Format(Strings.CanNotBeMoreItems, this.Label, this.MaxLines));
        }
    }
}