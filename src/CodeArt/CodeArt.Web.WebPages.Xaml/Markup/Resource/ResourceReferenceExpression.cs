using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    ///  引用资源的表达式
    /// </summary>
    internal class ResourceReferenceExpression : Expression
    {
        public object ResourceKey
        {
            get;private set;
        }


        public ResourceReferenceExpression(object resourceKey)
        {
            this.ResourceKey = resourceKey;
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
            var e = d as FrameworkElement;
            if (e == null) throw new XamlException("无法对类型" + e.GetType().FullName + "应用动态资源标记扩展表达式");
            return e.FindResource(this.ResourceKey);
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
