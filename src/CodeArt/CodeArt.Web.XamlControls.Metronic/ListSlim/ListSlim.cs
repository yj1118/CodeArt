using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.XamlControls.Metronic;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.Concurrent;
using CodeArt.Web.WebPages.Xaml.Script;
using FormSE = CodeArt.Web.XamlControls.Metronic.FormSE;
using CodeArt.Util;
using CodeArt.Web.WebPages;

namespace CodeArt.Web.XamlControls.Metronic
{
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.ListSlim.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class ListSlim : Control
    {
        public readonly static DependencyProperty ColumnsProperty = DependencyProperty.Register<UIElementCollection, ListSlim>("Columns", () => { return new UIElementCollection(); });

        public UIElementCollection Columns
        {
            get
            {
                return GetValue(ColumnsProperty) as UIElementCollection;
            }
            set
            {
                SetValue(ColumnsProperty, value);
            }
        }

        protected override void OnGotProxyCode(ref object baseValue)
        {
            using (var temp = StringPool.Borrow())
            {
                var sb = temp.Item;
                sb.Append("{");
                sb.Append("give:[new $$metronic.listSlim(");
                sb.Append(GetConfigCode());
                sb.Append("),new $$.databind()],'listSlim':true");
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
                sb.Append("columns:[");
                for (var i = 0; i < this.Columns.Count; i++)
                {
                    var column = this.Columns[i] as ListSlimColumn;
                    FillColumnCode(column, i, sb);
                    sb.Append(",");
                }
                if (this.Columns.Count > 0) sb.Length--;
                sb.Append("]");
                sb.Append("}");
                return sb.ToString();
            }
        }

        private void FillColumnCode(ListSlimColumn column, int columnIndex, StringBuilder code)
        {
            code.Append("{");
            code.AppendFormat("field:'{0}',", column.Field);
            code.AppendFormat("title:'{0}',", column.Title);
            if (!string.IsNullOrEmpty(column.Width)) code.AppendFormat("width:'{0}',", column.Width);
            code.AppendFormat("textAlign:'{0}',", column.TextAlign);
            if (!string.IsNullOrEmpty(column.GetTemplate))
                code.AppendFormat("template:{0},", column.GetTemplate);

            code.Length--;
            code.Append("}");
        }


        public ListSlim()
        { }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
        static ListSlim()
        { }



        public override DependencyObject GetChild(string childName)
        {
            return base.GetChild(childName);
        }

        public override IEnumerable<UIElement> GetActionElement(string actionName)
        {
            return this.Combine(base.GetActionElement(actionName));
        }
    }
}