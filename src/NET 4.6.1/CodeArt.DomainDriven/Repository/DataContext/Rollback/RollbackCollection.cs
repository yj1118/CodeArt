using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Text;


namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 回滚列表
    /// </summary>
    internal class RollbackCollection
    {
        private List<RepositoryRollbackEventArgs> _items = null;

        public RollbackCollection()
        {
            _items = new List<RepositoryRollbackEventArgs>();
        }

        public void Add(RepositoryRollbackEventArgs e)
        {
            _items.Add(e);
        }

        /// <summary>
        /// 执行回滚
        /// </summary>
        public void Execute(IDataContext sender)
        {
            if (_items.Count == 0) return;
            foreach (var e in _items)
            {
                e.Target.OnRollback(sender, e);
                e.Repository.OnRollback(sender, e);
            }
            this.Clear();//执行完毕后清理
        }

        /// <summary>
        /// 清理回滚列表
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }
    }
}
