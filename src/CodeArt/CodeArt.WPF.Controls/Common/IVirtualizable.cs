using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.WPF.Controls
{
    /// <summary>
    /// 表示可见的类型
    /// </summary>
    public interface IVirtualizable
    {
        /// <summary>
        /// 是否可见
        /// </summary>
        bool IsVisible
        {
            get;
            set;
        }

        /// <summary>
        /// 项高度
        /// </summary>
        double Height
        {
            get;
        }

        /// <summary>
        /// 项的序号
        /// </summary>
        int Index
        {
            get;
        }

    }
}
