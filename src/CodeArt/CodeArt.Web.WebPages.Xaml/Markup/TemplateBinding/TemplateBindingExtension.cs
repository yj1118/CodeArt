using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages;
using CodeArt.Runtime;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    [TypeConverter(typeof(TemplateBindingExtensionConverter))]
    public class TemplateBindingExtension : MarkupExtension
    {
        /// <summary>
        /// 模板绑定标签的表达式
        /// </summary>
        public string Expression { get; private set; }


        public TemplateBindingExtension(string expression)
        {
            this.Expression = expression;
        }


        public override object ProvideValue(object target, DependencyProperty property)
        {
            return _ProvideValue(target,property.Name);
        }

        public override object ProvideValue(object target, string propertyName)
        {
            return _ProvideValue(target, propertyName);
        }

        private object _ProvideValue(object target, string propertyName)
        {
            var path = ValueConverterUtil.GetPath(this.Expression, true);
            var converter = ValueConverterUtil.GetConverter(this.Expression, target, propertyName);
            var parameter = ValueConverterUtil.GetParameter(this.Expression, target, propertyName);

            return new TemplateBindingExpression(path, converter, parameter);
        }

    }
}