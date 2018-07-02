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
    public enum CurtainStatus
    {
        /// <summary>
        /// 等待演出
        /// </summary>
        Wait,
        /// <summary>
        /// 正在演出
        /// </summary>
        Perform
    }
}
