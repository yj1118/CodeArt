using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CodeArt.WPF.UI
{
    public static class Animations
    {
        public struct LinearConfig<T>
        {
            public T Start;
            public T End;
            public double DurationMilliseconds;
            public Action CallBack;
            public EasingMode EasingMode;
        }

        private const double EasingPower = 5;

        /// <summary>
        /// 减少透明度（从1减少到0）
        /// </summary>
        /// <param name="target"></param>
        /// <param name="durationMilliseconds"></param>
        /// <param name="easingMode"></param>
        public static void OpacityIn(UIElement target, double durationMilliseconds, EasingMode easingMode, Action callBack = null)
        {
            Opacity(target, 1, 0, durationMilliseconds, easingMode, callBack);
            //Animations.Linear(target, UIElement.OpacityProperty, new Animations.LinearConfig<double>()
            //{
            //    DurationMilliseconds = durationMilliseconds,
            //    EasingMode = easingMode,
            //    Start = 1,
            //    End = 0,
            //    CallBack = callBack
            //});
        }

        public static void Opacity(UIElement target, double from, double to, double durationMilliseconds, EasingMode easingMode, Action callBack = null)
        {
            Animations.Linear(target, UIElement.OpacityProperty, new Animations.LinearConfig<double>()
            {
                DurationMilliseconds = durationMilliseconds,
                EasingMode = easingMode,
                Start = from,
                End = to,
                CallBack = callBack
            });
        }

        public static void Opacity(DependencyObject target, DependencyProperty opacityProperty, double from, double to, double durationMilliseconds, EasingMode easingMode, Action callBack = null)
        {
            Animations.Linear(target, opacityProperty, new Animations.LinearConfig<double>()
            {
                DurationMilliseconds = durationMilliseconds,
                EasingMode = easingMode,
                Start = from,
                End = to,
                CallBack = callBack
            });
        }


        public static void OpacityIn(DependencyObject target, DependencyProperty opacityProperty, double durationMilliseconds, EasingMode easingMode, Action callBack = null)
        {
            Animations.Linear(target, opacityProperty, new Animations.LinearConfig<double>()
            {
                DurationMilliseconds = durationMilliseconds,
                EasingMode = easingMode,
                Start = 1,
                End = 0,
                CallBack = callBack
            });
        }

        /// <summary>
        /// 增加透明度（从0增加到1）
        /// </summary>
        /// <param name="target"></param>
        /// <param name="durationMilliseconds"></param>
        /// <param name="easingMode"></param>
        public static void OpacityOut(UIElement target, double durationMilliseconds, EasingMode easingMode, Action callBack = null)
        {
            Opacity(target, 0, 1, durationMilliseconds, easingMode, callBack);
        }

        public static void OpacityOut(DependencyObject target, DependencyProperty opacityProperty, double durationMilliseconds, EasingMode easingMode, Action callBack = null)
        {
            Animations.Linear(target, opacityProperty, new Animations.LinearConfig<double>()
            {
                DurationMilliseconds = durationMilliseconds,
                EasingMode = easingMode,
                Start = 0,
                End = 1,
                CallBack = callBack
            });
        }

        /// <summary>
        /// 缩放对象
        /// </summary>
        /// <param name="target"></param>
        /// <param name="max"></param>
        /// <param name="durationMilliseconds"></param>
        /// <param name="easingMode"></param>
        public static void Scale(UIElement target, double from, double to, double durationMilliseconds, EasingMode easingMode)
        {
            ScaleTransform scale = new ScaleTransform();
            target.RenderTransform = scale;
            target.RenderTransformOrigin = new Point(0.5, 0.5);
            EasingFunctionBase easeFunction = new PowerEase()
            {
                EasingMode = easingMode,
                Power = EasingPower
            };
            DoubleAnimation animation = new DoubleAnimation()
            {
                From = from,                                   //起始值
                To = to,                                     //结束值
                EasingFunction = easeFunction,                    //缓动函数
                Duration = TimeSpan.FromMilliseconds(durationMilliseconds)  //动画播放时间
            };
            AnimationClock clock = animation.CreateClock();
            scale.ApplyAnimationClock(ScaleTransform.ScaleXProperty, clock);
            scale.ApplyAnimationClock(ScaleTransform.ScaleYProperty, clock);
        }

        /// <summary>
        /// 以缩小的形式隐藏对象
        /// </summary>
        public static void ToSmallHidden(UIElement target, double durationMilliseconds, Action callBack = null)
        {
            const EasingMode easingMode = EasingMode.EaseOut;
            Animations.Scale(target, 1,0.5, durationMilliseconds, easingMode);
            Animations.OpacityIn(target, durationMilliseconds, easingMode, callBack);
        }

        /// <summary>
        /// 以缩小的形式呈现对象
        /// </summary>
        /// <param name="target"></param>
        /// <param name="durationMilliseconds"></param>
        /// <param name="easingMode"></param>
        /// <param name="callBack"></param>
        public static void ToSmallVisible(UIElement target, double durationMilliseconds, Action callBack = null)
        {
            const EasingMode easingMode = EasingMode.EaseOut;
            Animations.Scale(target, 1.5, 1, durationMilliseconds, easingMode);
            Animations.OpacityOut(target, durationMilliseconds, easingMode, callBack);
        }

        /// <summary>
        /// 以放大的形式隐藏对象
        /// </summary>
        public static void ToBigHidden(UIElement target, double durationMilliseconds, Action callBack = null)
        {
            const EasingMode easingMode = EasingMode.EaseOut;
            Animations.Scale(target, 1, 1.3, durationMilliseconds, easingMode);
            Animations.OpacityIn(target, durationMilliseconds, easingMode, callBack);
        }

        /// <summary>
        /// 以放大的形式显示对象
        /// </summary>
        public static void ToBigVisible(UIElement target, double durationMilliseconds, Action callBack = null)
        {
            const EasingMode easingMode = EasingMode.EaseOut;
            Animations.Scale(target, 0.5, 1, durationMilliseconds, easingMode);
            Animations.OpacityOut(target, durationMilliseconds, easingMode, callBack);
        }

        /// <summary>
        /// 线性动画
        /// </summary>
        /// <param name="target"></param>
        /// <param name="property"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="durationMilliseconds"></param>
        public static void Linear(DependencyObject target, DependencyProperty property, LinearConfig<Thickness> config)
        {
            Linear<Thickness>(target, property, config, () =>
             {
                 EasingFunctionBase easeFunction = new PowerEase()
                 {
                     EasingMode = config.EasingMode,
                     Power = EasingPower
                 };

                 ThicknessAnimation animation = new ThicknessAnimation();
                 animation.From = config.Start;
                 animation.To = config.End;
                 animation.Duration = new Duration(TimeSpan.FromMilliseconds(config.DurationMilliseconds));
                 animation.EasingFunction = easeFunction;
                 return animation;
             });
        }

        public static void Linear(DependencyObject target, DependencyProperty property, LinearConfig<double> config)
        {
            Linear<double>(target, property, config, () =>
            {
                EasingFunctionBase easeFunction = new PowerEase()
                {
                    EasingMode = config.EasingMode,
                    Power = EasingPower
                };

                DoubleAnimation animation = new DoubleAnimation();
                animation.From = config.Start;
                animation.To = config.End;
                animation.Duration = new Duration(TimeSpan.FromMilliseconds(config.DurationMilliseconds));
                animation.EasingFunction = easeFunction;
                return animation;
            });
        }

        public static void Linear<T>(DependencyObject target, DependencyProperty property, LinearConfig<T> config, Func<AnimationTimeline> createTimeline)
        {
            var animation = createTimeline();

            if (config.CallBack != null)
            {
                animation.Completed += (sender, e) =>
                {
                    config.CallBack();
                };
            }

            var animatable = target as IAnimatable;
            if (animatable != null)
            {
                AnimationClock clock = animation.CreateClock();
                animatable.ApplyAnimationClock(property, clock);
            }
            else
            {
                Storyboard.SetTarget(animation, target);
                Storyboard.SetTargetProperty(animation, new PropertyPath(property));
                Storyboard storyboard = new Storyboard();
                storyboard.Children.Add(animation);
                storyboard.Begin();
            }
        }

        public static void ShowDrawer(FrameworkElement target, double durationMilliseconds, Action callBack = null)
        {
            const EasingMode easingMode = EasingMode.EaseOut;

            var raw = target.Margin;

            if (target.HorizontalAlignment == HorizontalAlignment.Right)
            {
                var width = target.ActualWidth;
                //右边的抽屉
                Animations.Move(target,
                                new Thickness(raw.Left, raw.Top, -width, raw.Bottom),//由于是向右停靠HorizontalAlignment的值是Right
                                new Thickness(raw.Left, raw.Top, 0, raw.Bottom),
                                durationMilliseconds,
                                easingMode, callBack);
            }
            else if (target.HorizontalAlignment == HorizontalAlignment.Left)
            {
                var width = target.ActualWidth;
                //左边的抽屉
                Animations.Move(target,
                new Thickness(-width, raw.Top, raw.Right, raw.Bottom),
                new Thickness(0, raw.Top, raw.Right, raw.Bottom),
                durationMilliseconds,
                easingMode, callBack);
            }
            else if (target.VerticalAlignment == VerticalAlignment.Bottom)
            {
                var height = target.ActualHeight;
                //底部的抽屉
                Animations.Move(target,
                    new Thickness(raw.Left, raw.Top, raw.Right, -height),
                    new Thickness(raw.Left, raw.Top, raw.Right, 0),
                durationMilliseconds,
                easingMode, callBack);
            }
        }

        public static void HiddenDrawer(FrameworkElement target, double durationMilliseconds, Action callBack = null)
        {
            const EasingMode easingMode = EasingMode.EaseOut;
            var raw = target.Margin;

            if (target.HorizontalAlignment == HorizontalAlignment.Right)
            {
                var width = target.ActualWidth;
                Animations.Move(target,
                                new Thickness(raw.Left, raw.Top, 0, raw.Bottom),
                                new Thickness(raw.Left, raw.Top, -width, raw.Bottom),//由于是向右停靠HorizontalAlignment的值是Right
                                durationMilliseconds,
                                easingMode, callBack);
            }
            else if (target.HorizontalAlignment == HorizontalAlignment.Left)
            {
                var width = target.ActualWidth;
                Animations.Move(target,
                                new Thickness(0, raw.Top, raw.Right, raw.Bottom),
                                new Thickness(-width, raw.Top, raw.Right, raw.Bottom),
                                durationMilliseconds,
                                easingMode, callBack);
            }
            else if(target.VerticalAlignment == VerticalAlignment.Bottom)
            {
                var height = target.ActualHeight;
                //底部的抽屉
                Animations.Move(target,
                    new Thickness(raw.Left, raw.Top, raw.Right, 0),
                    new Thickness(raw.Left, raw.Top, raw.Right, -height),
                    durationMilliseconds,
                    easingMode, callBack);
            }
        }

        public static void Move(FrameworkElement target, Thickness from, Thickness to, double durationMilliseconds, EasingMode easingMode,Action callBack)
        {
            EasingFunctionBase easeFunction = new PowerEase()
            {
                EasingMode = easingMode,
                Power = EasingPower
            };
            ThicknessAnimation animation = new ThicknessAnimation()
            {
                From = from,                                   //起始值
                To = to,                                     //结束值
                EasingFunction = easeFunction,                    //缓动函数
                Duration = TimeSpan.FromMilliseconds(durationMilliseconds)  //动画播放时间
            };

            if(callBack != null)
            {
                animation.Completed += (sender, e) =>
                {
                    callBack();
                };
            }

            AnimationClock clock = animation.CreateClock();
            target.ApplyAnimationClock(FrameworkElement.MarginProperty, clock);
        }

    }
}
