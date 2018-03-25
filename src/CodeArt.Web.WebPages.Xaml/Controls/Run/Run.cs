using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml.Controls
{
    [ComponentLoader(typeof(RunLoader))]
    [ContentProperty("Content")]
    public class Run : FrameworkElement
    {
        #region 依赖属性

        public static DependencyProperty ContentProperty { get; private set; }

        static Run()
        {
            var contentMetadata = new PropertyMetadata(() => { return string.Empty; });
            ContentProperty = DependencyProperty.Register<string, Run>("Content", contentMetadata);
        }

        #endregion

        /// <summary>
        /// 文本内容
        /// </summary>
        public string Content
        {
            get
            {
                return GetValue(ContentProperty) as string;
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }

        protected override void Draw(PageBrush brush)
        {
            brush.Draw(this.Content, DrawOrigin.Current);
        }

        public static readonly Type Type = typeof(Run);

    }
}
