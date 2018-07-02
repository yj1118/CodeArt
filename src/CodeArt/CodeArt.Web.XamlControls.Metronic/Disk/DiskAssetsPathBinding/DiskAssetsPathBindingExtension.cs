using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages;
using CodeArt.Runtime;

namespace CodeArt.Web.XamlControls.Metronic
{
    [TypeConverter(typeof(DiskAssetsPathBindingExtensionConverter))]
    public class DiskAssetsPathBindingExtension : MarkupExtension
    {
        /// <summary>
        /// 模板绑定标签的表达式
        /// </summary>
        public string Expression { get; private set; }


        public DiskAssetsPathBindingExtension(string expression)
        {
            this.Expression = expression;
        }


        public override object ProvideValue(object target, DependencyProperty property)
        {       
            return ProvideValue();
        }

        public override object ProvideValue(object target, string propertyName)
        {
            return ProvideValue();
        }

        private object ProvideValue()
        {
            string path = this.Expression;
            return new DiskAssetsPathBindingExpression(path);
        }



    }
}