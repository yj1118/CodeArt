using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 在获取属性值之前触发的强制回调方法，该方法可以更改获取的值
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="baseValue"></param>
    public delegate void GotValueCallback(DependencyObject obj, ref object baseValue);
}
