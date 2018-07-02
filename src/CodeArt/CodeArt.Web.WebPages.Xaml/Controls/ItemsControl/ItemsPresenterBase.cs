using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml.Controls
{
    /// <summary>
    /// 相集合呈现器
    /// </summary>
    public abstract class ItemsPresenterBase : FrameworkElement
    {
        protected ItemsControl GetOwner()
        {
            return this.FindParent<ItemsControl>();
        }
    }



}
