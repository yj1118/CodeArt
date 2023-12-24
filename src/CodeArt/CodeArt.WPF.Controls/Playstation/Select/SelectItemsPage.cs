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
using System.Collections.ObjectModel;

using CodeArt.WPF.UI;


namespace CodeArt.WPF.Controls.Playstation
{
    public class SelectItemsPage : WorkScene
    {
        public SelectItemsPage()
        {
            this.DefaultStyleKey = typeof(SelectItemsPage);
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(SelectItemsPage));

        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }
            private set
            {
                SetValue(TitleProperty, value);
            }
        }

        public static readonly DependencyProperty LoadItemsProperty = DependencyProperty.Register("LoadItems", typeof(Func<IEnumerable<TextItem>>), typeof(SelectItemsPage));

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

        public static readonly DependencyProperty OwnerProperty = DependencyProperty.Register("Owner", typeof(Select), typeof(SelectItemsPage), new PropertyMetadata(null, OwnerChanged));

        private static void OwnerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var owner = (e.NewValue as Select);
            var items = (d as SelectItemsPage);
            if(owner != null)
            {
                items.Title = owner.Tip;
            }
        }

        /// <summary>
        /// 加载项的方法
        /// </summary>
        public Select Owner
        {
            get
            {
                return (Select)GetValue(OwnerProperty);
            }
            set
            {
                SetValue(OwnerProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CountProperty = DependencyProperty.Register("Count", typeof(int), typeof(SelectItemsPage), new PropertyMetadata(0));

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

        public int MaxCount
        {
            get
            {
                return _list.MaxCount;
            }
            set
            {
                _list.MaxCount = value;
            }
        }

        public int MinCount
        {
            get
            {
                return _list.MinCount;
            }
            set
            {
                _list.MinCount = value;
            }
        }



        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register("ItemWidth", typeof(double), typeof(SelectItemsPage), new PropertyMetadata(double.NaN));

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

        public static readonly DependencyProperty ItemContainerWidthProperty = DependencyProperty.Register("ItemContainerWidth", typeof(double), typeof(SelectItemsPage), new PropertyMetadata(double.NaN));

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

        private Loading _loading;
        private SelectItems _list;
        private Button _ok;

        public override void Rendered()
        {
            _list = GetTemplateChild("list") as SelectItems;
            _loading = GetTemplateChild("loading") as Loading;
            _ok = GetTemplateChild("ok") as Button;

            this.MakeChildsLoaded(_list, _loading, _ok);
            _list.DataContext = this;
            _ok.MouseDown += OnOk;

            Init();
        }

        private void OnOk(object sender, MouseButtonEventArgs e)
        {
            var datas = Get();
            this.Owner.Set(datas);
            Work.Current.Back();
        }

        private IEnumerable<TextItem> Get()
        {
            return _list.Get();
        }

        internal void Set(IEnumerable<TextItem> value)
        {
            _list.Set(value);
        }

        private void Init()
        {
            if (this.LoadItems != null)
            {
                var action = this.LoadItems;
                var items = action();//此处不能异步，因为外界需要调用set
                this.Dispatcher.Invoke(() =>
                {
                    _list.ItemsSource = items;
                    this.Count = items.Count();
                    Inited();
                });
            }
            else
            {
                Inited();
            }
        }

        private void Inited()
        {
            _loading.IsActive = false;
        }


        //internal void AddItem(TextItem data)
        //{
        //    _list.AddItem(data);
        //}

        //internal void RemoveItem(TextItem data)
        //{
        //    _list.RemoveItem(data);
        //}

        //internal bool ContainsItem(TextItem data)
        //{
        //    return _list.ContainsItem(data);
        //}

        //internal void ClearItems()
        //{
        //    _list.ClearItems();
        //}

    }
}