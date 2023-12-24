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
    public enum BootstrapValueType
    {
        /// <summary>
        /// 单纯的数值计算
        /// </summary>
        Double,
        /// <summary>
        /// 表示宽的计算
        /// </summary>
        Width,
        /// <summary>
        /// 表示高的计算
        /// </summary>
        Height,
        /// <summary>
        /// margin或padding等属性需要用到的单位
        /// </summary>
        Thickness
    }
}