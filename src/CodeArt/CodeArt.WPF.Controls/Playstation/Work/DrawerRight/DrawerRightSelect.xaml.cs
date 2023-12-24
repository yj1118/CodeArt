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

namespace CodeArt.WPF.Controls.Playstation
{
    /// <summary>
    /// DrawerRightSelect.xaml 的交互逻辑
    /// </summary>
    public partial class DrawerRightSelect : List
    {
        public DrawerRightSelect()
        {
            InitializeComponent();
        }

        private Action<DrawerRightSelectData, DrawerRightSelectData> _onSelected;

        public void SetItems(IEnumerable<DrawerRightSelectData> items, Action<DrawerRightSelectData, DrawerRightSelectData> onSelected, DrawerRightSelectData current = null)
        {
            this.ItemsSource = null;
            this.ItemsSource = DrawerRightSelectDataWrapper.CreateItems(items, current);
            _data = current;
            _onSelected = onSelected;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.SelectionChanged += OnSelected;
        }

        private void OnSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;

            var parent = (sender as List);
            var data = e.AddedItems[0] as DrawerRightSelectDataWrapper;
            var old = _data;
            var changed = Set(data);

            if (changed && _onSelected != null)
            {
                _onSelected(data, old);
            }
        }


        private DrawerRightSelectData _data;

        public DrawerRightSelectData Get()
        {
            return _data;
        }


        private bool Set(DrawerRightSelectDataWrapper data)
        {
            if (data == null) return false;
            if (_data != null && _data.Value.Equals(data.Value)) return false;

            var items = this.Items;
            foreach (DrawerRightSelectDataWrapper item in items)
            {
                item.Selected = item.Value.Equals(data.Value);
            }

            var texts = this.GetChilds<RangeText>();
            foreach (var text in texts)
            {
                var t = text.DataContext as DrawerRightSelectDataWrapper;
                if (t.Value.Equals(data.Value))
                {
                    _data = t;
                    text.IsFiexdFocus = true;
                    text.ImageVisibility = Visibility.Visible;
                }
                else
                {
                    text.IsFiexdFocus = false;
                    text.ImageVisibility = Visibility.Hidden;
                }
            }

            _data = data;
            return true;
        }

    }
}
