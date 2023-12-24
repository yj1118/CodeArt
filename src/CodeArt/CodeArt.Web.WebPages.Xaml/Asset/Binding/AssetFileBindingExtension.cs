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

namespace CodeArt.Web.WebPages.Xaml
{
    [TypeConverter(typeof(AssetFileBindingExtensionConverter))]
    public class AssetFileBindingExtension : MarkupExtension
    {
        /// <summary>
        /// 资产文件编号
        /// </summary>
        public string AssetFileKey { get; private set; }

        /// <summary>
        /// 附加字符串，比如 ?id=1 或 #target
        /// </summary>
        public string AdditionalString { get; private set; }


        public AssetFileBindingExtension(string assetKey,string additionalString)
        {
            this.AssetFileKey = assetKey;
            this.AdditionalString = additionalString;
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
            return new AssetFileBindingExpression(this.AssetFileKey, this.AdditionalString);
        }
    }
}