using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml.Controls
{
    [ContentProperty("Content")]
    public class UserControl : Control, IUserControl
    {
        #region 依赖属性

        public static DependencyProperty ContentProperty { get; private set; }


        static UserControl()
        {
            var contentMetadata = new PropertyMetadata(() => { return null; });
            ContentProperty = DependencyProperty.Register<UIElement, UserControl>("Template", contentMetadata);
        }

        #endregion

        /// <summary>
        /// 获取或设置用户控件内包含的内容
        /// </summary>
        public UIElement Content
        {
            get
            {
                return GetValue(ContentProperty) as UIElement;
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }

        public override DependencyObject GetChild(string childName)
        {
            return base.GetChild(childName) ?? this.Content.GetChild(childName);
        }

        public override IEnumerable<UIElement> GetActionElement(string actionName)
        {
            return this.Combine(base.GetActionElement(actionName), this.Content.GetActionElement(actionName));
        }

        public override void OnLoad()
        {
            if(this.Content != null)
                this.Content.OnLoad();
            base.OnLoad();
        }

    }
}
