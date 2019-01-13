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
    public class AsideMenuCode : CodeAsset
    {
        static AsideMenuCode()
        {
            
        }

        public AsideMenuCode()
        {
            this.Origin = DrawOrigin.Current;
        }

        protected override string GetCode()
        {
            var page = this.GetTemplateParent() as Page;

            var path = WebPageContext.Current.VirtualPath;

            var menu = page.AsideMenu;
            using (var temp = StringPool.Borrow())
            {
                var code = temp.Item;

                code.AppendLine("<li class=\"m--margin-top-20\"></li>");

                foreach(var obj in menu)
                {
                    var item = obj as MenuItem;
                    if(item != null)
                    {
                        if (item.Url.EqualsIgnoreCase(path))
                        {
                            code.AppendFormat("<li id=\"{0}\" class=\"m-menu__item m-menu__item--active\">",item.Id);
                            code.AppendLine();
                        }
                        else
                        {
                            code.AppendFormat("<li id=\"{0}\" class=\"m-menu__item\">",item.Id);
                            code.AppendLine();
                        }
                        code.AppendFormat("<a href=\"{0}\" class=\"m-menu__link \"><i class=\"m-menu__link-bullet m-menu__link-bullet--dot\"><span></span></i><span class=\"m-menu__link-text\">{1}</span></a>", item.Url, item.Text);
                        code.AppendLine();
                        code.AppendLine("</li>");
                    }
                    else
                    {
                        var section = obj as MenuSection;
                        code.AppendLine("<li class=\"m-menu__section m-menu__section\">");
                        code.AppendFormat("<h4 class=\"m-menu__section-text\">{0}</h4>", section.Text);
                        code.AppendLine();
                        code.AppendLine("</li>");
                    }
                }
                return code.ToString();
            }            
        }

        protected override void Draw(PageBrush brush)
        {
            base.Draw(brush);
        }
    }
}