using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.Concurrent;
using CodeArt.Util;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class BreadcrumbCode : CodeAsset
    {
        static BreadcrumbCode()
        {
            
        }

        public BreadcrumbCode()
        {
            this.Origin = DrawOrigin.Current;
        }

        protected override string GetCode()
        {
            var page = this.GetTemplateParent() as Page;

            var item = GetCurrent(page);
            if (item == null) return string.Empty;
            using (var temp = StringPool.Borrow())
            {
                var code = temp.Item;
                code.AppendFormat("<h3 class=\"m-subheader__title m-subheader__title--separator\">{0}</h3>", item.Text);
                code.AppendLine();
                code.AppendLine("<ul class=\"m-subheader__breadcrumbs m-nav m-nav--inline\">");

                code.AppendLine("<li class=\"m-nav__item m-nav__item--home\">");
                code.AppendLine("<a href=\"/\" class=\"m-nav__link m-nav__link--icon\">");
                code.AppendLine("<i class=\"m-nav__link-icon la la-home\"></i>");
                code.AppendLine("</a>");
                code.AppendLine("</li>");
                code.AppendLine("<li class=\"m-nav__separator\">-</li>");

                var headerItem = HeaderMenuCode.GetCurrent(page);
                if (headerItem != null)
                {
                    code.AppendLine("<li class=\"m-nav__item\">");
                    code.AppendFormat("<a href=\"{0}\" class=\"m-nav__link\">", headerItem.Url);
                    code.AppendLine();
                    code.AppendFormat("<span class=\"m-nav__link-text\">{0}</span>", headerItem.Text);
                    code.AppendLine();
                    code.AppendLine("</a>");
                    code.AppendLine("</li>");
                    code.AppendLine("<li class=\"m-nav__separator\">-</li>");
                }

                code.AppendLine("<li class=\"m-nav__item\">");
                code.AppendFormat("<a href=\"{0}\" class=\"m-nav__link\">", item.Url);
                code.AppendLine();
                code.AppendFormat("<span class=\"m-nav__link-text\">{0}</span>", item.Text);
                code.AppendLine();
                code.AppendLine("</a>");
                code.AppendLine("</li>");
                code.Append("</ul>");
                return code.ToString();
            }
        }

        private static MenuItem GetCurrent(Page page)
        {
            var path = WebPageContext.Current.VirtualPath;
            var menu = page.AsideMenu;
            foreach (var obj in menu)
            {
                var item = obj as MenuItem;
                if (item != null && item.Url.EqualsIgnoreCase(path))
                {
                    return item;
                }
            }
            return null;
        }


        protected override void Draw(PageBrush brush)
        {
            base.Draw(brush);
        }
    }
}