using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml.Controls;

namespace CodeArt.Web.XamlControls
{
    /// <summary>
    /// class样式附加器
    /// </summary>
    [SafeAccess]
    public abstract class ClassAppender : IValueConverter
    {
        /// <summary>
        /// 将绑定源的值转换为需要绑定的目标格式值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public object Convert(object value, object parameter)
        {
            var ui = RenderContext.Current.Target as UIElement;
            if (ui == null) return value;

            var templateParent = ui.GetTemplateParent() as UIElement;
            if (templateParent == null) return value;

            var parentClass = templateParent.Class;
            var baseClass = GetBaseClass(ui, templateParent);

            return string.IsNullOrEmpty(parentClass) ? baseClass : string.Format("{0} {1}", baseClass, parentClass);
        }

        protected abstract string GetBaseClass(UIElement target, UIElement templateParent);

    }
}