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
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.PageTurn.Template.html,CodeArt.Web.XamlControls.Metronic")]
    [ContentProperty("Row")]
    public class PageTurn : Control
    {
        public static DependencyProperty RowProperty = DependencyProperty.Register<UIElementCollection, PageTurn>("Row", () => { return new UIElementCollection(); });

        public UIElementCollection Row
        {
            get
            {
                return (UIElementCollection)GetValue(RowProperty);
            }
            set
            {
                SetValue(RowProperty, value);
            }
        }

        /// <summary>
        /// 是否显示翻页信息
        /// </summary>
        public readonly static DependencyProperty PaginationProperty = DependencyProperty.Register<bool, PageTurn>("Pagination", () => { return true; });

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

        public readonly static DependencyProperty PageSizeProperty = DependencyProperty.Register<int, PageTurn>("PageSize", () => { return 50; });

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

        public readonly static DependencyProperty PageLayoutProperty = DependencyProperty.Register<string, PageTurn>("PageLayout", () => { return " ['first', 'page', 'last']"; });

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

        public readonly static DependencyProperty EmptyProperty = DependencyProperty.Register<string, PageTurn>("Empty", () => { return "暂无数据"; });

        public string Empty
        {
            get
            {
                return (string)GetValue(EmptyProperty);
            }
            set
            {
                SetValue(EmptyProperty, value);
            }
        }


        protected override void OnGotProxyCode(ref object baseValue)
        {
            using (var temp = StringPool.Borrow())
            {
                var sb = temp.Item;
                sb.Append("{");
                sb.Append("give:new $$metronic.pageTurn(");
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
            var sender = view.GetSender<PageTurnSE>();
            var data = this.LoadData(view, sender);
            view.Output(data);
            return view;
        }

        public Func<ScriptView, PageTurnSE, DTObject> LoadData = null;

    }
}
