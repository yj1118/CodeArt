using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CodeArt.Web;
using CodeArt.Web.WebPages;
using CodeArt.Util;
using CodeArt.Concurrent;


namespace CodeArt.Web.WebPages.Xaml.Markup
{
    /// <summary>
    /// 标记扩展对象的基类
    /// </summary>
    public abstract class MarkupExtension
    {
        protected MarkupExtension()
        {
        }

        public abstract object ProvideValue(object target, DependencyProperty property);

        /// <summary>
        /// 为非依赖属性提供支持
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public abstract object ProvideValue(object target, string propertyName);
    }
}
