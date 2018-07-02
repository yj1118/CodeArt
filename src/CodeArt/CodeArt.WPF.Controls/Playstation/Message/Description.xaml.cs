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
    /// MessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class Description : WorkScene
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(Description));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty ItemsCountProperty = DependencyProperty.Register("ItemsCount", typeof(int), typeof(Description));

        public int ItemsCount
        {
            get { return (int)GetValue(ItemsCountProperty); }
            set { SetValue(ItemsCountProperty, value); }
        }

        public Description()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public override void Exited()
        {
            this.ItemsSource = null;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.MakeChildsLoaded(page);
        }

        private void OK_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Work.Current?.Back();
        }


        public ObservableCollection<DescriptionItem> ItemsSource
        {
            get
            {
                return this.list.ItemsSource as ObservableCollection<DescriptionItem>;
            }
            set
            {
                var old = this.list.ItemsSource as ObservableCollection<DescriptionItem>;
                if (old != null)
                {
                    old.CollectionChanged -= OnItemsSourceCollectionChanged;
                }
                if(value != null)
                {
                    value.CollectionChanged += OnItemsSourceCollectionChanged;
                }
                this.list.ItemsSource = value;
            }
        }

        private void OnItemsSourceCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var value = sender as ObservableCollection<DescriptionItem>;
            this.ItemsCount = value.Count;
        }



    }
}
