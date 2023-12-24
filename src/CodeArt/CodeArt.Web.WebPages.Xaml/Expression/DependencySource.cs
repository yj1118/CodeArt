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
    /// 绑定源
    /// </summary>
    public sealed class DependencySource
    {
        public DependencySource(DependencyObject d, DependencyProperty dp)
        {
            _d = d;
            _dp = dp;
        }

        /// <summary>
        /// 源对象
        /// </summary>
        public DependencyObject DependencyObject
        {
            get { return _d; }
        }

        /// <summary>
        /// 源属性
        /// </summary>
        public DependencyProperty DependencyProperty
        {
            get { return _dp; }
        }

        private DependencyObject _d;
        private DependencyProperty _dp;
    }
}
