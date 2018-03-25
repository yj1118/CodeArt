using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.Concurrent;

namespace CodeArt.Web.XamlControls.Metronic
{
    [SafeAccess]
    internal abstract class InputPropertyConverter : IValueConverter
    {
        /// <summary>
        /// 查找被绑定的目标所在的input组件对象
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public Input FindInput(object parameter)
        {
            var e = parameter as ITemplateCell;
            if (e == null) return null;
            var parent = (e.BelongTemplate != null ? e.BelongTemplate.TemplateParent : null) as UIElement;
            if (parent == null) return null;
            return parent.Parent as Input;
        }


        /// <summary>
        /// 将绑定源的值转换为需要绑定的目标格式值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public abstract object Convert(object value, object parameter);

    }
}