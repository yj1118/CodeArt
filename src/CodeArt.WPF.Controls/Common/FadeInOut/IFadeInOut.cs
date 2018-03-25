using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.WPF.Controls
{
    /// <summary>
    /// 淡入淡出的可见性元素
    /// </summary>
    public interface IFadeInOut
    {

        /// <summary>
        /// 准备淡出
        /// </summary>
        void PreFadeOut();

        /// <summary>
        /// 已经完成淡出
        /// </summary>
        void FadedOut();


        /// <summary>
        /// 准备淡入
        /// </summary>
        void PreFadeIn();


        /// <summary>
        /// 已经完成淡入
        /// </summary>
        void FadedIn();
    }
}
