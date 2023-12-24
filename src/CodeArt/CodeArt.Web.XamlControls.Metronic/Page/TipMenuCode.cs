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
    public class TipMenuCode : CodeAsset
    {
        public string Position
        {
            get;
            set;
        }

        static TipMenuCode()
        {
            
        }

        public TipMenuCode()
        {
            this.Origin = DrawOrigin.Current;
        }

        protected override string GetCode()
        {
            var page = this.GetTemplateParent() as Page;

            var path = WebPageContext.Current.VirtualPath;

            var menu = this.Position == "Before" ? page.BeforeLoginMenu : page.AfterLoginMenu;
            using (var temp = StringPool.Borrow())
            {
                var code = temp.Item;

                foreach(var obj in menu)
                {
                    var item = obj as MenuItem;
                    if(item != null)
                    {
                        code.AppendFormat("<li id=\"{0}\" class=\"m-nav__item\">", item.Id);
                        code.AppendLine();
                        code.AppendFormat("<a href=\"{0}\" class=\"m-nav__link\">", item.Url);
                        code.AppendLine();
                        code.AppendFormat("<i class=\"m-nav__link-icon {0}\"></i>",item.Icon);
                        code.AppendLine();
                        code.AppendLine("<span class=\"m-nav__link-title\">");
                        code.AppendLine("<span class=\"m-nav__link-wrap\">");
                        code.AppendFormat("<span class=\"m-nav__link-text\">{0}</span>", item.Text);
                        code.AppendLine();
                        code.AppendLine("</span>");
                        code.AppendLine("</span>");
                        code.AppendLine("</a>");
                        code.AppendLine("</li>");
                    }
                    else
                    {
                        var section = obj as MenuSection;
                        if(section != null)
                        {
                            code.AppendLine("<li class=\"m-nav__separator m-nav__separator--fit\"></li>");
                        }
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