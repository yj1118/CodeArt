using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.Concurrent;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class FormSectionRow : ContentControl
    {
        protected override void Draw(PageBrush brush)
        {
            if(string.IsNullOrEmpty(this.Class))
                brush.DrawLine("<div class=\"form-group m-form__group row m--padding-top-10 m--padding-bottom-10\">");
            else
            {
                brush.DrawFormat("<div class=\"form-group m-form__group row m--padding-top-10 m--padding-bottom-10 {0}\">",this.Class);
                brush.DrawLine();
            }
            //根据内容数量，自动分列

            using (var temp = ListPool<UIElement>.Borrow())
            {
                var childs = temp.Item;

                FillEffectiveChilds(childs);

                if (childs.Count > 0)
                {
                    var col = 12 / childs.Count;
                    foreach (UIElement child in childs)
                    {
                        brush.DrawFormat("<div class=\"col-lg-{0}\">", col);
                        brush.DrawLine();
                        child.Render(brush);
                        brush.DrawLine("</div>");
                    }
                }
            }

                
            brush.DrawLine("</div>");
        }

        private void FillEffectiveChilds(List<UIElement> childs)
        {
            var content = this.Content;
            foreach (UIElement child in content)
            {
                if (child.Visibility == Visibility.Visible)
                {
                    childs.Add(child);
                }
            }
        }

    }
}
