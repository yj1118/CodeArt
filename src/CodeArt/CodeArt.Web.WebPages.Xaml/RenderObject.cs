using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;

namespace CodeArt.Web.WebPages.Xaml
{

    /// <summary>
    /// 表示当前线程正在渲染的对象
    /// </summary>
    public sealed class RenderObject
    {
        /// <summary>
        /// 正在渲染的对象
        /// </summary>
        public object Target
        {
            get;
            private set;
        }

        ///// <summary>
        ///// 正在加载的属性
        ///// </summary>
        //public string PropertyName
        //{
        //    get
        //    {
        //        return _propertyNames.Count > 0 ? _propertyNames.First() : null;
        //    }
        //}

        //private Stack<string> _propertyNames = new Stack<string>();

        //internal void Push(string propertyName)
        //{
        //    _propertyNames.Push(propertyName);
        //}

        //internal void Pop()
        //{
        //    _propertyNames.Pop();
        //}

        public RenderObject(object target)
        {
            this.Target = target;
        }


    }
}
