using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

using CodeArt;
using CodeArt.Concurrent;
using CodeArt.Util;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class StatusEventArgs
    {
        private Dictionary<string,object> _data = new Dictionary<string, object>();

        internal StatusEventArgs() { }

        public void Set(string name, object value)
        {
            _data[name] = value;
        }

        public T Get<T>(string name, bool throwError = false)
        {
            if (_data.TryGetValue(name, out var value))
            {
                return DataUtil.ToValue<T>(value);
            }
            if (throwError) throw new DomainDrivenException("在状态事件上下文中没有找到" + name);
            return default(T);
        }

        public bool Contains(string name)
        {
            return _data.ContainsKey(name);
        }

        internal void Clear()
        {
            _data.Clear();
        }


        internal static Pool<StatusEventArgs> Pool = new Pool<StatusEventArgs>(() =>
        {
            return new StatusEventArgs();
        }, (obj, phase) =>
        {
            if(phase == PoolItemPhase.Returning)
            {
                obj.Clear();
            }
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 300 //5分钟之内未被使用，就移除
        });


    }
}