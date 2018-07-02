using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Text;


namespace CodeArt.AppSetting
{
    /// <summary>
    /// 共生列表
    /// </summary>
    internal class SymbioticCollection
    {
        private List<IDisposable> _items = null;

        public SymbioticCollection()
        {
            _items = new List<IDisposable>();
        }

        public void Add(IDisposable item)
        {
            _items.Add(item);
        }

        public int Count
        {
            get
            {
                return _items.Count;
            }
        }

        /// <summary>
        /// 清理共生数据
        /// </summary>
        public void Clear()
        {
            foreach(var item in _items)
            {
                try
                {
                    item.Dispose();
                }
                catch(Exception ex)
                {

                }
            }
            _items.Clear();
        }
    }
}
