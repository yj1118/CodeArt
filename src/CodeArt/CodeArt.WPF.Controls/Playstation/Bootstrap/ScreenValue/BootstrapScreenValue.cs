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
    public abstract class BootstrapScreenValue : BootstrapValue
    {
        internal BootstrapScreenValue(string range, int rangeIndex, string valueExpression)
            : base(range, rangeIndex, valueExpression)
        {
        }

        protected override object GetResult()
        {
            var args = this.ValueExpression.Trim('(', ')').Split(' ');
            double offset = GetArgumentValue(args, "o");//整体的大小调整
            double itemOffset = GetArgumentValue(args, "i");
            double multiplication = GetArgumentValue(args, "*");
            double division = GetArgumentValue(args, "/");

            if(multiplication > 0 && division > 0)
            {
                //先乘再除
                return (GetLogicAreaValue() + offset) * multiplication / division + itemOffset;
            }
            else
            {
                if (division > 0)
                {
                    return (GetLogicAreaValue() + offset) / division + itemOffset;
                }
                else if (multiplication > 0)
                {
                    return (GetLogicAreaValue() + offset) * multiplication + itemOffset;
                }
                else
                {
                    //输出原始设置
                    var value = GetArgumentValue(args);
                    return value;
                }
            }
        }

        /// <summary>
        /// 获取屏幕逻辑区域的高或者宽
        /// </summary>
        /// <returns></returns>
        protected abstract int GetLogicAreaValue();

        private static double GetArgumentValue(string[] args, string sign)
        {
            foreach (var arg in args)
            {
                if (arg.StartsWith(sign))
                {
                    return double.Parse(arg.Substring(1));
                }
            }
            return 0;
        }

        /// <summary>
        /// 获取没有任何符号的数值，这意味着是单纯的数值
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static double GetArgumentValue(string[] args)
        {
            foreach (var arg in args)
            {
                if (double.TryParse(arg,out var result))
                {
                    return result;
                }
            }
            return 0;
        }




    }
}