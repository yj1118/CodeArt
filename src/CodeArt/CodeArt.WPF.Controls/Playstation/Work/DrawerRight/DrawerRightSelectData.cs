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
    public class DrawerRightSelectData : TextItem
    {
        public string SubText { get; private set; }

        public DrawerRightSelectData(object value, string text,string subText)
            : base(value)
        {
            this.Text = text;
            this.SubText = subText;
        }

        public DrawerRightSelectData(object value, string text)
           : this(value,text,string.Empty)
        {
        }
    }
}