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
    public enum ButtonSize
    {
        /// <summary>
        /// 未指定尺寸，意味着按钮大小自定义
        /// </summary>
        None,
        xs,
        sm,
        md,
        lg
    }
}
