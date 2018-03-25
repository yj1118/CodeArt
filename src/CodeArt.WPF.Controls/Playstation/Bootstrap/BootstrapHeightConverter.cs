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
    public class BootstrapHeightConverter : IValueConverter
    {

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var exp = parameter.ToString();
            var be = BootstrapExpression.Create(BootstrapValueType.Height, exp);
            var bv = be.GetValue();
            return bv.Result;
        }

        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public readonly static BootstrapHeightConverter Instance = new BootstrapHeightConverter();
    }
}