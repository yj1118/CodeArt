using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CodeArt.WPF.Controls
{
    public class FadeInOut : UserControl,IFadeInOut
    {
        public virtual void PreFadeOut()
        {
        }

        public virtual void FadedOut()
        {
        }

        public virtual void PreFadeIn()
        {
        }

        public virtual void FadedIn()
        {
        }



        /// <summary>
        /// 准备淡出
        /// </summary>
        internal static void RaisePreFadeOut(UIElement target)
        {
            RaiseEvent(target,(f) => f.PreFadeOut());
        }

        /// <summary>
        /// 已经完成淡出
        /// </summary>
        internal static void RaiseFadedOut(UIElement target)
        {
            RaiseEvent(target, (f) => f.FadedOut());
        }


        /// <summary>
        /// 准备淡入
        /// </summary>
        internal static void RaisePreFadeIn(UIElement target)
        {
            RaiseEvent(target, (f) => f.PreFadeIn());
        }


        /// <summary>
        /// 已经完成淡入
        /// </summary>
        internal static void RaiseFadedIn(UIElement target)
        {
            RaiseEvent(target, (f) => f.FadedIn());
        }

        private static void RaiseEvent(UIElement target, Action<IFadeInOut> eventAction)
        {
            var f = target as IFadeInOut;
            if (f != null)
            {
                var e = f as FrameworkElement;
                if (e != null)
                {
                    if (e.Visibility == Visibility.Collapsed) return; //元素没有被显示，不触发事件
                }
                eventAction(f);
            }

            target.EachChilds((child) =>
            {
                var cf = child as IFadeInOut; //不是目标接口，继续遍历
                if (cf == null) return true;

                var e = cf as FrameworkElement;
                if (e != null)
                {
                    if (e.Visibility == Visibility.Collapsed) return false; //元素没有被显示，不触发事件,而且不必再遍历
                }
                eventAction(cf);
                return true;
            });
        }
    }
}
