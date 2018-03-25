using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.ModuleNest;
using CodeArt.Web.WebPages;
using CodeArt.Web.XamlControls.Bootstrap;

namespace CodeArt.Web.XamlControls.Metronic
{
    [ContentProperty("Items")]
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.TodoStyleRepeater.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class TodoStyleRepeater : Control
    {
        public static DependencyProperty ItemsProperty { get; private set; }
        public static DependencyProperty DataProperty { get; private set; }
        public static DependencyProperty ItemXSProperty { get; private set; }
        public static DependencyProperty ItemSMProperty { get; private set; }
        public static DependencyProperty ItemMDProperty { get; private set; }
        public static DependencyProperty ItemLGProperty { get; private set; }

        private static string[] Colors = new string[] { "green", "red", "blue", "purple", "yellow" };

        static TodoStyleRepeater()
        {
            var itemsMetadata = new PropertyMetadata(() => { return new UIElementCollection(); });
            ItemsProperty = DependencyProperty.Register<UIElementCollection, TodoStyleRepeater>("Items", itemsMetadata);

            var dataMetadata = new PropertyMetadata(() => { return DTObject.Empty; });
            DataProperty = DependencyProperty.Register<DTObject, TodoStyleRepeater>("Data", dataMetadata);

            var itemXSMetadata = new PropertyMetadata(() => { return string.Empty; });
            ItemXSProperty = DependencyProperty.Register<string, TodoStyleRepeater>("ItemXS", itemXSMetadata);

            var itemSMMetadata = new PropertyMetadata(() => { return string.Empty; });
            ItemSMProperty = DependencyProperty.Register<string, TodoStyleRepeater>("ItemSM", itemSMMetadata);

            var itemMDMetadata = new PropertyMetadata(() => { return string.Empty; });
            ItemMDProperty = DependencyProperty.Register<string, TodoStyleRepeater>("ItemMD", itemMDMetadata);

            var itemLGMetadata = new PropertyMetadata(() => { return string.Empty; });
            ItemLGProperty = DependencyProperty.Register<string, TodoStyleRepeater>("ItemLG", itemLGMetadata);
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

        public DTObject Data
        {
            get
            {
                return GetValue(DataProperty) as DTObject;
            }
            set
            {
                SetValue(DataProperty, value);
            }
        }

        public string ItemXS
        {
            get
            {
                return GetValue(ItemXSProperty) as string;
            }
            set
            {
                SetValue(ItemXSProperty, value);
            }
        }

        public string ItemSM
        {
            get
            {
                return GetValue(ItemSMProperty) as string;
            }
            set
            {
                SetValue(ItemSMProperty, value);
            }
        }

        public string ItemMD
        {
            get
            {
                return GetValue(ItemMDProperty) as string;
            }
            set
            {
                SetValue(ItemMDProperty, value);
            }
        }

        public string ItemLG
        {
            get
            {
                return GetValue(ItemLGProperty) as string;
            }
            set
            {
                SetValue(ItemLGProperty, value);
            }
        }

        public TodoStyleRepeater()
        {
        }

        protected override void OnPreRender()
        {
            base.OnPreRender();

            var list = Data.GetList("items", false);
            if (list == null || list.Count == 0) return;
            int index = 0;
            var columnClass = GetColumnClass();
            foreach (var data in list)
            {
                TodoStyleRepeaterItem item = new TodoStyleRepeaterItem();

                item.BorderColor = GetBorderColor(index);
                item.Title = data.GetValue<string>("title", string.Empty);
                item.Contents = GetContents(data).ToArray();
                item.Time = data.GetValue<string>("time", string.Empty);
                item.Tags = GetTags(data);
                item.ColumnClass = columnClass;
                var url = data.GetValue<string>("url", string.Empty);
                if (string.IsNullOrEmpty(url)) url = "javascript:;";
                item.Url = url;

                Items.Add(item);
                index++;
            }
        }

        private static string[] _widths = { "xs", "sm", "md", "lg" }; //顺序不能变

        private string GetColumnClass()
        {
            var code = new StringBuilder();
            foreach (var widthName in _widths)
            {
                string width = string.Empty;
                switch(widthName)
                {
                    case "xs":
                        {
                            width = this.ItemXS;
                            break;
                        }
                    case "sm":
                        {
                            width = this.ItemSM;
                            break;
                        }
                    case "md":
                        {
                            width = this.ItemMD;
                            break;
                        }
                    case "lg":
                        {
                            width = this.ItemLG;
                            break;
                        }
                }
                if (!string.IsNullOrEmpty(width))
                    code.AppendFormat("col-{0}-{1} ", widthName, width);
            }
            return code.ToString();
        }


        private string[] GetTags(DTObject obj)
        {
            var tagList = obj.GetList("tags", false);
            if (tagList == null || tagList.Count == 0) return Array.Empty<string>();

            var list = new List<string>();

            foreach (DTObject tag in tagList)
            {
                list.Add(tag.GetValue<string>());
            }

            return list.ToArray();
        }

        private string[] GetContents(DTObject obj)
        {
            var contents = obj.GetList("contents");
            if (contents == null || contents.Count == 0) return Array.Empty<string>();

            var list = new List<string>();
            foreach (DTObject content in contents)
            {
                list.Add(content.GetValue<string>());
            }
            return list.ToArray();
        }

        private string GetBorderColor(int index)
        {
            var i = index % 5;
            return Colors[i];
        }

    }
}
