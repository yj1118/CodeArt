using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;

using System.Windows.Controls;
using System.Windows;
using CodeArt.WPF;

namespace CodeArt.WPF.Controls.Playstation
{
    public enum RangeMode
    {
        /// <summary>
        /// 无特效
        /// </summary>
        None,
        /// <summary>
        /// 边框模式
        /// </summary>
        Border,
        /// <summary>
        /// 透明度模式
        /// </summary>
        Opacity,
        /// <summary>
        /// 同时拥有两种效果
        /// </summary>
        Both,
    }
}
