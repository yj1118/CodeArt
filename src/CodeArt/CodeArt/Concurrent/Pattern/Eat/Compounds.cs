using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Collections.Concurrent;

using System.Threading;
using CodeArt.IO;


namespace CodeArt.Concurrent.Pattern.Eat
{
    /// <summary>
    /// 复合物
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class Compounds<T> : IDisposable
    {
        private ConcurrentQueue<T> _items;

        public int Count
        {
            get
            {
                return _items.Count;
            }
        }

        private readonly int _maxStock;
        private readonly CompoundReplaceMode _replaceMode;
        private Rubbish _rubbish;

        public Compounds(int maxStock, CompoundReplaceMode replaceMode,Rubbish rubbish)
        {
            _maxStock = maxStock;
            _replaceMode = replaceMode;
            _rubbish = rubbish;
            _items = new ConcurrentQueue<T>();
        }

        public bool TryAdd(T newItem)
        {
            if (_maxStock > 0)
            {
                lock (_items)
                {
                    if (_items.Count < _maxStock)
                    {
                        _items.Enqueue(newItem);
                        return true;
                    }


                    if (_replaceMode == CompoundReplaceMode.HappyOld)
                    {
                        //喜旧
                        _rubbish.Add(newItem);
                        return false;
                    }
                    else
                    {
                        //喜新
                        T old = default(T);
                        if (_items.TryDequeue(out old))
                        {
                            _rubbish.Add(old);
                        }
                        _items.Enqueue(newItem); //加入当前的
                        return true;
                    }
                }
            }
            else
            {
                _items.Enqueue(newItem);
                return true;
            }
        }

        /// <summary>
        /// 使用食物，使用后会自动将食物销毁
        /// </summary>
        /// <param name="action"></param>
        public bool Using(Action<T> action)
        {
            T item = default(T);
            if(_items.TryDequeue(out item))
            {
                action(item);
                _rubbish.Add(item);
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            T item = default(T);
            while (_items.TryDequeue(out item))
            {
                Rubbish.DisposeItem(item);
            }
        }

    }
}
