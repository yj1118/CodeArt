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

using CodeArt.Util;

namespace CodeArt.WPF.Controls.Playstation
{
    /// <summary>
    /// Histogram.xaml 的交互逻辑
    /// </summary>
    public partial class Histogram : UserControl
    {
        public Histogram()
        {
            InitializeComponent();
        }

        public ObservableCollection<HistogramItem> ItemsSource
        {
            get
            {
                return this.list.ItemsSource as ObservableCollection<HistogramItem>;
            }
            set
            {
                CalculatePercentage(value);
                this.list.ItemsSource = value;
            }
        }

        private void CalculatePercentage(IList<HistogramItem> items)
        {
            if (items == null || items.Count == 0) return;
            var total = items.Sum((h) =>
            {
                return h.Value;
            }); 

            foreach(var item in items)
            {
                item.PercentageValue = total == 0 ? 0 : item.Value / total;
                item.ValueText = item.PercentageValue.PercentageText();

            }
        }


        public Action<HistogramItem> SelectItemAction;

        private void OnSelectItem(object sender, MouseButtonEventArgs e)
        {
            if (this.SelectItemAction != null)
            {
                var item = (sender as FrameworkElement).DataContext as HistogramItem;
                this.SelectItemAction(item);
            }   
        }
    }
}
