using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.Data;
using CodeArt.ModuleNest;
using CodeArt.Concurrent;
using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.Util;

namespace CodeArt.Web.XamlControls.Metronic
{
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.DataTable.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class DataTable : Control
    {
        /// <summary>
        /// 表头信息
        /// </summary>
        public readonly static DependencyProperty HeaderProperty = DependencyProperty.Register<UIElementCollection, DataTable>("Header", () => { return new UIElementCollection(); });

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

        /// <summary>
        /// 列定义
        /// </summary>
        public readonly static DependencyProperty ColumnsProperty = DependencyProperty.Register<UIElementCollection, DataTable>("Columns", () => { return new UIElementCollection(); });

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
                sb.Append("give:new $$metronic.datatable(");
                sb.Append(GetConfigCode());
                sb.Append("),'datatable':true");
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
                    var column = this.Columns[i] as DataTableColumn;
                    FillColumnCode(column, i, sb);
                    sb.Append(",");
                }
                if (this.Columns.Count > 0) sb.Length--;
                sb.Append("]");
                sb.Append("}");
                return sb.ToString();
            }
        }

        private void FillColumnCode(DataTableColumn column, int columnIndex, StringBuilder code)
        {
            if(column.Selector)
            {
                code.Append("{");
                code.AppendFormat("field: '{0}',title: '#',width: 50,sortable: false,textAlign: 'center',selector:", column.Field);
                code.Append("{class: 'm-checkbox--solid m-checkbox--brand'}}");
            }
            else
            {
                code.Append("{");
                code.AppendFormat("field:'{0}',", column.Field);
                code.AppendFormat("title:'{0}',", column.Title);
                if(!string.IsNullOrEmpty(column.Width)) code.AppendFormat("width:{0},", column.Width);
                code.AppendFormat("sortable:{0},",JSON.GetCode(column.Sortable));
                code.AppendFormat("textAlign:'{0}',", column.TextAlign);
                if (!string.IsNullOrEmpty(column.Responsive)) code.AppendFormat("responsive:{0},", column.Responsive);
                if (!string.IsNullOrEmpty(column.Type)) code.AppendFormat("type:'{0}',", column.Type);
                if (!string.IsNullOrEmpty(column.GetTemplate))
                    code.AppendFormat("template:{0},", column.GetTemplate);
                else if (column.Content.Count > 0) code.AppendFormat("template:{0},", GetTemplateFunction(columnIndex));

                if(column.TextField) code.Append("textField:true,");

                code.Length--;
                code.Append("}");
            }
        }

        private string GetTemplateFunction(int columnIndex)
        {
            return string.Format("getTemplate_{0}_{1}", this.Id, columnIndex);
        }

        protected override void Draw(PageBrush brush)
        {
            base.Draw(brush);
            for (var i = 0; i < this.Columns.Count; i++)
            {
                var column = this.Columns[i] as DataTableColumn;
                if(column.Content.Count > 0)
                {
                    DrawColumnTemplate(column.Content, i, brush);
                }
            }
        }

        private void DrawColumnTemplate(UIElementCollection content, int columnIndex, PageBrush brush)
        {
            string code = string.Empty;
            using (var temp = StringPool.Borrow())
            {
                var sb = temp.Item;
                sb.AppendLine("<script>");
                sb.AppendFormat("function {0}()", GetTemplateFunction(columnIndex));
                sb.Append("{");
                sb.AppendLine();
                sb.AppendFormat("return {0};",JSON.GetCode(GetColumnTemplateCode(content)));
                sb.AppendLine();
                sb.AppendLine("}");
                sb.Append("</script>");
                code = sb.ToString();
            }
            brush.DrawLine(code, DrawOrigin.Bottom);
        }

        private string GetColumnTemplateCode(UIElementCollection content)
        {
            string code = string.Empty;
            using (var temp = StringPool.Borrow())
            {
                var sb = temp.Item;
                foreach (UIElement item in content)
                {
                    var ec = XamlUtil.GetCode(item);
                    sb.AppendLine(ec);
                }
                code = sb.ToString();
            }
            return code;
        }


        public override void OnInit()
        {
            base.OnInit();
            this.RegisterScriptAction("Load", this.Load);
        }

        private IScriptView Load(ScriptView view)
        {
            if (this.LoadData == null) throw new XamlException(Strings.NoLoadDataMethod);
            var sender = view.GetSender<DataTableSE>();
            var data = this.LoadData(view, sender);

            //datatable组件不识别日期类型，必须得转换成字符串
            ProcessDate(sender, data);

            //将数据转为客户端组件可以识别的格式
            data.Transform("meta.page=@pageIndex",(value)=>
            {
                return DataUtil.ToValue<int>(value);
            });
            data.Transform("meta.pages=@pageCount");
            data.Transform("meta.perpage=@pageSize");
            data.Transform("meta.total=@dataCount");
            data.Transform("rows=>data");
            data.Transform("~meta,data");

            return new DataView(data);
        }

        private void ProcessDate(DataTableSE sender, DTObject data)
        {
            var dateColumns = sender.DateColumns;
            var datetimeColumns = sender.DatetimesColumns;
            if (dateColumns.Length > 0 || datetimeColumns.Length > 0)
            {
                data.Each("rows", (row) =>
                {
                    foreach (var column in dateColumns)
                    {
                        if (row.TryGetValue<DateTime>(column, out var value))
                        {
                            row[column] = value.ToString("yyyy/MM/dd");
                        }
                    }

                    foreach (var column in datetimeColumns)
                    {
                        if(row.TryGetValue<DateTime>(column,out var value))
                        {
                            row[column] = value.ToString("yyyy/MM/dd hh:mm");
                        }
                    }
                });
            }

        }


        public override DependencyObject GetChild(string childName)
        {
            return base.GetChild(childName) ?? this.Header.GetChild(childName) ?? this.Columns.GetChild(childName);
        }

        public Func<ScriptView, DataTableSE, DTObject> LoadData = null;
    }
}
