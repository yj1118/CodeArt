using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using CodeArt.DTO;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.XamlControls.Metronic
{
    /// <summary>
    /// 动态输入组件
    /// </summary>
    public class DynamicInputSE : ScriptElement
    {
        public DynamicInputSE() { }


        #region 发射命令

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            this.View.WriteCode(string.Format("{0}.proxy().clear();", this.Id));
        }

        public void Accept(DTObject config)
        {
            var html = GetInputsHtml(config);
            this.View.WriteCode(string.Format("{0}.proxy().accept({1});", this.Id, JSON.GetCode(html)));
        }

        private string GetInputsHtml(DTObject config)
        {
            var items = config.GetList("items");
            StringBuilder html = new StringBuilder();
            html.AppendLine("<ContentControl xmlns=\"http://schemas.codeart.cn/web/xaml\" xmlns:m=\"http://schemas.codeart.cn/web/xaml/metronic\" xmlns:ms=\"http://schemas.codeart.cn/web/xaml/metronic/sealed\">");
            foreach (var item in items)
            {
                html.Append("<Column xs=\"12\" md=\"12\" class=\"col-trim dynamicItem\">");
                html.Append(GenerateHtml(item));
                html.AppendLine("</Column>");
            }
            html.Append("</ContentControl>");

            ContentControl content = XamlReader.ReadComponent(html.ToString()) as ContentControl;
            var brush = new PageBrush();
            content.Render(brush);
            var code = brush.GetCode();
            return code;
        }



        private string GenerateHtml(DTObject item)
        {
            var type = item.GetValue<string>("type");
            var label = item.GetValue<string>("name");
            var message = item.GetValue<string>("message", string.Empty);
            var required = item.GetValue<bool>("required", false);

            switch (type)
            {
                case "text":
                    {
                        StringBuilder code = new StringBuilder();
                        code.AppendFormat("<ms:input type=\"text\" max=\"200\" required=\"{0}\" name=\"{1}\" >", (required ? "true" : "false"), label);
                        code.AppendLine();
                        code.AppendFormat("<label xs=\"12\">{0}</label>", label);
                        code.AppendLine();
                        code.AppendLine("<core xs=\"12\"></core>");
                        code.AppendFormat("<help xs=\"12\">{0}</help>", message);
                        code.AppendLine();
                        code.AppendLine("</ms:input>");
                        return code.ToString();
                    }
                case "textarea":
                    {
                        StringBuilder code = new StringBuilder();
                        code.AppendFormat("<ms:input type=\"textarea\" max=\"2000\" required=\"{0}\" name=\"{1}\" rows=\"4\" >", (required ? "true" : "false"), label);
                        code.AppendLine();
                        code.AppendFormat("<label xs=\"12\">{0}</label>", label);
                        code.AppendLine();
                        code.AppendLine("<core xs=\"12\"></core>");
                        code.AppendFormat("<help xs=\"12\">{0}</help>", message);
                        code.AppendLine();
                        code.AppendLine("</ms:input>");
                        return code.ToString();
                    }
                case "radio":
                    return GetRadioSelectHtml(item, type, label, message, required);
                case "dropdown":
                    return GetDropdownSelectHtml(item, type, label, message, required);
                case "checkbox":
                    return GetCheckboxSelectHtml(item, type, label, message, required);
                case "day":
                case "minute":
                case "month":
                    return GetDateTimeHtml(item, type, label, message, required, type);
            }
            return string.Empty;
        }

        private string GetRadioSelectHtml(DTObject item, string type, string label, string message, bool required)
        {
            StringBuilder code = new StringBuilder();
            code.AppendFormat("<ms:input type=\"radio\" required=\"{0}\" inline=\"true\" skin=\"square-blue\" name=\"{1}\">", (required ? "true" : "false"), label);
            code.AppendLine();
            code.AppendFormat("<label xs=\"12\">{0}</label>", label);
            code.AppendLine();
            code.AppendLine("<core xs=\"12\">");
            code.Append(GetOptionsHtml(item));
            code.AppendLine("</core>");
            code.AppendFormat("<help xs=\"12\">{0}</help>", message);
            code.AppendLine();
            code.AppendLine("</ms:input>");
            return code.ToString();
        }

        private string GetDropdownSelectHtml(DTObject item, string type, string label, string message, bool required)
        {
            StringBuilder code = new StringBuilder();
            code.AppendFormat("<ms:dropdown required=\"{0}\" name=\"{1}\">", (required ? "true" : "false"), label);
            code.AppendLine();
            code.AppendFormat("<label xs=\"12\">{0}</label>", label);
            code.AppendLine();
            code.AppendLine("<core xs=\"12\">");
            code.Append(GetOptionsHtml(item));
            code.AppendLine("</core>");
            code.AppendFormat("<help xs=\"12\">{0}</help>", message);
            code.AppendLine();
            code.AppendLine("</ms:dropdown>");
            return code.ToString();
        }

        private string GetCheckboxSelectHtml(DTObject item, string type, string label, string message, bool required)
        {
            StringBuilder code = new StringBuilder();
            code.AppendFormat("<ms:input type=\"checkbox\" required=\"{0}\" inline=\"true\" skin=\"square-blue\" name=\"{1}\" stringMode=\"true\">", (required ? "true" : "false"), label);
            code.AppendLine();
            code.AppendFormat("<label xs=\"12\">{0}</label>", label);
            code.AppendLine();
            code.AppendLine("<core xs=\"12\">");
            code.Append(GetOptionsHtml(item));
            code.AppendLine("</core>");
            code.AppendFormat("<help xs=\"12\">{0}</help>", message);
            code.AppendLine();
            code.AppendLine("</ms:input>");
            return code.ToString();
        }

        private string GetOptionsHtml(DTObject item)
        {
            var items = item.GetList("options");
            StringBuilder html = new StringBuilder();
            html.AppendLine("<items>");
            foreach (var option in items)
            {
                html.AppendFormat("<item value=\"{0}\">{0}</item>", option.GetValue<string>());
                html.AppendLine();
            }
            html.AppendLine("</items>");
            return html.ToString();
        }

        private string GetDateTimeHtml(DTObject item, string type, string label, string message, bool required, string mode)
        {
            StringBuilder code = new StringBuilder();
            code.AppendFormat("<ms:input type=\"date\" required=\"{0}\" name=\"{1}\" mode=\"{2}\" >", (required ? "true" : "false"), label, mode);
            code.AppendLine();
            code.AppendFormat("<label xs=\"12\">{0}</label>", label);
            code.AppendLine();
            code.AppendLine("<core xs=\"12\"></core>");
            code.AppendFormat("<help xs=\"12\">{0}</help>", message);
            code.AppendLine();
            code.AppendLine("</ms:input>");
            return code.ToString();
        }



        #endregion


    }
}
