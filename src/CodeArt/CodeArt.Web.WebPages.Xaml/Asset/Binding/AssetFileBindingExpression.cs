using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages;
using CodeArt.Runtime;

namespace CodeArt.Web.WebPages.Xaml
{
    public class AssetFileBindingExpression : Expression
    {
        /// <summary>
        /// 资产文件编号
        /// </summary>
        public string AssetFileKey { get; private set; }

        public AssetFileBindingExpression(string assetFileKey)
        {
            this.AssetFileKey = assetFileKey;
        }

        public override object GetValue(DependencyObject d, DependencyProperty dp)
        {
            return GetValue(d);
        }

        public override object GetValue(object o, string propertyName)
        {
            return GetValue(o);
        }

        public override bool SetValue(DependencyObject d, DependencyProperty dp, object value)
        {
            return false;
        }

        public override bool SetValue(object o, string propertyName, object value)
        {
            return false;
        }


        private object GetValue(object o)
        {
            var obj = o as ITemplateCell;
            if (obj == null) throw new XamlException("资产绑定表达式应用的对象必须是" + typeof(ITemplateCell).FullName);
            var source = obj.BelongTemplate;

            var file = source.GetFile(this.AssetFileKey);
            if (file == null) throw new XamlException("没有找到资产文件" + this.AssetFileKey);
            return file.VirtualPath;
        }

    }
}