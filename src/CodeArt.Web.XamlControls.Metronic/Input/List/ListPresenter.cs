using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class ListPresenter : FrameworkElement
    {
        protected override void Draw(PageBrush brush)
        {
            var list = (this.BelongTemplate.TemplateParent as List);
            brush.DrawLine("<div class=\"m-section\">");
            brush.DrawLine("<div class=\"m-section__content\">");
            brush.DrawLine("<table class=\"table m-table table-striped m-table--head-bg-brand\">");
            DrawHeader(brush, list);
            DrawBody(brush, list);
            brush.DrawLine("</table>");
            brush.DrawLine("</div>");
            brush.DrawLine("</div>");
        }

        private void DrawHeader(PageBrush brush, List list)
        {
            brush.DrawLine("<thead>");
            brush.DrawLine("<tr>");
            brush.DrawLine("<th style=\"width:50px; text-align:center;\">#</th>");
            var columns = list.Columns;
            foreach(ListColumn column in columns)
            {
                if(string.IsNullOrEmpty(column.Width))
                {
                    brush.DrawFormat("<th style=\"text-align:{0};\" nowrap=\"nowrap\">{1}</th>", column.TextAlign, column.Title);
                }
                else
                {
                    brush.DrawFormat("<th style=\"width:{0}; text-align:{1};\">{2}</th>", column.Width, column.TextAlign, column.Title);
                }
                brush.DrawLine();
            }
            brush.DrawFormat("<th style=\"width:220px; text-align:center;\">{0}</th>", Strings.Actions);
            brush.DrawLine();
            brush.DrawLine("</tr>");
            brush.DrawLine("</thead>");
        }

        private void DrawBody(PageBrush brush, List list)
        {
            brush.DrawLine("<tbody>");
            brush.DrawLine("<tr data-name=\"listItem\">");
            brush.DrawLine("<th style=\"width:50px; text-align:center;vertical-align: middle;\" scope=\"row\"></th>");
            var columns = list.Columns;
            foreach (ListColumn column in columns)
            {
                brush.DrawFormat("<td style=\"width:{0}; text-align:{1};\">",column.Width,column.TextAlign);
                brush.DrawLine();
                column.Content.Render(brush);
                brush.DrawLine("</td>");
            }
            DrawActions(brush);
            brush.DrawLine("</tr>");
            brush.DrawLine("</tbody>");
        }

        private void DrawActions(PageBrush brush)
        {
            brush.Draw("<td class=\"list-actions\" style=\"width:220px; text-align:center;\">");
            brush.DrawFormat("<a href=\"javascript:;\" class=\"btn m-btn m-btn--hover-info m-btn--icon m-btn--icon-only m-btn--pill\" title=\"{0}\" data-name=\"addItem\"><i class=\"la la-plus\"></i></a>", Strings.NewRow);
            brush.DrawFormat("<a href=\"javascript:;\" class=\"btn m-btn m-btn--hover-primary m-btn--icon m-btn--icon-only m-btn--pill\" title=\"{0}\" data-name=\"prevItem\"><i class=\"la la-arrow-up\"></i></a>", Strings.MoveUp);
            brush.DrawFormat("<a href=\"javascript:;\" class=\"btn m-btn m-btn--hover-primary m-btn--icon m-btn--icon-only m-btn--pill\" title=\"{0}\" data-name=\"nextItem\"><i class=\"la la-arrow-down\"></i></a>", Strings.MoveDown);
            brush.DrawFormat("<a href=\"javascript:;\" class=\"btn m-btn m-btn--hover-accent m-btn--icon m-btn--icon-only m-btn--pill\" title=\"{0}\" data-name=\"resetItem\"><i class=\"la la-refresh\"></i></a>", Strings.Reset);
            brush.DrawFormat("<a href=\"javascript:;\" class=\"btn m-btn m-btn--hover-danger m-btn--icon m-btn--icon-only m-btn--pill\" title=\"{0}\" data-name=\"removeItem\"><i class=\"la la-close\"></i></a>", Strings.Remove);
            brush.Draw("</td>");
            brush.DrawLine();
        }

    }
}
