using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages;
using CodeArt.Runtime;
using CodeArt.DTO;

namespace CodeArt.Web.WebPages.Xaml
{
    public class BindingExpression : Expression
    {
        /// <summary>
        /// 绑定源的路径，源目标是应用模板的对象
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// 值转换器
        /// </summary>
        public IValueConverter Converter { get; private set; }

        public object Parameter { get; private set; }

        public IBindingSourceFinder SourceFinder { get; private set; }

        public BindingExpression(IBindingSourceFinder sourceFinder, string path, IValueConverter converter, object parameter)
        {
            this.SourceFinder = sourceFinder;
            this.Path = path;
            this.Converter = converter;
            this.Parameter = parameter;
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

        private object GetValue(object d)
        {
            var source = this.SourceFinder.Find(d);
            object value = null;
            var dtoSource = source as DTObject;
            if(dtoSource != null)
            {
                value = dtoSource.GetValue(this.Path);
            }
            else
            {
                value = RuntimeUtil.GetPropertyValue(source, this.Path);
            }

            if (this.Converter != null)
            {
                value = this.Converter.Convert(value, this.Parameter);
            }
            return value;
        }

    }
}