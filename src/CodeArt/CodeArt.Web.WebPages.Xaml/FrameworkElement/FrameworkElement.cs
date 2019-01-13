using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;
using CodeArt.HtmlWrapper;

using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    public abstract class FrameworkElement : UIElement, IFrameworkElement
    {
        public static readonly DependencyProperty ResourcesProperty;

        public static readonly DependencyProperty XamlStyleProperty;

        public static readonly DependencyProperty DataContextProperty;

        static FrameworkElement()
        {
            var resourcesMetadata = new PropertyMetadata(() => { return null; });
            ResourcesProperty = DependencyProperty.Register<ResourceDictionary, FrameworkElement>("Resources", resourcesMetadata);

            var xamlStyleMetadata = new PropertyMetadata(() => { return null; }, OnXamlStyleChanged);
            XamlStyleProperty = DependencyProperty.Register<XamlStyle, FrameworkElement>("XamlStyle", xamlStyleMetadata);

            var dataContextMetadata = new PropertyMetadata(() => { return null; }, OnDataContextChanged);
            DataContextProperty = DependencyProperty.Register<object, FrameworkElement>("DataContext", dataContextMetadata);
        }

        private static void OnXamlStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as FrameworkElement).OnXamlStyleChanged(e);
        }

        protected virtual void OnXamlStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            (e.NewValue as XamlStyle)?.Apply(this);
        }


        private static void OnDataContextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as FrameworkElement).OnDataContextChanged(e);
        }

        protected virtual void OnDataContextChanged(DependencyPropertyChangedEventArgs e)
        {
            if(this.DataContextChanged != null)
            {
                this.DataContextChanged(this, e);
            }
        }


        public ResourceDictionary Resources
        {
            get
            {
                return GetValue(ResourcesProperty) as ResourceDictionary;
            }
            set
            {
                SetValue(ResourcesProperty, value);
            }
        }

        public object FindResource(object resourceKey)
        {
            if(this.Resources != null)
            {
                var result = this.Resources[resourceKey];
                if (result != null) return result;
            }

            //如果没有找到资源，那么从父级或者全局应用里面找
            var parent = this.Parent as FrameworkElement;
            if (parent == null) return Application.Current.FindResource(resourceKey);
            return parent.FindResource(resourceKey);
        }

        public XamlStyle XamlStyle
        {
            get
            {
                return GetValue(XamlStyleProperty) as XamlStyle;
            }
            set
            {
                SetValue(XamlStyleProperty, value);
            }
        }

        /// <summary>
        /// 获取或设置元素在参与数据绑定时的数据上下文
        /// </summary>
        public object DataContext
        {
            get
            {
                return GetValue(DataContextProperty) as object;
            }
            set
            {
                SetValue(DataContextProperty, value);
            }
        }

        public event DependencyPropertyChangedEventHandler DataContextChanged;

        /// <summary>
        /// 初始化组件完毕时触发该事件，在该事件对组件赋值可固化值
        /// </summary>
        public event RoutedEventHandler Inited;

        public override void OnInit()
        {
            EventHandlerAttribute.Process(this, RoutedEvent.Init);

            if (this.Inited != null)
            {
                this.Inited(this, this);
            }
            base.OnInit();
        }

        public event RoutedEventHandler Loaded;

        public override void OnLoad()
        {
            EventHandlerAttribute.Process(this, RoutedEvent.Load);

            if (this.Loaded != null)
            {
                this.Loaded(this, this);
            }
            base.OnLoad();
        }
    }
}
