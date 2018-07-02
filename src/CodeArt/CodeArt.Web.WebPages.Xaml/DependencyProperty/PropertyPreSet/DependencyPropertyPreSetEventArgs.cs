using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.WebPages.Xaml
{
    public sealed class DependencyPropertyPreSetEventArgs : IDependencyPropertyPreSetEventArgs
    {

        /// <summary>
        /// 获取或设置是否允许执行赋值操作
        /// </summary>
        public bool Allow { get; set; }

        /// <summary>
        /// 获取或设置赋予的值
        /// </summary>
        public object Value { get; set; }

        public DependencyProperty Property { get; private set; }


        public DependencyPropertyPreSetEventArgs(DependencyProperty property, object value)
        {
            this.Property = property;
            this.Value = value;
            this.Allow = true; //默认是允许更改的
        }

    }
}
