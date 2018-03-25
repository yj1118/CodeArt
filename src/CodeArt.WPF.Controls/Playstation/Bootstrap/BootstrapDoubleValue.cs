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
using System.Globalization;

using CodeArt.WPF.UI;

namespace CodeArt.WPF.Controls.Playstation
{
    /// <summary>
    /// 
    /// </summary>
    public class BootstrapDoubleValue : BootstrapValue
    {
        internal BootstrapDoubleValue(string range, int rangeIndex, string valueExpression)
            : base(range, rangeIndex, valueExpression)
        {
        }

        protected override object GetResult()
        {
            return double.Parse(this.ValueExpression);
        }
    }
}