using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CodeArt.WPF.Controls.Playstation
{
    public abstract class ViewDefinition : UIElement
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty PreLoadProperty = DependencyProperty.Register("PreLoad", typeof(bool), typeof(ViewDefinition), new PropertyMetadata(false));

        /// <summary>
        /// 是否预加载视图
        /// </summary>
        public bool PreLoad
        {
            get
            {
                return (bool)GetValue(PreLoadProperty);
            }
            set { SetValue(PreLoadProperty, value); }
        }

        public string Name
        {
            get
            {
                return this.GetName();
            }
        }

        public string TextName
        {
            get
            {
                return this.GetTextName();
            }
        }

        /// <summary>
        /// 视图的唯一名称，要符合.NET的命名约定
        /// </summary>
        /// <returns></returns>
        protected abstract string GetName();

        /// <summary>
        /// 视图的文本名称
        /// </summary>
        /// <returns></returns>
        protected abstract string GetTextName();

        public string ImageSrc
        {
            get
            {
                return GetImageSrc();
            }
        }

        protected abstract string GetImageSrc();

        public View CreateView()
        {
            var view = ImplCreateView();
            if (view != null)
            {
                view.ViewName = this.Name;
                view.ViewTextName = this.TextName;
            }
            return view;
        }

        protected abstract View ImplCreateView();

        public static readonly RoutedEvent ButtonMouseUpEvent = EventManager.RegisterRoutedEvent(
            "MouseUp", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(ViewDefinition));

        public event RoutedEventHandler ButtonMouseUp
        {
            add { AddHandler(ButtonMouseUpEvent, value); }
            remove { RemoveHandler(ButtonMouseUpEvent, value); }

        }

        internal void RaiseButtonMouseUp(object sender, MouseButtonEventArgs e)
        {
            var arg = new RoutedEventArgs(ButtonMouseUpEvent, this);
            base.RaiseEvent(arg);
        }


    }
}
