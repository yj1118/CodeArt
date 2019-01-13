using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml.Controls
{
    [ContentProperty("Items")]
    public class CollectionPresenter : FrameworkElement
    {
        #region 依赖属性

        public static DependencyProperty ItemsProperty = DependencyProperty.Register<UIElementCollection, CollectionPresenter>("Items", () => { return null; });
    
        static CollectionPresenter()
        {
        }

        public UIElementCollection Items
        {
            get
            {
                return GetValue(ItemsProperty) as UIElementCollection;
            }
            set
            {
                SetValue(ItemsProperty, value);
            }
        }

        public static DependencyProperty ItemFormatProperty = DependencyProperty.Register<string, CollectionPresenter>("ItemFormat", () => { return null; });

        public string ItemFormat
        {
            get
            {
                return GetValue(ItemFormatProperty) as string;
            }
            set
            {
                SetValue(ItemFormatProperty, value);
            }
        }

        #endregion

        protected override void Draw(PageBrush brush)
        {
            var items = this.Items;
            if (items == null) return;

            var format = this.ItemFormat;
            if (string.IsNullOrEmpty(format))
            {
                items.Render(brush);
            }
            else
            {
                
                foreach(UIElement item in items)
                {
                    if (item == null) continue;
                    string itemCode = string.Empty;
                    using (var temp = XamlUtil.BrushPool.Borrow())
                    {
                        var newBrush = temp.Item;
                        item.Render(newBrush);
                        itemCode = newBrush.GetCode();
                        brush.DrawFormat(format, itemCode);
                    }
                }
            }
        }
    }
}
