using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class ModalBodyPresenter : FrameworkElement
    {
        #region 依赖属性

        public readonly static DependencyProperty ContentProperty = DependencyProperty.Register<object, ModalBodyPresenter>("Content", () => { return null; });

        public object Content
        {
            get
            {
                return GetValue(ContentProperty);
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }


        #endregion

        protected override void Draw(PageBrush brush)
        {
            var modal = this.BelongTemplate.TemplateParent as Modal;
            if(modal.BodyHeight > 0)
            {
                brush.DrawFormat("<div class=\"m-scrollable\" data-scrollbar-shown=\"true\" data-scrollable=\"true\" data-max-height=\"{0}\">",modal.BodyHeight);
                var e = this.Content as IUIElement;
                if (e != null)
                {
                    e.Render(brush);
                }
                brush.DrawFormat("</div>");
            }
            else
            {
                var e = this.Content as IUIElement;
                if (e != null)
                {
                    e.Render(brush);
                }
            }

        }
    }
}
