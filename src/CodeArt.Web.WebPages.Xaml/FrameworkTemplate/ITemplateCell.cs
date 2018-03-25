using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 表示元素可以作为模板的组成部分使用
    /// </summary>
    public interface ITemplateCell
    {
        /// <summary>
        ///  元素所属的模板
        /// </summary>
        FrameworkTemplate BelongTemplate { get; }

    }
}
