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
    public class ItemsPresenter : ItemsPresenterBase
    {
        protected override void Draw(PageBrush brush)
        {
            var owner = GetOwner();
            if (owner == null) return;

            var items = owner.Items;

            if (items.Count > 0)
            {
                for (var i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var itemTemplate = owner.CreateItemTemplate(i);
                    itemTemplate.TemplateParent = item;
                    itemTemplate.Render(brush);
                }
            }
            else
            {
                var source = owner.ItemsSource;
                if(source != null)
                {
                    var i = 0;
                    foreach(var dataContext in source)
                    {
                        var item = CreateItem(i); //创建内容项
                        item.DataContext = dataContext;

                        var itemTemplate = owner.CreateItemTemplate(i);
                        itemTemplate.TemplateParent = item;
                        itemTemplate.Render(brush);
                        i++;
                    }
                }
            }


        }

        private List<ContentPresenter> _items = new List<ContentPresenter>();

        private ContentPresenter CreateItem(int itemIndex)
        {
            if (itemIndex < _items.Count)
            {
                var item = _items[itemIndex];
                item.LoadPinned();
                item.OnLoad();
                return item;
            }
            else
            {
                lock (_items)
                {
                    if (itemIndex < _items.Count) return _items[itemIndex];
                    var item = new ContentPresenter();
                    item.Parent = this;
                    _items.Add(item);
                    return item;
                }
            }
        }


    }
}
