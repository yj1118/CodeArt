using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

using CodeArt;
using CodeArt.Util;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DomainEventArgs
    {
        private HybridDictionary _data = null;

        public DomainEventArgs() { }

        public void Set(string name, object value)
        {
            if (_data == null) _data = new HybridDictionary();
            _data[name] = value;
        }

        public T Get<T>(string name, bool throwError = false)
        {
            if (_data == null)
            {
                if (throwError) throw new DomainDrivenException("在领域事件上下文中没有找到" + name);
                return default(T);
            }
            var value = _data[name];
            if (throwError && value == null) throw new DomainDrivenException("在领域事件上下文中没有找到" + name);
            return DataUtil.ToValue<T>(value);
        }

        public bool Contains(string name)
        {
            if (_data == null) return false;
            return _data.Contains(name);
        }
    }
}