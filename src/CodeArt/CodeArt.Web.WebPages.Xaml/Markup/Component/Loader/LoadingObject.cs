using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using CodeArt.Runtime;

using HtmlAgilityPack;
using CodeArt.HtmlWrapper;
using CodeArt.AppSetting;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    /// <summary>
    /// 表示当前线程的加载信息
    /// </summary>
    public sealed class LoadingObject
    {
        /// <summary>
        /// 正在加载的对象
        /// </summary>
        public object Target
        {
            get;
            private set;
        }

        /// <summary>
        /// 正在加载的属性
        /// </summary>
        public string PropertyName
        {
            get
            {
                return _propertyNames.Count > 0 ? _propertyNames.First() : null;
            }
        }

        private Stack<string> _propertyNames = new Stack<string>();

        internal void Push(string propertyName)
        {
            _propertyNames.Push(propertyName);
        }

        internal void Pop()
        {
            _propertyNames.Pop();
        }

        public LoadingObject(object target)
        {
            this.Target = target;
        }


    }
}
