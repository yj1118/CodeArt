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
    /// 绑定表达式规则如下：
    /// 1.当绑定源的属性发生变化时，会触发属性更改事件
    /// 2.当获取目标的属性的值时，会取出绑定源的属性的值
    /// 3.当目标的属性被赋值时，如果赋的值是表达式，那么新的表达式将替换绑定表达式，这意味着目标的属性不再绑定
    /// 4.当目标的属性被赋值时，如果赋的值是一般对象，那么该对象将替换绑定表达式，这意味着目标的属性不再绑定
    /// </summary>
    public class TemplateBindingExpression : Expression
    {
        /// <summary>
        /// 绑定源的路径，源目标是应用模板的对象
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// 被绑定的属性名称
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// 值转换器
        /// </summary>
        public IValueConverter Converter { get; private set; }

        public object Parameter { get; private set; }

        /// <summary>
        /// 被绑定的源对象
        /// </summary>
        private object _source;

        private DependencyProperty _sourceProperty;

        private DependencyObject _target;

        private DependencyProperty _targetProperty;


        public TemplateBindingExpression(string path, IValueConverter converter, object parameter)
        {
            this.Path = path;
            this.Converter = converter;
            this.Parameter = parameter;
            this.PropertyName = GetPropertyName();
        }

        #region 表达式被绑定时

        public override void OnAttach(DependencyObject d, DependencyProperty dp)
        {
            _target = d;
            _targetProperty = dp;

            var templateCell = _target as ITemplateCell;
            if (templateCell == null)
                throw new XamlException("模板绑定表达式应用的对象必须是" + typeof(ITemplateCell).FullName);

            var template = templateCell.BelongTemplate; //得到应用绑定表达式对象所在的模板对象
            if (template == null)
                throw new XamlException("无法获取对象所在模板，不能应用模板绑定表达式" + _target.GetType().FullName);

            //截获模板的应用对象被改变事件
            template.TemplateParentChanged += (s, e) =>
            {
                if (_source != null)
                {
                    //移除老对象上绑定的事件
                    var dependencySource = _source as DependencyObject;
                    if (dependencySource != null)
                    {
                        IList<DependencyPropertyChangedEventHandler> handlers = null;
                        if (_events.TryGetValue(dependencySource, out handlers))
                        {
                            foreach (var handler in handlers)
                            {
                                dependencySource.RemovePropertyChanged(_sourceProperty, handler);
                            }
                            _events.Remove(dependencySource);
                        }
                    }
                }


                {
                    //在新的应用对象上追加事件
                    _source = LocateSource(e.NewValue);
                    var dependencySource = _source as DependencyObject;
                    if (_source == null) return;

                    _sourceProperty = DependencyProperty.GetProperty(dependencySource.ObjectType, this.PropertyName);
                    if (_sourceProperty == null)
                        throw new XamlException("在类型" + dependencySource.GetType() + "和其继承链上没有找到依赖属性" + this.Path + "的定义");
                    DependencyPropertyChangedEventHandler handler = (sp, ep) =>
                    {
                        _target.OnPropertyChanged(_targetProperty, ep.NewValue, ep.OldValue);
                    };
                    dependencySource.AddPropertyChanged(_sourceProperty, handler);
                    _events.Add(dependencySource, handler);

                    //由于改变了模板应用的对象，意味着_target的属性也被改变了，所以这里要主动触发一次
                    var newValue = dependencySource.GetValue(_sourceProperty);
                    _target.OnPropertyChanged(_targetProperty, newValue, null);
                }

            };
        }

        public override void OnAttach(object obj, string propertyName)
        {
            var d = obj as DependencyObject;
            if (d == null) throw new XamlException("模板绑定表达式应用的对象必须是" + typeof(DependencyObject).FullName);
            var dp = DependencyProperty.GetProperty(d.ObjectType, propertyName);
            if (dp != null) OnAttach(d, dp);
            else
            {
                _target = d;
            }
            
            //如果没有对应的dp，那么不需要通知触发属性变化事件
        }

        private MultiDictionary<object, DependencyPropertyChangedEventHandler> _events = new MultiDictionary<object, DependencyPropertyChangedEventHandler>(false);

      //  private void OnAttach(DependencyObject target, string propertyName)
       // {
           // var templateCell = target as ITemplateCell;
            //if (templateCell == null)
              //  throw new XamlException("模板绑定表达式应用的对象必须是" + typeof(ITemplateCell).FullName);

            //var template = templateCell.BelongTemplate; //得到应用绑定表达式对象所在的模板对象
            //if (template == null)
               // throw new XamlException("无法获取对象所在模板，不能应用模板绑定表达式" + target.GetType().FullName);

            //截获模板的应用对象被改变事件
           // template.TemplateParentChanged += (s, e) =>
            //{
                //if (e.OldValue != null)
                //{
                //    //移除老对象上绑定的事件
                //    var source = LocateSource(e.OldValue) as DependencyObject;
                //    if (source != null)
                //    {
                //        var property = DependencyProperty.GetProperty(source.ObjectType, this.PropertyName);
                //        IList<DependencyPropertyChangedEventHandler> handlers = null;
                //        if (_events.TryGetValue(source, out handlers))
                //        {
                //            foreach (var handler in handlers)
                //            {
                //                source.RemovePropertyChanged(property, handler);
                //            }
                //            _events.Remove(source);
                //        }
                //    }
                //}

              
                //{
                //    //在新的应用对象上追加事件
                //    var source = LocateSource(e.NewValue) as DependencyObject;
                //    if (source == null) return;

                //    var sourceProperty = DependencyProperty.GetProperty(source.ObjectType, this.PropertyName);
                //    if (sourceProperty == null)
                //        throw new XamlException("在类型" + source.GetType() + "和其继承链上没有找到依赖属性" + this.Path + "的定义");
                //    DependencyPropertyChangedEventHandler handler = (sp, ep) =>
                //    {
                //        BindValue(target, propertyName, source, sourceProperty);
                //    };
                //    source.AddPropertyChanged(sourceProperty, handler);
                //    _events.Add(source, handler);

                //    BindValue(target, propertyName, source, sourceProperty);
                //}
            //};

        //}

        private string GetPropertyName()
        {
            var path = this.Path;
            if (path.IndexOf(".") == -1) return this.Path;

            var temp = path.Split('.');
            return temp[temp.Length - 1];
        }

        /// <summary>
        /// 根据路径定位source,并返回绑定的属性名称
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private object LocateSource(object source)
        {
            var path = this.Path;
            if (path.IndexOf(".") == -1) return source;

            var propertyNames = path.Split('.');
            var value = source;
            var lastIndex = propertyNames.Length - 1;
            for (var i = 0; i < lastIndex; i++)
            {
                var propertyName = propertyNames[i];
                value = RuntimeUtil.GetPropertyValue(value, propertyName);
                if (value == null) break;
            }
            return value;
        }

        //private void BindValue(DependencyObject target, string targetPropertyName)
        //{
        //var source = _source as DependencyObject;
        //if (source == null) return;

        ////此处需要注意，如果是表达式，需要使用表达式
        //var sourceValue = source.GetActualValue(_sourceProperty);
        //if (this.Converter != null)
        //{
        //    var parameter = this.Parameter ?? target;//默认将目标对象作为参数传递
        //    var exp = sourceValue as Expression;
        //    if (exp == null)
        //        sourceValue = this.Converter.Convert(sourceValue, parameter);
        //    else
        //        sourceValue = new ValueConverterExpressionWrapper(exp, this.Converter, parameter);
        //}

        //target.SetValue(targetPropertyName, sourceValue);
        //}

        #endregion


        #region 设置值

        public override bool SetValue(DependencyObject d, DependencyProperty dp, object value)
        {
            d.ClearExpression(dp); //清理属性的值,移除表达式，设置值为默认值
            d.SetValue(dp, value);
            return true;
        }

        public override bool SetValue(object o, string propertyName, object value)
        {
            var d = o as DependencyObject;
            if (d != null)
            {
                if (_targetProperty != null)
                    SetValue(d, _targetProperty, value);
                else
                    d.SetValue(propertyName, value);
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
            return GetValueFromSource(d);
        }

        public override object GetValue(object o, string propertyName)
        {
            return GetValueFromSource(o);
        }

        private object GetValueFromSource(object target)
        {
            if (_source == null) return null;

            object value = null;
            var ui = _source as UIElement;
            if (ui != null)
            {
                value = ui.GetValue(_sourceProperty);
            }
            else
            {
                value = RuntimeUtil.GetPropertyValue(_source, this.PropertyName);
            }

            if (this.Converter != null)
            {
                value = this.Converter.Convert(value, this.Parameter);
            }
            return value;
        }

        #endregion

    }
}