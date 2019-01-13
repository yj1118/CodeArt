using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;

namespace CodeArt.Web.XamlControls.Metronic
{
    [TypeConverter(typeof(LayoutConverter))]
    public enum Layout
    {
        Inline = 1,
        /// <summary>
        /// label和input不在同一行的布局
        /// </summary>
        Wrap = 2,
        /// <summary>
        /// cell是以最小的形式呈现组件，常用于列表的查询条件
        /// </summary>
        Cell=3,
        /// <summary>
        /// 组件是隐藏的
        /// </summary>
        Hidden = 4
    }
}