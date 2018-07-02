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

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    /// <summary>
    /// 一个内部包装类
    /// </summary>
    internal class ValueConverterExpressionWrapper : Expression
    {
        public Expression Expression { get; private set; }


        /// <summary>
        /// 值转换器
        /// </summary>
        public IValueConverter Converter { get; private set; }

        public object Parameter { get; private set; }

        public ValueConverterExpressionWrapper(Expression expression, IValueConverter converter, object parameter)
        {
            this.Expression = expression;
            this.Converter = converter;
            this.Parameter = parameter;
        }

        #region 设置值

        public override bool SetValue(DependencyObject d, DependencyProperty dp, object value)
        {
            d.SetValue(dp, value, true);
            return true;
        }

        public override bool SetValue(object o, string propertyName, object value)
        {
            var ui = o as UIElement;
            if (ui != null)
            {
                ui.SetValue(propertyName, value, true);
            }
            else
            {
                RuntimeUtil.SetPropertyValue(o, propertyName, value);
            }
            return true;
        }

        #endregion

        #region 获取值

        public override object GetValue(DependencyObject d, DependencyProperty dp)
        {
            return this.Converter.Convert(this.Expression.GetValue(d, dp), this.Parameter);
        }

        public override object GetValue(object o, string propertyName)
        {
            return this.Converter.Convert(this.Expression.GetValue(o, propertyName), this.Parameter);
        }

        #endregion


    }
}