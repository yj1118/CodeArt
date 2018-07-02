using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml.Controls
{
    /// <summary>
    /// 集合面板呈现器
    /// </summary>
    public class ItemsPanelPresenter : ItemsPresenterBase
    {
        protected override void Draw(PageBrush brush)
        {
            var owner = GetOwner();
            if (owner == null) return;
            owner.ItemsPanel.Render(brush);
        }
    }
}
