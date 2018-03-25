using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.WebPages.Xaml
{
    public sealed class DependencyPropertyGotEventArgs : IDependencyPropertyGotEventArgs
    {
        /// <summary>
        /// 获取或设置赋予的值
        /// </summary>
        public object Value { get; set; }

        public DependencyProperty Property { get; private set; }


        public DependencyPropertyGotEventArgs(DependencyProperty property, object value)
        {
            this.Property = property;
            this.Value = value;
        }

    }
}
