using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml.Controls
{
    /// <summary>
    /// 输出自定义属性，请将该对象放在需要输出的节点的起始位置处，例如：
    /// <m:TextBox>
    /// <AttributesPresenter />
    /// ...
    /// ...
    /// ...
    /// </m:TextBox>
    /// </summary>
    public class AttributesPresenter : FrameworkElement
    {
        protected override void Draw(PageBrush brush)
        {
            var ui = (this.BelongTemplate.TemplateParent as UIElement);
            if (ui == null) return;
            var attrs = ui.Attributes;
            brush.Backspace(1); //移除 >
            XamlUtil.OutputAttributes(attrs, brush);
            brush.Draw(">");
        }
    }
}
