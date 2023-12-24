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
    public class HeaderMenuCode : CodeAsset
    {
        static HeaderMenuCode()
        {
            
        }

        public HeaderMenuCode()
        {
            this.Origin = DrawOrigin.Current;
        }

        protected override string GetCode()
        {
            var page = this.GetTemplateParent() as Page;
            var menu = page.HeaderMenu;

            var path = WebPageContext.Current.VirtualPath;
            var fragment = WebPageContext.Current.Fragments.FirstOrDefault();
            if (fragment != null) fragment = string.Format("/{0}/", fragment);

            using (var temp = StringPool.Borrow())
            {
                var code = temp.Item;
                foreach (MenuItem item in menu)
                {
                    if ((fragment != null && item.Url.StartsWith(fragment)) || item.Url.EqualsIgnoreCase(path))
                        code.AppendLine("<li class=\"m-menu__item m-menu__item--active\">");
                    else
                        code.AppendLine("<li class=\"m-menu__item\">");
                    code.AppendFormat("<a href=\"{0}\" class=\"m-menu__link \"><span class=\"m-menu__item-here\"></span><span class=\"m-menu__link-text\">{1}</span></a>", item.Url, item.Text);
                    code.AppendLine();
                    code.AppendLine("</li>");
                }
                return code.ToString();
            }
        }


        public static MenuItem GetCurrent(Page page)
        {
            var menu = page.HeaderMenu;

            var path = WebPageContext.Current.VirtualPath;
            var fragment = WebPageContext.Current.Fragments.FirstOrDefault();
            if (fragment != null) fragment = string.Format("/{0}/", fragment);

            foreach (MenuItem item in menu)
            {
                if ((fragment != null && item.Url.StartsWith(fragment)) || item.Url.EqualsIgnoreCase(path))
                    return item;
            }
            return null;
        }


        protected override void Draw(PageBrush brush)
        {
            base.Draw(brush);
        }
    }
}