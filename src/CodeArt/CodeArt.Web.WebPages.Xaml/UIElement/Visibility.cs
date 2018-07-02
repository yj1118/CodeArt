using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    [TypeConverter(typeof(VisibilityConverter))]
    public enum Visibility
    {
        //
        // 摘要:
        //     显示元素。
        Visible = 0,
        //
        // 摘要:
        //     不显示元素，且不为其保留布局空间。
        Collapsed = 2
    }
}
