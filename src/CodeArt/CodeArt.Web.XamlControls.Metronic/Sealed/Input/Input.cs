using CodeArt.Web.WebPages.Xaml.Sealed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.DTO;
using HtmlAgilityPack;

namespace CodeArt.Web.XamlControls.Metronic.Sealed
{
    public class Input : SealedControl
    {
        protected override CodeAsset[] GetCodeAssets(HtmlNode node)
        {
            string type = this.GetType(node);
            List<CodeAsset> assets = new List<CodeAsset>();
            assets.Add(new LinkCode() { ExternalKey = "metronic:wrapper.input.js", Origin = DrawOrigin.Bottom });

            switch (type)
            {
                case "checkbox":
                case "radio":
                    assets.Add(new LinkCode() { ExternalKey = "metronic:icheck.js", Origin = DrawOrigin.Bottom });
                    assets.Add(new LinkCode() { ExternalKey = "metronic:icheck.css", Origin = DrawOrigin.Header });
                    break;
                case "date":
                    assets.Add(new LinkCode() { ExternalKey = "metronic:bootstrap-datepicker.js", Origin = DrawOrigin.Bottom });
                    assets.Add(new LinkCode() { ExternalKey = "metronic:bootstrap-timepicker.min.js", Origin = DrawOrigin.Bottom });
                    assets.Add(new LinkCode() { ExternalKey = "metronic:clockface.js", Origin = DrawOrigin.Bottom });
                    assets.Add(new LinkCode() { ExternalKey = "metronic:bootstrap-daterangepicker.moment.min.js", Origin = DrawOrigin.Bottom });
                    assets.Add(new LinkCode() { ExternalKey = "metronic:bootstrap-daterangepicker.daterangepicker.js", Origin = DrawOrigin.Bottom });
                    assets.Add(new LinkCode() { ExternalKey = "metronic:bootstrap-colorpicker.js", Origin = DrawOrigin.Bottom });
                    assets.Add(new LinkCode() { ExternalKey = "metronic:bootstrap-datetimepicker.min.js", Origin = DrawOrigin.Bottom });
                    assets.Add(new LinkCode() { ExternalKey = "metronic:bootstrap-datepicker.zh-CN.js", Origin = DrawOrigin.Bottom });
                    assets.Add(new LinkCode() { ExternalKey = "metronic:bootstrap-datetimepicker.zh-CN.js", Origin = DrawOrigin.Bottom });
                    assets.Add(new LinkCode() { ExternalKey = "metronic:wrapper.input-date.js", Origin = DrawOrigin.Bottom });
                    assets.Add(new LinkCode() { ExternalKey = "metronic:clockface.css", Origin = DrawOrigin.Header });
                    assets.Add(new LinkCode() { ExternalKey = "metronic:bootstrap-datepicker3.min.css", Origin = DrawOrigin.Header });
                    assets.Add(new LinkCode() { ExternalKey = "metronic:bootstrap-timepicker.min.css", Origin = DrawOrigin.Header });
                    assets.Add(new LinkCode() { ExternalKey = "metronic:bootstrap-colorpicker.css", Origin = DrawOrigin.Header });
                    assets.Add(new LinkCode() { ExternalKey = "metronic:bootstrap-daterangepicker-bs3.css", Origin = DrawOrigin.Header });
                    assets.Add(new LinkCode() { ExternalKey = "metronic:bootstrap-datetimepicker.min.css", Origin = DrawOrigin.Header });
                    break;
                case "list":
                    assets.Add(new LinkCode() { ExternalKey = "metronic:wrapper.input-list.js", Origin = DrawOrigin.Bottom });
                    break;
                case "tags":
                case "dropdown":
                    assets.Add(new LinkCode() { ExternalKey = "metronic:select2.css", Origin = DrawOrigin.Header });
                    assets.Add(new LinkCode() { ExternalKey = "metronic:select2.js", Origin = DrawOrigin.Bottom });
                    break;
            }

            return assets.ToArray();
        }

        protected override void FillCode(HtmlNode node, PageBrush brush)
        {
            string type = this.GetType(node);

            switch (type)
            {
                case "text":
                    InputTextPainter.Instance.FillHtml(this, node, brush);
                    return;
                case "number":
                    InputNumberPainter.Instance.FillHtml(this, node, brush);
                    return;
                case "textarea":
                    InputTextareaPainter.Instance.FillHtml(this, node, brush);
                    return;
                case "checkbox":
                    InputCheckboxPainter.Instance.FillHtml(this, node, brush);
                    return;
                case "date":
                    InputDatePainter.Instance.FillHtml(this, node, brush);
                    return;
                case "email":
                    InputEmailPainter.Instance.FillHtml(this, node, brush);
                    return;
                case "hidden":
                    InputHiddenPainter.Instance.FillHtml(this, node, brush);
                    return;
                case "list":
                    InputListPainter.Instance.FillHtml(this, node, brush);
                    return;
                case "password":
                    InputPasswordPainter.Instance.FillHtml(this, node, brush);
                    return;
                case "radio":
                    InputRadioPainter.Instance.FillHtml(this, node, brush);
                    return;
                case "tags":
                    InputTagsPainter.Instance.FillHtml(this, node, brush);
                    return;
                case "texts":
                    InputTextsPainter.Instance.FillHtml(this, node, brush);
                    return;
                case "url":
                    InputUrlPainter.Instance.FillHtml(this, node, brush);
                    return;
                case "dropdown":
                    InputDropdownPainter.Instance.FillHtml(this, node, brush);
                    return;

            }

            throw new ApplicationException("没有找到类型为" + type + "的input解析器");
        }

        protected string GetType(HtmlNode node)
        {
            var attr = node.Attributes["type"];
            if (attr == null) throw new WebException(node.OriginalName + "没有指定type");
            return attr == null ? null : attr.Value;
        }
    }
}
