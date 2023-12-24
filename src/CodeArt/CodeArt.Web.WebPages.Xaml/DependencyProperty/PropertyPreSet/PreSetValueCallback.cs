using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 在设置属性值之前触发的强制回调方法，该方法可以更改设置的值
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="baseValue"></param>
    /// <returns></returns>
    public delegate bool PreSetValueCallback(DependencyObject obj, ref object baseValue);
}
