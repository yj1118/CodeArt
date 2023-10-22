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
    /// <summary>
    /// 基于数据上下文的绑定表达式
    /// 该表达式还没有TemplateBindingExpression类那样考虑所有情况，需要根据实际使用来进行完善
    /// </summary>
    public class DataContextBindingExpression : Expression
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

        public DataContextBindingExpression(string path, IValueConverter converter, object parameter)
        {
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

        private object GetValue(object o)
        {
            var source = FindSource(o);
            object value = null;
            if(string.IsNullOrEmpty(this.Path))
            {
                //当没有指定path时，以源作为value
                value = source;

                var dtoValue = value as DTObject;
                if(dtoValue != null && dtoValue.IsSingleValue)
                {
                    value = dtoValue.GetValue();
                }
            }
            else
            {
                var dtoSource = source as DTObject;
                if (dtoSource != null)
                {
                    value = dtoSource.GetValue(this.Path, null);
                }
                else
                {
                    //在初始化阶段，DataContext还不存在，所以有可能从属性里加载，这里就要对属性进行判断
                    var pi = source.GetType().ResolveProperty(this.Path);
                    if(pi != null)
                    {
                        value = RuntimeUtil.GetPropertyValue(source, this.Path);
                    }
                }
            }

            if (this.Converter != null)
            {
                value = this.Converter.Convert(value, this.Parameter);
            }
            return value;
        }

        private object FindSource(object target)
        {
            var ca = target as CustomAttribute;
            if (ca != null) target = ca.Owner; //如果是自定义标签，那么用标签所在的对象作为目标

            var e = target as FrameworkElement;
            if (e == null) throw new XamlException("绑定表达式应用的对象必须是" + typeof(FrameworkElement).FullName);

            var p = e;
            while (p != null)
            {
                var d = p.DataContext;
                if (d != null) return d;
                p = p.Parent as FrameworkElement;
            }
            return target; //如果对象链上不包括DataContext，那么使用自身作为DataContext
        }


    }
}