using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml.Controls
{
    [ContentProperty("Items")]
    public class ItemsControl : Control, IItemsControl
    {
        public static DependencyProperty ItemsProperty;

        private static DependencyProperty ItemTemplateProperty;

        public static DependencyProperty ItemsPanelProperty;

        public static DependencyProperty ItemsSourceProperty;

        static ItemsControl()
        {
            var itemsMetadata = new PropertyMetadata(() => { return new ItemCollection(); });
            ItemsProperty = DependencyProperty.Register<ItemCollection, ItemsControl>("Items", itemsMetadata);

            var itemTemplateMetadata = new PropertyMetadata(() => { return null; });
            ItemTemplateProperty = DependencyProperty.Register<FrameworkTemplate, ItemsControl>("ItemTemplate", itemTemplateMetadata);

            var itemsPanelMetadata = new PropertyMetadata(() => { return null; });
            ItemsPanelProperty = DependencyProperty.Register<FrameworkTemplate, ItemsControl>("ItemsPanel", itemsPanelMetadata);

            var itemsSourceMetadata = new PropertyMetadata(() => { return null; });
            ItemsSourceProperty = DependencyProperty.Register<IEnumerable, ItemsControl>("ItemsSource", itemsSourceMetadata);
        }


        public ItemCollection Items
        {
            get
            {
                return GetValue(ItemsProperty) as ItemCollection;
            }
            set
            {
                SetValue(ItemsProperty, value);
            }
        }

        public FrameworkTemplate ItemsPanel
        {
            get
            {
                return GetValue(ItemsPanelProperty) as FrameworkTemplate;
            }
            set
            {
                SetValue(ItemsPanelProperty, value);
            }
        }

        public FrameworkTemplate ItemTemplate
        {
            get
            {
                return GetValue(ItemTemplateProperty) as FrameworkTemplate;
            }
            set
            {
                SetValue(ItemTemplateProperty, value);
            }
        }

        public IEnumerable ItemsSource
        {
            get
            {
                return GetValue(ItemsSourceProperty) as IEnumerable;
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public override void OnInit()
        {
            if (this.ItemsPanel == null)
                this.ItemsPanel = TemplateFactory.Create<ControlTemplate>(this, ItemsControl.ItemsPanelProperty);

            if (this.ItemTemplate == null)
                this.ItemTemplate = TemplateFactory.Create<ControlTemplate>(this, ItemsControl.ItemTemplateProperty);

            if (this.ItemTemplate == null)
                throw new XamlException("没有得到类型"+this.GetType().FullName+ "的ItemTemplate");

            base.OnInit();
        }

        private List<FrameworkTemplate> _itemTemplates = new List<FrameworkTemplate>();

        public FrameworkTemplate CreateItemTemplate(int itemIndex)
        {
            if (itemIndex < _itemTemplates.Count)
            {
                var itemTemplate = _itemTemplates[itemIndex];
                itemTemplate.LoadPinned();
                itemTemplate.OnLoad();
                return itemTemplate;
            }
            else
            {
                lock (_itemTemplates)
                {
                    if (itemIndex < _itemTemplates.Count) return _itemTemplates[itemIndex];
                    var itemTemplate = this.ItemTemplate.Clone() as FrameworkTemplate;
                    if (itemTemplate == null) throw new XamlException("获取项模板失败");
                    _itemTemplates.Add(itemTemplate);
                    return itemTemplate;
                }
            }
        }
    }
}
