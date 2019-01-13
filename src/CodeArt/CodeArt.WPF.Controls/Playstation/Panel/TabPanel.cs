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
using System.Collections;
using System.Windows.Markup;
using System.Threading;

using CodeArt.WPF.UI;
using CodeArt.Concurrent.Pattern;

namespace CodeArt.WPF.Controls.Playstation
{
    [ContentProperty("Items")]
    public class TabPanel : Control
    {
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(UIElementCollection), typeof(TabPanel), new PropertyMetadata(null));

        public UIElementCollection Items
        {
            get {
                var value = GetValue(ItemsProperty);
                if(value == null)
                {
                    value = new UIElementCollection(this, this);
                    SetValue(ItemsProperty, value);
                }
                return (UIElementCollection)GetValue(ItemsProperty);
            }
            set { SetValue(ItemsProperty, value); }
        }


        public UIElementCollection ActualItems
        {
            get
            {
                return this.container.Children;
            }
        }


        public TabPanel()
        {
            this.DefaultStyleKey = typeof(TabPanel);
        }

        private Grid container;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            container = GetTemplateChild("container") as Grid;

            UIElement[] temp = new UIElement[this.Items.Count];
            this.Items.CopyTo(temp, 0);
            foreach (var item in temp)
            {
                this.Items.Remove(item);
                item.Visibility = Visibility.Collapsed;
                container.Children.Add(item);
            }
        }


        private ActionPipelineSlim _pipeline = new ActionPipelineSlim();

        private void _ShowItem(string itemName, Func<UIElement> createItem, Action complete)
        {
            if (_itemName == itemName) return;
            var finded = false;

            foreach (FrameworkElement item in container.Children)
            {
                var show = GetItem(item) == itemName;
                if (show)
                {
                    _itemName = itemName;
                    item.Visibility = Visibility.Visible;
                    _pipeline.IncrementAsync();
                    FadeInOut.RaisePreFadeIn(item);
                    Animations.OpacityOut(item, 500, EasingMode.EaseOut, () =>
                    {
                        FadeInOut.RaiseFadedIn(item);
                        complete();
                    });
                    finded = true;
                }
                else
                {
                    if (item.Visibility == Visibility.Visible)
                    {
                        _pipeline.IncrementAsync();
                        FadeInOut.RaisePreFadeOut(item);
                        item.Visibility = Visibility.Collapsed;
                        Animations.OpacityIn(item, 500, EasingMode.EaseOut, () =>
                        {
                            item.Visibility = Visibility.Collapsed;
                            FadeInOut.RaiseFadedOut(item);
                            complete();
                        });
                    }
                }
            }

            if (!finded && createItem != null)
            {
                //自动添加项,并显示
                var item = createItem();
                TabPanel.SetItem(item, itemName);
                container.Children.Add(item);
                item.Visibility = Visibility.Visible;

                _pipeline.IncrementAsync();
                FadeInOut.RaisePreFadeIn(item);
                Animations.OpacityOut(item, 500, EasingMode.EaseOut, () =>
                {
                    FadeInOut.RaiseFadedIn(item);
                    complete();
                });
                _itemName = itemName;
            }
        }


        private string _itemName = null;

        public void ShowItem(string itemName)
        {
            ShowItem(itemName, null);
        }

        public void ShowItem(string itemName, Func<UIElement> createItem)
        {
            _pipeline.Queue((complete) =>
            {
                _ShowItem(itemName, createItem, complete);
            });
        }

        public UIElement FindItem(string itemName)
        {
            foreach (UIElement item in container.Children)
            {
                if (GetItem(item) == itemName) return item;
            }
            return null;
        }

        public bool TryAddItem(string itemName, Func<UIElement> createItem)
        {
            var item = FindItem(itemName);
            if (item != null) return false;
            
            item = createItem();
            item.Visibility = Visibility.Collapsed;
            TabPanel.SetItem(item, itemName);
            container.Children.Add(item);

            return true;
        }


        public static readonly DependencyProperty ItemProperty = DependencyProperty.RegisterAttached("Item", typeof(string), typeof(TabPanel));

        public static string GetItem(DependencyObject obj)
        {
            return (string)obj.GetValue(ItemProperty);
        }

        public static void SetItem(DependencyObject obj, string value)
        {
            obj.SetValue(ItemProperty, value);
        }

        public bool IsShow(DependencyObject target)
        {
            var itemName = GetItem(target);
            return _itemName == itemName;
        }
    }
}