using CodeArt.Concurrent;
using CodeArt.DTO;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.XamlControls.Metronic
{
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.ListView.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class ListView : Control
    {
        public readonly static DependencyProperty HeaderProperty = DependencyProperty.Register<UIElementCollection, ListView>("Header", () => { return new UIElementCollection(); });

        public UIElementCollection Header
        {
            get
            {
                return GetValue(HeaderProperty) as UIElementCollection;
            }
            set
            {
                SetValue(HeaderProperty, value);
            }
        }


        public readonly static DependencyProperty ItemsProperty = DependencyProperty.Register<UIElementCollection, ListView>("Items", () => { return new UIElementCollection(); });

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

        public readonly static DependencyProperty ToolsProperty = DependencyProperty.Register<UIElementCollection, ListView>("Tools", () => { return new UIElementCollection(); });

        public UIElementCollection Tools
        {
            get
            {
                return GetValue(ToolsProperty) as UIElementCollection;
            }
            set
            {
                SetValue(ToolsProperty, value);
            }
        }


        public readonly static DependencyProperty ColumnProperty = DependencyProperty.Register<int, ListView>("Column", () => { return 2; });

        public string Column
        {
            get
            {
                return GetValue(ColumnProperty) as string;
            }
            set
            {
                SetValue(ColumnProperty, value);
            }
        }


        public readonly static DependencyProperty TitleFieldProperty = DependencyProperty.Register<string, ListView>("TitleField", () => { return string.Empty; });

        public string TitleField
        {
            get
            {
                return GetValue(TitleFieldProperty) as string;
            }
            set
            {
                SetValue(TitleFieldProperty, value);
            }
        }

        /// <summary>
        /// 是否显示翻页信息
        /// </summary>
        public readonly static DependencyProperty PaginationProperty = DependencyProperty.Register<bool, ListView>("Pagination", () => { return true; });

        public bool Pagination
        {
            get
            {
                return (bool)GetValue(PaginationProperty);
            }
            set
            {
                SetValue(PaginationProperty, value);
            }
        }

        public readonly static DependencyProperty PageSizeProperty = DependencyProperty.Register<int, ListView>("PageSize", () => { return 10; });

        public int PageSize
        {
            get
            {
                return (int)GetValue(PageSizeProperty);
            }
            set
            {
                SetValue(PageSizeProperty, value);
            }
        }

        public readonly static DependencyProperty PageLayoutProperty = DependencyProperty.Register<string, ListView>("PageLayout", () => { return " ['first', 'prev', 'page', 'next', 'last', 'limit']"; });

        public string PageLayout
        {
            get
            {
                return (string)GetValue(PageLayoutProperty);
            }
            set
            {
                SetValue(PageLayoutProperty, value);
            }
        }


        protected override void OnGotProxyCode(ref object baseValue)
        {
            using (var temp = StringPool.Borrow())
            {
                var sb = temp.Item;
                sb.Append("{");
                sb.Append("give:new $$metronic.listView(");
                sb.Append(GetConfigCode());
                sb.Append(")");
                sb.Append("}");
                baseValue = sb.ToString();
            }
        }

        private string GetConfigCode()
        {
            using (var temp = StringPool.Borrow())
            {
                var sb = temp.Item;
                sb.Append("{");
                sb.AppendFormat("pagination:{0},", this.Pagination.ToString().ToLower());
                sb.AppendFormat("pageSize:{0},", this.PageSize);
                sb.AppendFormat("pageLayout:{0}", this.PageLayout);
                sb.Append("}");
                return sb.ToString();
            }
        }


        public override void OnInit()
        {
            base.OnInit();
            this.RegisterScriptAction("Load", this.Load);
        }

        private IScriptView Load(ScriptView view)
        {
            if (this.LoadData == null) throw new XamlException(Strings.NoLoadDataMethod);
            var sender = view.GetSender<ListViewSE>();
            var data = this.LoadData(view, sender);
            view.Output(data);
            return view;
        }

        public Func<ScriptView, ListViewSE, DTObject> LoadData = null;


        public override DependencyObject GetChild(string childName)
        {
            return base.GetChild(childName) ?? this.Items.GetChild(childName) ?? this.Tools.GetChild(childName);
        }

        public override IEnumerable<UIElement> GetActionElement(string actionName)
        {
            return this.Combine(base.GetActionElement(actionName) , this.Items.GetActionElement(actionName) , this.Tools.GetActionElement(actionName));
        }

    }
}
