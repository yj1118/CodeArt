using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.ModuleNest;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class DataTableQuery : ContentControl
    {
        protected override void Draw(PageBrush brush)
        {
            if (this.Content.Count == 0) return;
            brush.DrawLine("<div class=\"m-form m-form--label-align-right m--margin-top-20 m--margin-bottom-30 c-datatableQuery\">");
            brush.DrawLine("<div class=\"row align-items-center\">");
            brush.DrawLine("<div class=\"col-sm-12\">");
            brush.DrawLine("<div class=\"form-group m-form__group row align-items-center\">");
            DrawMembers(brush);
            DrawButton(brush);
            brush.DrawLine("</div>");
            brush.DrawLine("</div>");
            brush.DrawLine("</div>");
            brush.Draw("</div>");
        }

        private void DrawMembers(PageBrush brush)
        {
            var cls = string.Empty;//1个成员
            switch(this.Content.Count)
            {
                case 1: cls = "col-xl-5 col-md-12"; break;
                case 2: cls = "col-xl-4 col-lg-6 col-md-12"; break;
                case 3: cls = "col-xl-3 col-lg-6 col-md-12"; break;
                default:
                    throw new XamlException("DataTableQuery 暂不支持3个以上的查询条件");
            }
            foreach (UIElement item in this.Content)
            {
                brush.DrawFormat("<div class=\"{0}\">", cls);
                brush.DrawLine();
                item.Render(brush);
                brush.DrawLine("<div class=\"d-xl-none m--margin-bottom-10\"></div>");
                brush.DrawLine("</div>");
            }
        }

        private void DrawButton(PageBrush brush)
        {
            if (this.Content.Count == 1) return;//1个成员的时候不显示搜索按钮
            var cls = string.Empty;
            if (this.Content.Count == 2) cls = "col-xl-4 col-md-12 m--align-right";
            else cls = "col-xl-3 col-lg-6 col-md-12 m--align-right";

            brush.DrawFormat("<div class=\"{0}\">", cls);
            brush.DrawLine();
            var noneCls = this.Content.Count == 2 ? "d-xl-none" : "d-lg-none";
            brush.DrawFormat("<div class=\"m-separator m-separator--dashed {0}\"></div>", noneCls);
            brush.DrawLine();
            brush.DrawLine("<a href=\"javascript:;\" data-proxy=\"{invoke:{component:'',events:[{client:'click',server:'OnSearch',view:'',option:{}}]}}\" class=\"btn btn-primary m-btn m-btn--custom m-btn--icon m-btn--air\">");
            brush.DrawFormat("<span><i class=\"la la-search\"></i><span>{0}</span></span>",Strings.Search);
            brush.DrawLine();
            brush.DrawLine("</a>");
            brush.DrawLine("</div>");
        }
    }
}
