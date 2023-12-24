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

namespace CodeArt.WPF.Controls.Playstation
{
    /// <summary>
    /// Navigation.xaml 的交互逻辑
    /// </summary>
    public partial class NavigationPage : UserControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(NavigationPage));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(PageStatus), typeof(NavigationPage), new PropertyMetadata(PageStatus.Normal));

        /// <summary>
        /// 
        /// </summary>
        public PageStatus Status
        {
            get { return (PageStatus)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }


        public NavigationPage()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.items.ItemsSource = new ObservableCollection<NavigationItem>();
            _protector = new EventProtector<object>();
        }

        private EventProtector<object> _protector = null;

        private void _OnSelectItem(object sender)
        {
            UIElement element = null;
            var item = (sender as FrameworkElement).DataContext as NavigationItem;
            if (item != null)
            {
                element = item.Create();
            }

            if (element != null)
            {
                Work.Current.Go(element,()=>
                {
                    _protector.End();
                });
            }
            else
            {
                _protector.End();
            }
        }

        private void OnSelectItem(object sender, MouseButtonEventArgs e)
        {
            _protector.Start(_OnSelectItem, sender);
        }

        public void AddItem(NavigationItem item)
        {
            var data = this.items.ItemsSource as ObservableCollection<NavigationItem>;
            data.Add(item);
        }

        public void AddItems(IEnumerable<NavigationItem> items)
        {
            var data = this.items.ItemsSource as ObservableCollection<NavigationItem>;
            foreach (var item in items) data.Add(item);
        }

    }
}
