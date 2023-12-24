using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.WebPages.Xaml.Controls
{
    public interface IItemsControl
    {
        ItemCollection Items { get; }
        FrameworkTemplate ItemsPanel { get; set; }

        /// <summary>
        /// 创建项模板
        /// </summary>
        /// <returns></returns>
        FrameworkTemplate CreateItemTemplate(int itemIndex);
    }
}