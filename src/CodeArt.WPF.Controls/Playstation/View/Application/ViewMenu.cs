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

using CodeArt.Util;

namespace CodeArt.WPF.Controls.Playstation
{
    public abstract class ViewMenu : Control
    {
        public ViewMenu()
        {
        }

        private ViewOwner _owner;
        private ViewContainer _container;

        public void Init(ViewOwner owner, ViewContainer container)
        {
            _owner = owner;

            _container = container;
            _container.ViewChanged += OnViewChanged;
            _container.Maxed += OnWorkMaxed;
            _container.Reduced += OnWorkReduced;
            _container.ViewDisabled += OnViewDisabled;
            _container.ViewEnabled += OnViewEnabled;

            BuildMenuItems();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _panel = GetPanel();
        }


        private Panel _panel;

        private List<ViewMenuButton> _buttons = new List<ViewMenuButton>();

        protected virtual void BuildMenuItems()
        {
            foreach (var definition in _owner.ViewDefinitions)
            {
                CreateButton(definition);
            }
        }

        private void CreateButton(ViewDefinition definition)
        {
            var button = CreateButton();
            button.Name = definition.Name;
            button.Text = definition.TextName;
            button.ImageSrc = definition.ImageSrc;
            button.MouseUp += definition.RaiseButtonMouseUp;
            button.Init(_container);
            _panel.Children.Add(button);
            _buttons.Add(button);
        }

        protected void AddDefinition(ViewDefinition definition)
        {
            _owner.ViewDefinitions.Add(definition);
            CreateButton(definition);
        }


        /// <summary>
        /// 获取装载菜单按钮的面板对象
        /// </summary>
        /// <returns></returns>
        protected abstract Panel GetPanel();


        protected abstract ViewMenuButton CreateButton();

        private void OnViewEnabled(string viewName)
        {
            var btn = FindButton(viewName);
            btn.Status = FloatStatus.Focus;
        }

        private void OnViewDisabled(string viewName)
        {
            var btn = FindButton(viewName);
            btn.Status = FloatStatus.LoseFocus;
        }

        private void OnWorkReduced()
        {
            this.Visibility = Visibility.Visible;
        }

        private void OnWorkMaxed()
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void OnViewChanged(View old, View current)
        {
            if (old != null)
            {
                var viewName = old.ViewName;
                var btn = FindButton(viewName);
                btn.Status = FloatStatus.LoseFocus;
            }

            if (current != null)
            {
                var viewName = current.ViewName;
                var btn = FindButton(viewName);
                btn.Status = FloatStatus.Focus;
            }
        }

        private ViewMenuButton FindButton(string name)
        {
            var btn = _buttons.FirstOrDefault((button) =>
            {
                return button.Name.EqualsIgnoreCase(name);
            });
            if (btn == null) throw new InvalidOperationException(string.Format(Strings.NoDesktopButton, name));
            return btn;
        }

    }
}
