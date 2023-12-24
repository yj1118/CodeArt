using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Concurrent;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml.Controls
{
    [ComponentLoader(typeof(HtmlElementLoader))]
    [ContentProperty("Children")]
    public class HtmlElement : FrameworkElement
    {
        #region 依赖属性

        public static DependencyProperty ChildrenProperty { get; private set; }

        public static DependencyProperty AttachedAttributesProperty { get; private set; }


        static HtmlElement()
        {
            var childrenMetadata = new PropertyMetadata(() => { return new UIElementCollection(); });
            ChildrenProperty = DependencyProperty.Register<UIElementCollection, HtmlElement>("Children", childrenMetadata);

            var attachedAttributesMetadata = new PropertyMetadata(() => { return new CustomAttributeCollection(); });
            AttachedAttributesProperty = DependencyProperty.Register<CustomAttributeCollection, HtmlElement>("AttachedAttributes", attachedAttributesMetadata);
        }


        #endregion

        [IgnoreBlank(false)]
        public UIElementCollection Children
        {
            get
            {
                return GetValue(ChildrenProperty) as UIElementCollection;
            }
            set
            {
                SetValue(ChildrenProperty, value);
            }
        }

        /// <summary>
        /// 附加自定义属性
        /// </summary>
        public CustomAttributeCollection AttachedAttributes
        {
            get
            {
                return GetValue(AttachedAttributesProperty) as CustomAttributeCollection;
            }
            set
            {
                SetValue(AttachedAttributesProperty, value);
            }
        }

        /// <summary>
        /// 标签名称
        /// </summary>
        public string TagName
        {
            get;
            internal set;
        }
        
        public bool IsSingNode
        {
            get;
            internal set;
        }

        private string GetFrontCode()
        {
            using (var temp = StringPool.Borrow())
            {
                var code = temp.Item;
                code.AppendFormat("<{0}", this.TagName);

                OutputProperties(code, UIElement.IdProperty,
                                                 UIElement.NameProperty,
                                                 UIElement.StyleProperty,
                                                 UIElement.ClassProperty);

                var proxyCode = this.ProxyCode;
                if (!string.IsNullOrEmpty(proxyCode) && !this.ProxyCodeIsExpression())
                    code.AppendFormat(" data-proxy=\"{0}\"", proxyCode);

                //if (!string.IsNullOrEmpty(this.Name))
                //    code.AppendFormat(" name=\"{0}\"", this.Name);

                //if (!string.IsNullOrEmpty(this.Style))
                //    code.AppendFormat(" style=\"{0}\"", this.Style);

                OutputAttributes(code);

                if (this.IsSingNode
                    && !string.Equals(this.TagName, "form", StringComparison.OrdinalIgnoreCase))  //修复form的BUG
                    code.Append("/");
                code.Append(">");
                return code.ToString();
            }
        }

        private void OutputAttributes(StringBuilder code)
        {
            var attached = this.AttachedAttributes;
            var local = this.Attributes;

            //先打印附加属性
            foreach (var item in attached)
            {
                var attr = item as CustomAttribute;
                if (local.GetAttribute(attr.Name) != null) continue; //本地属性有，那么不打印，这意味着输出本地属性
                XamlUtil.OutputAttribute(attr, code);
            }
            XamlUtil.OutputAttributes(local, code);
        }


        private void OutputProperties(StringBuilder code, params DependencyProperty[] properties)
        {
            foreach (var property in properties)
            {
                var value = this.GetValue(property);
                if (value != null)
                {
                    var strValue = value.ToString();
                    if (!string.IsNullOrEmpty(strValue))
                        code.AppendFormat(" {0}=\"{1}\"", property.Name.FirstToLower(), strValue);
                }
            }
        }


        private string GetBehindCode()
        {
            if (this.IsSingNode) return string.Empty;
            return string.Format("</{0}>", this.TagName);
        }

        protected override void Draw(PageBrush brush)
        {
            var frontCode = GetFrontCode();

            if(!string.IsNullOrEmpty(frontCode))
                brush.Draw(frontCode);
            var children = this.Children;
            foreach (var element in children)
            {
                var ui = element as UIElement;
                if(ui != null) ui.Render(brush);
            }

            var behindCode = GetBehindCode();
            if (!string.IsNullOrEmpty(behindCode))
                brush.Draw(behindCode);
        }

        public override DependencyObject GetChild(string childName)
        {
            return base.GetChild(childName) ?? this.Children.GetChild(childName);
        }

        public override IEnumerable<UIElement> GetActionElement(string actionName)
        {
            return this.Combine(base.GetActionElement(actionName), this.Children.GetActionElement(actionName));
        }

        public override void OnLoad()
        {
            this.Children.OnLoad();
            base.OnLoad();
        }


        public static readonly Type Type = typeof(HtmlElement);
    }
}
