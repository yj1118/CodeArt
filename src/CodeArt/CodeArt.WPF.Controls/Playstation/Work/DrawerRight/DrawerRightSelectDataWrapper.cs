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

using CodeArt.WPF.UI;

namespace CodeArt.WPF.Controls.Playstation
{
    internal class DrawerRightSelectDataWrapper : DrawerRightSelectData
    {
        public bool Selected { get; set; }

        public DrawerRightSelectDataWrapper(DrawerRightSelectData data)
            : base(data.Value, data.Text, data.SubText)
        {
            this.Selected = false;
        }


        public static IEnumerable<DrawerRightSelectDataWrapper> CreateItems(IEnumerable<DrawerRightSelectData> datas, DrawerRightSelectData current = null)
        {
            List<DrawerRightSelectDataWrapper> items = new List<DrawerRightSelectDataWrapper>(datas.Count());
            foreach (var data in datas)
            {
                var item = new DrawerRightSelectDataWrapper(data);
                item.Selected = current != null && item.Value.Equals(current.Value);
                items.Add(item);
            }
            return items;
        }
    }
}