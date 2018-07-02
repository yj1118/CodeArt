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
using System.Windows.Media.Animation;
using System.Threading;
using System.Collections.Concurrent;
using System.Drawing;

using CodeArt.WPF.UI;
using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.Concurrent.Sync;
using CodeArt.Concurrent.Pattern;
using CodeArt.WPF.Screen;


namespace CodeArt.WPF.Controls.Playstation
{
    public class Work : Control
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(Work));

        /// <summary>
        /// 
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty LogoProperty = DependencyProperty.Register("Logo", typeof(string), typeof(Work));

        /// <summary>
        /// 
        /// </summary>
        public string Logo
        {
            get { return (string)GetValue(LogoProperty); }
            set { SetValue(LogoProperty, value); }
        }


        /// <summary>
        /// 消息框的图片背景颜色
        /// </summary>
        public static readonly DependencyProperty TipImageColorProperty = DependencyProperty.Register("TipImageColor", typeof(System.Windows.Media.Brush), typeof(Work), new PropertyMetadata(Util.GetBrush("#fff")));

        public System.Windows.Media.Brush TipImageColor
        {
            get { return (System.Windows.Media.Brush)GetValue(TipImageColorProperty); }
            set { SetValue(TipImageColorProperty, value); }
        }

        public static readonly DependencyProperty ExitConfirmMessageProperty = DependencyProperty.Register("ExitConfirmMessage", typeof(string), typeof(Work), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 离开时的确认提示语
        /// </summary>
        public string ExitConfirmMessage
        {
            get { return (string)GetValue(ExitConfirmMessageProperty); }
            set { SetValue(ExitConfirmMessageProperty, value); }
        }

        public static readonly DependencyProperty ShowTitleBarProperty = DependencyProperty.Register("ShowTitleBar", typeof(bool), typeof(Work), new PropertyMetadata(true));

        /// <summary>
        /// 
        /// </summary>
        public bool ShowTitleBar
        {
            get { return (bool)GetValue(ShowTitleBarProperty); }
            set { SetValue(ShowTitleBarProperty, value); }
        }

        public static readonly DependencyProperty ShowCloseProperty = DependencyProperty.Register("ShowClose", typeof(bool), typeof(Work), new PropertyMetadata(true));

        /// <summary>
        /// 
        /// </summary>
        public bool ShowClose
        {
            get { return (bool)GetValue(ShowCloseProperty); }
            set { SetValue(ShowCloseProperty, value); }
        }

        public static readonly DependencyProperty ShowKeyboardProperty = DependencyProperty.Register("ShowKeyboard", typeof(bool), typeof(Work), new PropertyMetadata(false));

        /// <summary>
        /// 
        /// </summary>
        public bool ShowKeyboard
        {
            get { return (bool)GetValue(ShowKeyboardProperty); }
            set { SetValue(ShowKeyboardProperty, value); }
        }

        private Grid container;
        private TitleBar titleBar;
        private DrawerRight right;
        private DrawerLeft left;
        private DrawerTip tip;
        private DrawerBottom bottom;
        private Grid mask;
        private Grid obstruction;
        private EventProtector _onCloseMask;


        private ConcurrentQueue<Action> _actions;


        private void Execute(Action action)
        {
            _actions.Enqueue(action);
            TryContinue();
        }

        private void TryContinue()
        {
            if (_locked) return;
            if (_actions.TryDequeue(out var action))
            {
                action();
            }
        }


        public Work()
        {
            this.DefaultStyleKey = typeof(Work);
            _actions = new ConcurrentQueue<Action>();
            _onCloseMask = new EventProtector();
            InitTip();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.container = GetTemplateChild("container") as Grid;
            this.titleBar = GetTemplateChild("titleBar") as TitleBar;
            this.right = GetTemplateChild("right") as DrawerRight;
            this.left = GetTemplateChild("left") as DrawerLeft;
            this.bottom = GetTemplateChild("bottom") as DrawerBottom;
            this.tip = GetTemplateChild("tip") as DrawerTip;
            this.tip.MouseUp += OnClickTip;
            this.mask = GetTemplateChild("mask") as Grid;
            this.obstruction = GetTemplateChild("obstruction") as Grid;

            this.MakeChildsLoaded(this.left, this.right, this.bottom);
        }

        private void OnClickTip(object sender, MouseButtonEventArgs e)
        {
            this.CloseTip();
        }

        public void Init(Window root)
        {
            this.titleBar.Init(root);
        }

        private void OnCloseMask(object sender, MouseButtonEventArgs e)
        {
            _onCloseMask.Start(CloseDrawer);
        }

        private Stack<UIElement> _elements = new Stack<UIElement>();

        public int ElementCount
        {
            get
            {
                return _elements.Count;
            }
        }

        private const double Duration = 500;
        private volatile bool _locked = false;

        /// <summary>
        /// 定位到目标页
        /// </summary>
        /// <param name="element"></param>
        public void Go(UIElement element, Action callBack = null)
        {
            Execute(() =>
            {
                Redirect(element, Mode.Go, callBack);
            });
        }

        private enum Mode
        {
            /// <summary>
            /// 跳到下一页
            /// </summary>
            Go,
            /// <summary>
            /// 替换当前页
            /// </summary>
            Replace,
            /// <summary>
            /// 重置到目标,清理所有历史页
            /// </summary>
            Top
        }


        private void Redirect(UIElement element, Mode mode, Action callBack)
        {
            this.LockScreen();

            this.CloseDrawer();//跳转之前，关闭抽屉

            _locked = true;

            if (this.container.Children.Count > 0)
            {
                //之前就存在元素
                UIElement current = null;
                switch (mode)
                {
                    case Mode.Go:
                        {
                            current = _elements.Peek();
                        }
                        break;
                    case Mode.Replace:
                        {
                            current = _elements.Pop();
                        }
                        break;
                    case Mode.Top:
                        {
                            current = _elements.Peek();
                        }
                        break;
                }

                _elements.Push(element);
                this.container.Children.Add(element);
                FadeInOut.RaisePreFadeIn(element);
                Animations.ToSmallVisible(element, Duration);

                FadeInOut.RaisePreFadeOut(current);
                Animations.ToSmallHidden(current, Duration, () =>
                {
                    this.container.Children.RemoveAt(0);
                    _locked = false;
                    RaiseRendered(element);
                    FadeInOut.RaiseFadedIn(element);
                    switch (mode)
                    {
                        case Mode.Replace:
                            {
                                RaiseExited(current);
                            }
                            break;
                        case Mode.Top:
                            {
                                var top = _elements.Pop();
                                ClearElements();
                                _elements.Push(top);
                            }
                            break;
                    }
                    FadeInOut.RaiseFadedOut(current);

                    if (callBack != null) callBack();
                    this.UnlockScreen();
                    TryContinue();
                });
            }
            else
            {
                _elements.Push(element);
                this.container.Children.Add(element);
                Animations.ToSmallVisible(element, Duration, () =>
                {
                    _locked = false;
                    RaiseRendered(element);
                    if (callBack != null) callBack();
                    this.UnlockScreen();
                    TryContinue();
                });
            }
        }

        public void Clear()
        {
            ClearElements();
            this.container.Children.Clear();
        }

        private void ClearElements()
        {
            //清空
            while (_elements.Count > 0)
            {
                var t = _elements.Pop();
                RaiseExited(t);
            }
        }

        public UIElement CurrentElement
        {
            get
            {
                return _elements.Peek();
            }
        }


        private void RaiseRendered(UIElement element)
        {
            try
            {
                var scenes = element.GetChilds<IScene>();
                foreach (var scene in scenes)
                {
                    scene.Rendered();
                }

                {
                    var scene = element as IScene;
                    if (scene != null)
                    {
                        scene.Rendered();
                    }
                }
            }
            catch (Exception ex)
            {
                Work.Current.Catch(ex);
            }
        }

        /// <summary>
        /// 当element被移除时触发
        /// </summary>
        /// <param name="element"></param>
        private void RaiseExited(UIElement element)
        {
            try
            {
                var scenes = element.GetChilds<IScene>();
                foreach (var scene in scenes)
                {
                    scene.Exited();
                }

                {
                    var scene = element as IScene;
                    if (scene != null)
                    {
                        scene.Exited();
                    }
                }
            }
            catch(Exception ex)
            {
                Work.Current.Catch(ex);
            }
        }

        /// <summary>
        /// 以当前页为起点，反向找出对象，当前页索引为0,上一页为-1,以此类推
        /// </summary>
        /// <param name="index"></param>
        public UIElement Get(int index)
        {
            var absIndex = Math.Abs(index);
            return _elements.ElementAt(absIndex);
        }

        public T Get<T>() where T : UIElement
        {
            var index = GetIndex<T>();
            return Get(index) as T;
        }

        /// <summary>
        /// 返回上一个工作页
        /// </summary>
        public void Back(Action<bool> callBack = null)
        {
            Back(-1, callBack);
        }

        /// <summary>
        /// 返回上一个工作页
        /// </summary>
        public void Back(int index, Action<bool> callBack = null)
        {
            Execute(()=>
            {
                _Back(index, callBack);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="callBack">bool参数表示是否真的返回到目标了</param>
        private void _Back(int index, Action<bool> callBack)
        {
            this.LockScreen();

            if (index > 0)
            {
                if (callBack != null) callBack(false); //没有页面可以跳转，所以false
                this.UnlockScreen();
                return;
            }

            if (index == 0)
            {
                if (callBack != null) callBack(true); //0就是当前页，已经抵达，所以true
                this.UnlockScreen();
                return;
            }

            _locked = true;
            if (_elements.Count < 2)
            {
                _locked = false;
                if (callBack != null) callBack(false);
                this.UnlockScreen();
                TryContinue();
                return;
            }

            var absIndex = Math.Abs(index);
            var current = _elements.Peek();
            var target = _elements.ElementAt(absIndex);

            var isReset = Reset(target);
            this.container.Children.Insert(0, target);

            FadeInOut.RaisePreFadeIn(target);
            Animations.ToBigVisible(target, Duration, () =>
             {
                 if (isReset)
                 {
                     RaiseRendered(target);
                 }
                 FadeInOut.RaiseFadedIn(target);
             });

            FadeInOut.RaisePreFadeOut(current);
            Animations.ToBigHidden(current, Duration, () =>
            {
                this.container.Children.RemoveAt(1);
                for (var i = 0; i < absIndex; i++)
                {
                    var t = _elements.Pop();
                    RaiseExited(t);
                }

                FadeInOut.RaiseFadedOut(current);

                _locked = false;
                if (callBack != null) callBack(true); //成功跳转,true
                this.UnlockScreen();
                TryContinue();
            });
        }

        /// <summary>
        /// 用元素<paramref name="element"/>替换当前页
        /// </summary>
        /// <param name="element"></param>
        public void Replace(UIElement element, Action callBack = null)
        {
            Execute(() =>
            {
                Redirect(element, Mode.Replace, callBack);
            });
        }

        /// <summary>
        /// 清空所有的历史记录，置顶对象
        /// </summary>
        /// <param name="element"></param>
        /// <param name="callBack"></param>
        public void Top(UIElement element, Action callBack = null)
        {
            Execute(() =>
            {
                Redirect(element, Mode.Top, callBack);
            });
        }


        /// <summary>
        /// 返回首页，清理除首页以外的所有历史页（首页即第一页）
        /// </summary>
        /// <param name="callBack"></param>
        public void Index(Action callBack = null)
        {
            Execute(() =>
            {
                Back(-(_elements.Count - 1));
            });
        }

        /// <summary>
        /// 获得页面的序号,负数，表示从当前页倒退多少次可以达到<typeparamref name="T"/>类型的页,
        /// 该值大于0表示没有找到
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int GetIndex<T>()
        {
            int index = 1;
            for (int i = 0; i < _elements.Count; i++)
            {
                var e = _elements.ElementAt(i);
                if (e is T)
                {
                    index = -i;
                    break;
                }
            }
            return index;
        }


        public void Back<T>(Action<bool> callBack = null)
        {
            Execute(() =>
            {
                int index = GetIndex<T>();
                Back(index, callBack);
            });
        }



        public void Catch(Exception ex, Action backAction = null)
        {
#if DEBUG
            Message(ex.GetCompleteInfo(), false, backAction, null);
#endif

#if !DEBUG
            Message(ex.GetCompleteMessage(), false,backAction, null);
#endif

        }

        public void Message(string message, Action backAction = null, Action callBack = null)
        {
            Message(message, true, backAction, callBack);
        }

        public void Message(string message, bool newScene, Action backAction)
        {
            Message(message, newScene, backAction, null);
        }


        public void Message(string message, bool newScene, Action backAction = null, Action callBack = null)
        {
            Execute(() =>
            {
                var page = new MessagePage();
                this.MakeChildsLoaded(page);
                page.Message = message;
                page.BackAction = backAction;
                if (newScene) Go(page, callBack);
                else Replace(page, callBack);
            });
        }


        public void Confirm(string message,
                            (string Text, Action Action) OK,
                            (string Text, Action Action) Cancel,
                            Action callback = null)
        {
            Work.Current.Go(new MessageBox()
            {
                Message = message,
                OKText = OK.Text,
                OKAction = OK.Action,
                CancelText = Cancel.Text,
                CancelAction = Cancel.Action,
            }, callback);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="waitTitle"></param>
        /// <param name="asyncAction">在异步环境中执行的方法</param>
        /// <param name="toWatiPageCallBack">当完全切换到等待页后触发</param>
        public void Wait(string waitTitle, Action asyncAction, Action toWatiPageCallBack = null)
        {
            Execute(() =>
            {
                var page = new WaitingPage();
                this.MakeChildsLoaded(page);
                page.Title = waitTitle;
                Go(page, () =>
                {
                    this.AsyncRun(() =>
                    {
                        asyncAction();
                    });
                    if(toWatiPageCallBack != null)
                        toWatiPageCallBack();
                });
            });
        }



        private TimeoutMonitor _waitTimeout;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="waitTitle"></param>
        /// <param name="asyncAction">在异步环境中执行的方法</param>
        /// <param name="timeoutAction">超时需要执行的行为</param>
        /// <param name="timeoutSeconds">最大等待时间（秒），过了该时间认为超时了</param>
        public void Wait(string waitTitle, Action<WaitingToken> asyncAction, Action timeoutAction, int timeoutSeconds = 5, Action toWatiPageCallBack = null)
        {
            Execute(() =>
            {
                var token = InitWaitTimeout(timeoutAction, timeoutSeconds);

                var page = new WaitingPage();
                this.MakeChildsLoaded(page);
                page.Title = waitTitle;
                Go(page, () =>
                {
                    this.AsyncRun(() =>
                    {
                        asyncAction(token);
                    });
                    if (toWatiPageCallBack != null)
                        toWatiPageCallBack();
                });
            });
        }

        private WaitingToken _waitingToken = null;
        private object _syncWaitingToken = new object();

        private WaitingToken CreateWaitingToken(Action complete)
        {
            lock (_syncWaitingToken)
            {
                _waitingToken = new WaitingToken(complete);
                return _waitingToken;
            }
        }

        /// <summary>
        /// 获取token信息，如果token过期，那么返回null
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public WaitingToken GetWaitingToken(Guid tokenId)
        {
            lock (_syncWaitingToken)
            {
                return _waitingToken.Id == tokenId ? _waitingToken : null;
            }
        }

        private void WaitComplete()
        {
            ClearWaitTimeout();
        }

        private WaitingToken InitWaitTimeout(Action timeoutAction, int timeoutSeconds)
        {
            ClearWaitTimeout();
            var token = CreateWaitingToken(WaitComplete);
            _waitTimeout = new TimeoutMonitor(() =>
            {
                token.IsTimedOut = true;
                timeoutAction();
            });
            _waitTimeout.Start(timeoutSeconds * 1000);
            return token;
        }

        private void ClearWaitTimeout()
        {
            if (_waitTimeout != null)
            {
                _waitTimeout.Dispose();
                _waitTimeout = null;
            }
        }

        private static Work _current;

        public static Work Current
        {
            get
            {
                return _current;
            }
            set
            {
                _current = value;
            }
        }

        private Window _window;

        /// <summary>
        /// work所在的window
        /// </summary>
        public Window Window
        {
            get
            {
                if (_window == null) _window = this.GetParent<Window>();
                return _window;
            }
        }

        private Rectangle _logicArea;
        public Rectangle LogicArea
        {
            get
            {
                if (_logicArea == default(Rectangle))
                    _logicArea = SystemScreen.GetLogicArea(this.Window);
                return _logicArea;
            }
        }

        #region right

        /// <summary>
        /// 以一个可以单选的集合列表形式打开右侧栏
        /// </summary>
        /// <param name="items"></param>
        /// <param name="onSelected"></param>
        /// <param name="current"></param>
        public void OpenRight(IEnumerable<DrawerRightSelectData> items, Action<DrawerRightSelectData, DrawerRightSelectData> onSelected, DrawerRightSelectData current = null, Action callback = null)
        {
            var select = new DrawerRightSelect();
            select.SetItems(items, onSelected, current);
            OpenRight(select, callback);
        }

        public void OpenRight(UIElement content,Action callback = null)
        {
            this.CloseDrawer();

            right.SetContent(content);
            right.Visibility = Visibility.Visible;
            mask.Visibility = Visibility.Visible;

            right.UpdateLayout();//更新布局，使ActualWidth生效
            _OpenMask(0.5, true);
            Animations.ShowDrawer(this.right, Duration, callback);
        }


        public void CloseRight(Action callback = null)
        {
            _CloseMask();
            Animations.HiddenDrawer(this.right, Duration, () =>
            {
                this.right.Visibility = Visibility.Collapsed;
                this.mask.Visibility = Visibility.Collapsed;
                _onCloseMask.End();
                if (callback != null) callback();
            });
        }

        #endregion

        public event MaskStatusChangedEventHandler MaskStatusChanged;

        private void RaiseMaskStatusChanged(MaskStatus status)
        {
            if(this.MaskStatusChanged != null)
            {
                this.MaskStatusChanged(this, new MaskStatusChangedEventArgs(status));
            }
        }


        private void _OpenMask(double maskOpacity,bool touchClose, UIElement content = null, Action callback = null)
        {
            if(touchClose) this.mask.MouseUp += OnCloseMask;

            if (content != null) this.mask.Children.Add(content);
            RaiseMaskStatusChanged(MaskStatus.PreOpen);
            Animations.Opacity(this.mask, 0, maskOpacity, Duration, EasingMode.EaseOut,()=>
            {
                RaiseMaskStatusChanged(MaskStatus.Opened);
                if (callback != null) callback();
            });
        }

        private void _CloseMask(Action callback = null)
        {
            RaiseMaskStatusChanged(MaskStatus.PreClose);
            Animations.Opacity(mask, mask.Opacity, 0, Duration, EasingMode.EaseOut, () =>
            {
                this.mask.Children.Clear();
                this.mask.MouseUp -= OnCloseMask;
                RaiseMaskStatusChanged(MaskStatus.Closed);
                if (callback != null) callback();
            });
        }

        public void OpenMask(double maskOpacity, UIElement content = null,Action callback = null)
        {
            mask.Visibility = Visibility.Visible;
            _OpenMask(maskOpacity, false, content, callback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loadingMessage"></param>
        /// <param name="callback">可以通过该方法更新显示的文本</param>
        /// <returns></returns>
        public Action<string> OpenMask(string loadingMessage, Action callback = null)
        {
            var content = new LoadingText();
            content.Message = loadingMessage;
            content.HorizontalAlignment = HorizontalAlignment.Center;
            content.VerticalAlignment = VerticalAlignment.Center;
            this.OpenMask(0.95, content, callback);
            return (msg) =>
            {
                content.Message = msg;
            };
        }

        public void CloseMask(Action callback = null)
        {
            _CloseMask(()=>
            {
                mask.Visibility = Visibility.Collapsed;
                if (callback != null) callback();
            });
        }


        #region left

        public void OpenLeft(object content)
        {
            this.CloseDrawer();

            this.left.Visibility = Visibility.Visible;
            this.left.Value = content;
            mask.Visibility = Visibility.Visible;
            _OpenMask(0.5, true);
            Animations.ToSmallVisible(left, Duration);
        }

        public void CloseLeft()
        {
            _CloseMask();
            Animations.ToBigHidden(left, Duration, () =>
            {
                this.left.Visibility = Visibility.Collapsed;
                this.mask.Visibility = Visibility.Collapsed;
                _onCloseMask.End();
            });
        }

        #endregion

        #region bottom

        public void OpenBottom(UIElement content)
        {
            this.CloseDrawer();

            bottom.SetContent(content);
            bottom.Visibility = Visibility.Visible;

            bottom.UpdateLayout();//更新布局，使ActualHeight生效
            Animations.ShowDrawer(this.bottom, Duration);
        }


        public void CloseBottom()
        {
            Animations.HiddenDrawer(this.bottom, Duration, () =>
            {
                this.bottom.Visibility = Visibility.Collapsed;
            });
        }

        #endregion

        #region tip


        private volatile bool _tipRunning = false;
        private ConcurrentQueue<Action> _tipActions;
        private System.Timers.Timer _tipTimer;

        private void InitTip()
        {
            _tipActions = new ConcurrentQueue<Action>();
            _tipTimer = new System.Timers.Timer();
            _tipTimer.Elapsed += TipTimerElapsed;
            _tipTimer.Enabled = false;
        }

        private void TipTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(()=>
            {
                CloseTip();
            });
        }

        private void TipExecute(Action action)
        {
            _tipActions.Enqueue(action);
            TryTipContinue();
        }

        private void TryTipContinue()
        {
            if (_tipRunning) return;
            if (_tipActions.TryDequeue(out var action))
            {
                action();
            }
        }

        public void Tip(string content, int showTimes = 5000, string imageSrc = null, int textMaxWidth = 600)
        {
            TipExecute(() =>
            {
                _Tip(content, showTimes, imageSrc, textMaxWidth);
            });
        }

        private void _Tip(string content, int showTimes = 5000, string imageSrc = null, int textMaxWidth = 600)
        {
            _tipRunning = true;
            tip.Content = content;
            if (imageSrc != null) tip.ImageSrc = imageSrc;
            tip.TextMaxWidth = textMaxWidth;
            tip.Visibility = Visibility.Visible;
            tip.UpdateLayout();//更新布局，使ActualWidth生效

            Animations.ShowDrawer(tip, Duration, () =>
            {
                _tipTimer.Interval = showTimes;
                _tipTimer.Start();
            });
        }

        private void CloseTip()
        {
            _tipTimer.Stop();
            Animations.HiddenDrawer(tip, Duration, () =>
            {
                tip.Visibility = Visibility.Collapsed;
                _tipRunning = false;
                TryTipContinue();
            });
        }

        #endregion

        private void CloseDrawer()
        {
            if (this.right.Visibility == Visibility.Visible)
            {
                this.CloseRight();
            }

            if (this.left.Visibility == Visibility.Visible)
            {
                this.CloseLeft();
            }

            if(this.bottom.Visibility == Visibility.Visible)
            {
                this.CloseBottom();
            }

        }

        #region 刷新

        public void Refresh<T>()
        {
            var type = typeof(T);
            foreach (var ele in _elements)
            {
                if (ele.GetType().IsImplementOrEquals(type))
                {
                    _waitingRefresh.Add(ele);
                }
            }
        }

        private List<object> _waitingRefresh = new List<object>();

        private bool Reset(object ele)
        {
            try
            {
                if (_waitingRefresh.Contains(ele))
                {
                    var scene = ele as IScene;
                    if (scene != null)
                    {
                        scene.Reset();
                    }
                    _waitingRefresh.Remove(ele);
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                Work.Current.Catch(ex);
                return false;
            }
        }

        #endregion

        /// <summary>
        /// 使用UI线程执行方法
        /// </summary>
        /// <param name="action"></param>
        public static void UIInvoke(Action action)
        {
            if(Current != null)
            {
                Current.Dispatcher.Invoke(action);
            }
        }

        #region 离开相关

        /// <summary>
        /// 退出程序（调用该方法会清空所有的工作场景）
        /// </summary>
        public void Exit(Action<Action> preExit=null)
        {
            if(!string.IsNullOrEmpty(this.ExitConfirmMessage))
            {
                Work.Current.ShowTitleBar = false;
                //有提示语
                Work.Current.Go(new MessageBox()
                {
                    Message = this.ExitConfirmMessage,
                    CancelText = Strings.Cancel,
                    OKAction = ()=>
                    {
                        if (preExit != null)
                            preExit(() => _Exit());
                        else
                            _Exit();
                    },
                    CancelAction = ()=>
                    {
                        Work.Current.ShowTitleBar = true;
                        Work.Current.Back();
                    }
                });
            }
            else
            {
                if (preExit != null)
                    preExit(() => _Exit());
                else
                    _Exit();
            }
        }

        private void _Exit()
        {
            Clear();
            Application.Current.Shutdown();
        }

        /// <summary>
        /// 自定义点击标题栏离开的按钮的行为
        /// </summary>
        public Action TitleBarExitAction
        {
            get
            {
                return titleBar.ExitAction;
            }
            set
            {
                titleBar.ExitAction = value;
            }
        }


        #endregion

        #region 状态栏

        public void AddStatus(TitleBarStatus status)
        {
            this.titleBar.AddStatus(status);
        }

        public void ReplaceOrAddStatus(string targetName, TitleBarStatus status)
        {
            this.titleBar.ReplaceOrAddStatus(targetName, status);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetName">需要被替换的状态名称</param>
        /// <param name="status"></param>
        public void ReplaceStatus(string targetName, TitleBarStatus status)
        {
            this.titleBar.ReplaceStatus(targetName, status);
        }

        public void RemoveStatus(string targetName)
        {
            this.titleBar.RemoveStatus(targetName);
        }

        public void ClearStatus()
        {
            this.titleBar.ClearStatus();
        }

        #endregion

        /// <summary>
        /// 锁屏后无法触发任何事件
        /// </summary>
        public void LockScreen()
        {
            this.obstruction.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 解锁屏
        /// </summary>
        public void UnlockScreen()
        {
            this.obstruction.Visibility = Visibility.Collapsed;
        }

    }
}