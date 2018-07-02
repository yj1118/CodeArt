using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml.Controls
{
    [ContentProperty("Content")]
    public class ContentPresenter : FrameworkElement
    {
        #region 依赖属性

        public static DependencyProperty ContentProperty { get; private set; }
    
        static ContentPresenter()
        {
            var contentMetadata = new PropertyMetadata(() => { return null; });
            ContentProperty = DependencyProperty.Register<object, ContentPresenter>("Content", contentMetadata);
        }

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
            var content = this.Content;
            if (content == null) return;
            var e = content as IUIElement;
            if (e != null)
            {
                e.Render(brush);
                return;
            }
            brush.Draw(content.ToString());
        }
    }
}
