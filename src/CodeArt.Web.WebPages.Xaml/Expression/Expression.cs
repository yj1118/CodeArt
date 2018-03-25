using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CodeArt.Web;
using CodeArt.Web.WebPages;
using CodeArt.Util;
using CodeArt.Concurrent;


namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 表达式用于定义属性之间的关联性
    /// 每个表达式只对挂载的属性负责，对于该属性会有唯一的实例与之相关联
    /// </summary>
    public class Expression
    {
        public Expression()
        {
        }

        /// <summary>
        /// 源集合
        /// </summary>
        /// <returns></returns>
        public virtual DependencySource[] GetSources()
        {
            return null;
        }

        public virtual object GetValue(DependencyObject d, DependencyProperty dp)
        {
            return null;
        }

        public virtual object GetValue(object o, string propertyName)
        {
            return null;
        }


        /// <summary>
        /// 允许表达式存储设定值
        /// </summary>
        /// <param name="d"></param>
        /// <param name="dp"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool SetValue(DependencyObject d, DependencyProperty dp, object value)
        {
            return false;
        }

        public virtual bool SetValue(object o, string propertyName, object value)
        {
            return false;
        }

        /// <summary>
        /// 当表达式被设置到一个属性上时发生通知
        /// </summary>
        public virtual void OnAttach(DependencyObject d, DependencyProperty dp)
        {
        }


        public virtual void OnAttach(object obj, string propertyName)
        {
        }

        /// <summary>
        /// 当表达式被从一个属性上移除时发生通知
        /// </summary>
        public virtual void OnDetach(DependencyObject d, DependencyProperty dp)
        {
        }

        public virtual void OnDetach(object obj, string propertyName)
        {
        }

    }
}
