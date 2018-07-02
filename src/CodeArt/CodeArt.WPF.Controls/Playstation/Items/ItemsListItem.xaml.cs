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

using CodeArt.WPF.Controls;

namespace CodeArt.WPF.Controls.Playstation
{
    /// <summary>
    /// ItemsListItem.xaml 的交互逻辑
    /// </summary>
    public partial class ItemsListItem : UserControl
    {
        public ItemsListItem()
        {
            InitializeComponent();
        }

        private Items _parent;


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _parent = this.GetParent<Items>();
            add.MouseUp -= OnAdd;
            add.MouseUp += OnAdd;

            moveUp.MouseUp -= OnMoveUp;
            moveUp.MouseUp += OnMoveUp;

            moveDown.MouseUp -= OnMoveDown;
            moveDown.MouseUp += OnMoveDown;

            remove.MouseUp -= OnRemove;
            remove.MouseUp += OnRemove;
        }

        private void OnRemove(object sender, MouseButtonEventArgs e)
        {
            var data = this.DataContext as ItemsData;
            if (data == null) return;
            _parent.RemoveItem(data.Index);
        }

        private void OnMoveDown(object sender, MouseButtonEventArgs e)
        {
            var data = this.DataContext as ItemsData;
            if (data == null) return;
            _parent.MoveDownItem(data.Index);
        }

        private void OnAdd(object sender, MouseButtonEventArgs e)
        {
            var data = this.DataContext as ItemsData;
            if (data == null) return;
            _parent.InsertItem(data.Index + 1);
        }

        private void OnMoveUp(object sender, MouseButtonEventArgs e)
        {
            var data = this.DataContext as ItemsData;
            if (data == null) return;
            _parent.MoveUpItem(data.Index);
        }
    }
}
