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

using CodeArt.WPF.UI;

namespace CodeArt.WPF.Controls.Playstation
{
    /// <summary>
    /// 代表工作场景
    /// </summary>
    public interface IScene : IFadeInOut
    {
        /// <summary>
        /// 表示场景已经呈现完毕，这意味着动画效果已结束,使用该方法来处理场景加载后的逻辑可以令场景切换的动画效果更流畅
        /// 一个组件只会被绘制一次（除非显示的调用刷新）
        /// </summary>
        void Rendered();

        /// <summary>
        /// 场景被刷新时触发
        /// </summary>
        void Reset();

        /// <summary>
        /// 表示已退出场景，这意味着场景被销毁了
        /// </summary>
        void Exited();


    }
}