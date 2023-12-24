using CodeArt.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    ///  静态属性的表达式
    /// </summary>
    internal class StaticReferenceExpression : Expression
    {
        private string _path;
        private Type _ownerType;
        private string _memberName;


        public StaticReferenceExpression(string path, Type ownerType, string memberName)
        {
            _path = path;
            _ownerType = ownerType;
            _memberName = memberName;
        }

        public override DependencySource[] GetSources()
        {
            return null;
        }

        /// <summary>
        /// 计算资源表达式的值
        /// </summary>
        /// <param name="d"></param>
        /// <param name="dp"></param>
        /// <returns></returns>
        public override object GetValue(DependencyObject d, DependencyProperty dp)
        {
            return _GetValue();
        }

        public override object GetValue(object o, string propertyName)
        {
            return _GetValue();
        }

        private object _GetValue()
        {
            object value = _ownerType.GetStaticValue(_memberName);
            if (value != null) return value;
            throw new XamlException("没有找到静态值" + _path);
        }

        /// <summary>
        /// 不能为资源设置值
        /// </summary>
        /// <param name="d"></param>
        /// <param name="dp"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool SetValue(DependencyObject d, DependencyProperty dp, object value)
        {
            return false;
        }
    }
}
