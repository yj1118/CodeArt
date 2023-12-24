using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages;
using CodeArt.Runtime;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 绑定源查找器
    /// </summary>
    public interface IBindingSourceFinder
    {
        /// <summary>
        /// 查找绑定的源
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        object Find(object target);
    }
}