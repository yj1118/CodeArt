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
    public class TagsPresenter : FrameworkElement
    {
        protected override void Draw(PageBrush brush)
        {
            var tags = (this.BelongTemplate.TemplateParent as Tags);
            brush.DrawLine("<div class=\"row m--padding-left-10 m--padding-right-10\" \">");
            DrawBody(brush, tags);
            brush.DrawLine("</div>");
        }

        private void DrawBody(PageBrush brush, Tags tags)
        {
            var width = 12 / tags.Column;
            var xaml = string.Format("<!DOCTYPE xaml><m:TextBox xmlns:m=\"http://schemas.codeart.cn/web/xaml/metronic\" layout=\"Cell\" minlength=\"{0}\" maxlength=\"{1}\" field=\"value\" label=\"{2}\" />", tags.ItemMinLength, tags.ItemMaxLength, 
                                                                                            string.IsNullOrEmpty(tags.ItemLabel) ? tags.Label : tags.ItemLabel) ;
            brush.DrawFormat("<div class=\"col-sm-{0} m--padding-5\" data-tags-item=\"true\">", width);
            brush.DrawLine();
            brush.DrawLine(_getTextBoxCode(xaml));
            brush.DrawLine("</div>");
        }

        private static Func<string, string> _getTextBoxCode = LazyIndexer.Init<string, string>((xaml) =>
        {
            return XamlUtil.GetCode(xaml, true);
        });


        private bool ShowCommand(Tags tags)
        {
            return tags.AllowAdd || tags.AllowRemove;
        }

    }
}
