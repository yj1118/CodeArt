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
using CodeArt.Util;

namespace CodeArt.WPF.Controls.Playstation
{
    /// <summary>
    /// 
    /// </summary>
    public class BootstrapThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _getResult(parameter.ToString());
        }

        private static Func<string, Thickness> _getResult = LazyIndexer.Init<string, Thickness>((code)=>
        {
            Thickness result = new Thickness();
            var exps = code.Split(';');
            for (var i = 0; i < exps.Length; i++)
            {
                var exp = exps[i];
                var be = BootstrapExpression.Create(BootstrapValueType.Width, exp);
                var bv = be.GetValue();
                var itemValue = (double)bv.Result;
                switch (i)
                {
                    case 0: result.Left = itemValue; break;
                    case 1: result.Top = itemValue; break;
                    case 2: result.Right = itemValue; break;
                    case 3: result.Bottom = itemValue; break;
                }
            }
            return result;
        });


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public readonly static BootstrapThicknessConverter Instance = new BootstrapThicknessConverter();
    }
}