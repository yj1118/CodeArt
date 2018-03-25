using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.WebPages.Xaml
{
    public interface IDependencyObject
    {
        /// <summary>
        /// 属性被更改时触发
        /// </summary>
        event DependencyPropertyChangedEventHandler PropertyChanged;

        event DependencyPropertyPreSetEventHandler PropertyPreSet;

        event DependencyPropertyGotEventHandler PropertyGot;
    }
}
