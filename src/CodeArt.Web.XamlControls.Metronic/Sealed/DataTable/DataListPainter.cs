using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using HtmlAgilityPack;
using CodeArt.HtmlWrapper;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml.Sealed;
using CodeArt.Web.WebPages.Xaml;

namespace CodeArt.Web.XamlControls.Metronic.Sealed
{
    internal class DataListPainter : DataTablePainter
    {
        protected override void DrawTbody(SealedControl obj, HtmlNode node, PageBrush brush)
        {
            brush.DrawLine("<tbody data-name=\"dataContent\" data-proxy=\"{give:new $$.databind()}\">");
            brush.DrawLine("<tr data-proxy=\"{display:'',loops:'rows'}\">");

            var selectMode = node.GetAttributeValue("select", "false");
            if (selectMode == "true") brush.DrawLine("<td><input type=\"checkbox\" class=\"checkboxes\" value=\"1\" /></td>");

            var items = node.SelectNodesEx("columns/item");
            foreach (var item in items)
            {
                if (item.InnerHtml.Length == 0) brush.DrawFormat("<td data-proxy=\"{0}\"{1}{2}></td>", GetSystemProxyCode(item), SealedPainter.GetFullClassName(item), SealedPainter.GetStyleCode(item));
                else
                {
                    brush.DrawFormat("<td{0}{1}>", SealedPainter.GetFullClassName(item), SealedPainter.GetStyleCode(item));
                    obj.Elements.Render(brush, item.InnerHtml);
                    //brush.DrawXaml(item.InnerHtml);
                    brush.Draw("</td>");
                }
                brush.DrawLine();
            }

            brush.DrawLine("</tr>");
            brush.Draw("</tbody>");
        }

        private static string GetSystemProxyCode(HtmlNode item)
        {
            string binds = item.GetAttributeValue("binds", string.Empty);
            if (string.IsNullOrEmpty(binds)) binds = GetSystemBindsCode(item);
            string onbind = item.GetAttributeValue("onbind", string.Empty);
            string format = item.GetAttributeValue("format", string.Empty);

            StringBuilder code = new StringBuilder("{");
            if (!string.IsNullOrEmpty(binds)) code.AppendFormat("binds:{0},", binds);
            if (!string.IsNullOrEmpty(onbind)) code.AppendFormat("onbind:{0},", onbind);
            if (!string.IsNullOrEmpty(format)) code.AppendFormat("format:'{0}',", format);
            if (code.Length > 1) code.Length--;
            code.Append("}");
            return code.ToString();
        }

        private static string GetSystemBindsCode(HtmlNode item)
        {
            string textField = item.GetAttributeValue("text", string.Empty);
            string valueField = item.GetAttributeValue("value", string.Empty);

            StringBuilder code = new StringBuilder();
            if (!string.IsNullOrEmpty(textField)) code.AppendFormat("text:'{0}',", textField);
            if (!string.IsNullOrEmpty(valueField)) code.AppendFormat("'@value':'{0}',", valueField);

            if (!string.IsNullOrEmpty(textField) && string.IsNullOrEmpty(valueField))
                code.AppendFormat("'@value':'{0}',", textField);

            if (code.Length == 0) return string.Empty;
            code.Length--;
            return "{" + code.ToString() + "}";
        }

        protected override string GetGiveCode()
        {
            return "new $$.wrapper.metronic.datalist()";
        }

        public static DataListPainter Instance = new DataListPainter();
    }
}
