using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using CodeArt.Concurrent.Pattern;
using CodeArt.Util;

namespace CodeArt.WPF.Controls.Playstation
{
    public class ViewContainer : WorkScene
    {
        private Grid container;

        public ViewContainer()
        {
            this.DefaultStyleKey = typeof(ViewContainer);
        }

        public override void Exited()
        {
            foreach (var p in _loaded)
            {
                var view = p.Value;
                if (view != null)
                    view.Exited();
            }
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.container = GetTemplateChild("container") as Grid;
        }

        public ViewOwner Owner
        {
            get;
            private set;
        }

        public void Init(ViewOwner owner)
        {
            this.Owner = owner;
        }


        public static ViewContainer Instance
        {
            get;
            private set;
        }

        public View Current
        {
            get;
            private set;
        }

        private Dictionary<string, View> _loaded = new Dictionary<string, View>();

        private ActionPipelineSlim _queue = new ActionPipelineSlim();

        /// <summary>
        /// 打开视图
        /// </summary>
        public void OpenView(string name)
        {
            _queue.Queue((complete) =>
            {
                if (IsDisabled(name))
                {
                    complete();
                    return;
                }

                if (this.Current != null && this.Current.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    complete();
                    return;
                }

                var view = _AddView(name);
                if (view == null)
                {
                    complete();
                    return;
                }

                var old = this.Current;
                if (old != null)
                {
                    if (old.ViewName == view.ViewName)
                    {
                        complete();
                        return;
                    }
                    _queue.IncrementAsync();
                    old.Close(complete);
                }
                _queue.IncrementAsync();
                view.Open(complete);
                this.Current = view;
                OnViewChanged(old, this.Current);
            });
        }

        public void RefreshView(string name)
        {
            _queue.Queue((complete) =>
            {
                View view = null;
                if (_loaded.TryGetValue(name, out view))
                {
                    view.Reset();
                }
                complete();
            });
        }

        /// <summary>
        /// 预加载视图，但不打开
        /// </summary>
        public void PreLoadView(string name)
        {
            _queue.Queue((complete) =>
            {
                var view = _AddView(name);
                view.SimulateOpen();
                view.SimulateClose();
                complete();
            });
        }

        /// <summary>
        /// 追加视图，但不打开
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private View _AddView(string name)
        {
            View view = null;
            if (!_loaded.TryGetValue(name, out view))
            {
                view = this.Owner.CreateView(name);
                _loaded.Add(name, view);
                if (view == null) return null;
                this.container.Children.Add(view);
                this.MakeChildsLoaded(view);
            }
            return view;
        }

        /// <summary>
        /// 关闭视图
        /// </summary>
        /// <param name="name"></param>
        public void CloseView()
        {
            _queue.Queue((complete) =>
            {
                if (this.Current == null)
                {
                    complete();
                    return;
                }
                this.Current.Close(complete);
            });
        }

        public event Action<View, View> ViewChanged;

        private void OnViewChanged(View old, View current)
        {
            if (this.ViewChanged != null)
                this.ViewChanged(old, current);
        }

        public event Action Maxed;

        /// <summary>
        /// 最大化工作区
        /// </summary>
        public void Maximize()
        {
            OnMaxed();
        }

        private void OnMaxed()
        {
            if (this.Maxed != null)
            {
                this.Maxed();
            }
        }

        public event Action Reduced;

        /// <summary>
        /// 还原工作区
        /// </summary>
        public void Reduce()
        {
            OnReduced();
        }

        private void OnReduced()
        {
            if (this.Reduced != null)
            {
                this.Reduced();
            }
        }


        public event Action<string> ViewDisabled;

        private void OnViewDisabled(string viewName)
        {
            if (this.ViewDisabled != null)
                this.ViewDisabled(viewName);
        }

        public event Action<string> ViewEnabled;

        private void OnViewEnabled(string viewName)
        {
            if (this.ViewEnabled != null)
                this.ViewEnabled(viewName);
        }


        private List<string> _disabledViews = new List<string>();

        private bool IsDisabled(string name)
        {
            return _disabledViews.Contains(name);
        }

        public void DisableView(string name)
        {
            _queue.Queue((complete) =>
            {
                if (this.Current != null && this.Current.Name == name)
                {
                    Work.Current.Message(Strings.CanNotDisableUsedView, false);
                    complete();
                    return;
                }

                if (!_disabledViews.Contains(name))
                {
                    _disabledViews.Add(name);
                    OnViewDisabled(name);
                }
                complete();
            });
        }

        public void EnableView(string name)
        {
            _queue.Queue((complete) =>
            {
                if (_disabledViews.Remove(name))
                {
                    OnViewEnabled(name);
                }
                complete();
            });
        }

    }
}
