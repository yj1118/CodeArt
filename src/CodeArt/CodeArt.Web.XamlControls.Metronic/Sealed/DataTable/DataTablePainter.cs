using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

using HtmlAgilityPack;
using CodeArt.HtmlWrapper;


using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Sealed;

namespace CodeArt.Web.XamlControls.Metronic.Sealed
{
    internal abstract class DataTablePainter
    {
        protected DataTablePainter() { }

        public virtual void FillHtml(object obj, HtmlNode node, PageBrush brush)
        {
            var className = GetContainerClassName(node);
            className = UIUtil.GetClassName(node, className);
            SealedPainter.CreateNodeCode(brush, "div", className, SealedPainter.GetStyleCode(node), GetProxyCode(obj, node), (pageBrush) =>
              {
                  pageBrush.Draw(CreateWrapper(node));
                  pageBrush.Draw(string.Format("<table data-name=\"dataTable\" class=\"{0}\">", UIUtil.GetClassName(node, GetTableClassName(node))));
                  pageBrush.DrawLine();
                  pageBrush.DrawLine(CreateThead(node));
                  DrawTbody(obj as SealedControl, node, pageBrush);
                  pageBrush.Draw("</table>");
              }, () =>
              {
                  return UIUtil.GetProperiesCode(node, "id", "data-name");
              });
        }

        protected virtual string CreateWrapper(HtmlNode node)
        {
            var wrapper = node.SelectSingleNodeEx("wrapper");
            if (wrapper == null) return string.Empty;
            StringBuilder html = new StringBuilder();
            html.AppendLine("<div class=\"table-actions-wrapper\">");
            html.Append(wrapper.InnerHtml);
            html.AppendLine("</div>");
            return html.ToString();
        }


        protected virtual string CreateThead(HtmlNode node)
        {
            StringBuilder html = new StringBuilder();
            html.AppendLine("<thead>");
            html.Append(CreateTheadHeading(node));
            html.Append(CreateTheadFilter(node));
            html.Append("</thead>");
            return html.ToString();
        }

        private string CreateTheadHeading(HtmlNode node)
        {
            var items = node.SelectNodesEx("headers/item");
            if (items.Count == 0) return string.Empty;
            StringBuilder html = new StringBuilder();
            html.AppendFormat("<tr role=\"row\" class=\"heading{0}\">", HaveFilters(node) ? string.Empty : " noFilters");
            html.AppendLine();
            var selectMode = node.GetAttributeValue("select", "false");
            if (selectMode == "true")
            {
                html.AppendLine("<th class=\"table-checkbox\"><input type=\"checkbox\" class=\"group-checkable\"/></th>");
            }

            foreach (var item in items)
            {
                html.AppendFormat("<th{0}{1}>{2}</th>", SealedPainter.GetFullClassName(item), SealedPainter.GetStyleCode(item), item.GetAttributeValue("text", string.Empty));
                html.AppendLine();
            }
            html.AppendLine("</tr>");
            return html.ToString();
        }

        private bool HaveFilters(HtmlNode node)
        {
            return node.SelectNodesEx("filters/item").Count > 0;
        }

        private string CreateTheadFilter(HtmlNode node)
        {
            var items = node.SelectNodesEx("filters/item");
            if (items.Count == 0) return string.Empty;
            StringBuilder html = new StringBuilder();
            html.AppendLine("<tr role=\"row\" class=\"filter\">");
            var selectMode = node.GetAttributeValue("select", "false");
            if (selectMode == "true") html.AppendLine("<td></td>");
            foreach (var item in items)
            {
                string colspan = item.GetAttributeValue("colspan", string.Empty);
                if (colspan.Length > 0) colspan = string.Format(" colspan=\"{0}\"", colspan);

                html.AppendFormat("<td{0}{1}{2}>{3}</td>", SealedPainter.GetFullClassName(item), SealedPainter.GetStyleCode(item), colspan, item.InnerHtml);
                html.AppendLine();
            }
            html.AppendLine("</tr>");
            return html.ToString();
        }

        protected virtual void DrawTbody(SealedControl obj, HtmlNode node, PageBrush brush)
        {
            brush.DrawLine("<tbody>");
            brush.Draw("</tbody>");
        }

        private string GetContainerClassName(HtmlNode node)
        {
            string skin = node.GetAttributeValue("skin", string.Empty);
            if (string.IsNullOrEmpty(skin)) return "table-container";
            return string.Format("table-container table-container-{0}", skin);
        }

        private string GetTableClassName(HtmlNode node)
        {
            string skin = node.GetAttributeValue("skin", string.Empty);
            switch (skin)
            {
                case "close": return "table table-striped table-bordered table-hover"; //封闭
                case "open": return "table table-striped table-hover";  //开放的
            }
            return "table";
        }

        private string GetProxyCode(object obj, HtmlNode node)
        {
            var uiParas = new StringBuilder(UIUtil.GetJSONMembers(node, "url:", "action:", "onload", "saveState", "maxHeight:", "page","xaml:true","name:","view:"));
            if (uiParas.Length > 0) uiParas.Append(",");
            uiParas.AppendFormat("paras:{0}", GetParasCode(node));
            if (uiParas.Length > 0) uiParas.Append(",");
            uiParas.AppendFormat("columns:{0}", GetColumnDefindsCode(node));

            return UIUtil.GetProxyCode(obj as UIElement, node, GetGiveCode(), uiParas.ToString(), UIUtil.GetJSONMembers(node));
        }

        protected abstract string GetGiveCode();


        internal static string GetParasCode(HtmlNode node)
        {
            var paras = node.SelectNodesEx("paras/add");
            StringBuilder code = new StringBuilder("[");
            foreach (HtmlNode para in paras)
            {
                code.Append("{");
                string id = para.GetAttributeValue("refId", string.Empty);
                if (!string.IsNullOrEmpty(id)) code.AppendFormat("id:'{0}'", id);
                else
                {
                    string provider = para.GetAttributeValue("provider", string.Empty);
                    if (!string.IsNullOrEmpty(provider)) code.AppendFormat("provider:{0}", provider);
                }
                code.Append("},");
            }
            if (paras.Count > 1) code.Length--;
            code.Append("]");
            return code.ToString();
        }

        /// <summary>
        /// 获取列定义的配置代码，包括是否参与排序、是否参与搜索
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal static string GetColumnDefindsCode(HtmlNode node)
        {
            var items = node.SelectNodesEx("headers/item");
            StringBuilder code = new StringBuilder("[");

            var selectMode = node.GetAttributeValue("select", "false");
            if (selectMode == "true") code.Append("{orderable:false,searchable:false},");

            foreach (HtmlNode item in items)
            {
                string orderable = item.GetAttributeValue("orderable", "false");
                string searchable = item.GetAttributeValue("searchable", "false");
                code.Append("{");
                code.AppendFormat("orderable:{0},searchable:{1}", orderable, searchable);
                string defaultOrder = item.GetAttributeValue("defaultOrder", string.Empty);
                if (defaultOrder.Length > 0) code.AppendFormat(",defaultOrder:'{0}'", defaultOrder);
                code.Append("},");
            }
            if (code.Length > 1) code.Length--;
            code.Append("]");
            return code.ToString();
        }


        public static LinkCode[] Links = new LinkCode[]
        {
                new LinkCode() { ExternalKey = "metronic:dataTables.bootstrap.css", Origin = DrawOrigin.Header },
                new LinkCode() { ExternalKey = "metronic:jquery.dataTables.js", Origin = DrawOrigin.Bottom },
                new LinkCode() { ExternalKey = "metronic:dataTables.bootstrap.js", Origin = DrawOrigin.Bottom },
                new LinkCode() { ExternalKey = "metronic:datatable.js", Origin = DrawOrigin.Bottom }
        };

    }
}
