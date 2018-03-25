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
using CodeArt.WPF.Screen;

namespace CodeArt.WPF.Controls.Playstation
{
    /// <summary>
    /// 根据当前屏幕逻辑宽度计算宽度
    /// 语法：o表示整个屏幕宽度的偏移量
    /// i表示整个屏幕宽度除以数量后，每个块的宽度的偏移量
    /// /表示除法
    /// *表示乘法
    /// </summary>
    public class BootstrapHeightValue : BootstrapScreenValue
    {
        internal BootstrapHeightValue(string range, int rangeIndex, string valueExpression)
            : base(range, rangeIndex, valueExpression)
        {
        }

        protected override int GetLogicAreaValue()
        {
            return Work.Current.LogicArea.Height;
        }
    }
}