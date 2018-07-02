using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;

namespace CodeArt.DomainDriven.DataAccess
{
    /// <summary>
    /// 临时索引，主要用于记录已处理的表信息，防止运行时死循环
    /// </summary>
    internal class TempIndex
    {
        private List<string> _tableKeys;

        private TempIndex()
        {
            _tableKeys = new List<string>();
        }

        /// <summary>
        /// 尝试将表加入到临时索引中，如果表不存在于索引中，那么可以成功添加到索引，并返回true
        /// 否则返回false
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public bool TryAdd(DataTable table)
        {
            if (_tableKeys.Contains(table.UniqueKey)) return false;
            _tableKeys.Add(table.UniqueKey);
            return true;
        }

        private void Clear()
        {
            _tableKeys.Clear();
        }

        private static Pool<TempIndex> _pool = new Pool<TempIndex>(() =>
        {
            return new TempIndex();
        }, (index, phase) =>
        {
            if (phase == PoolItemPhase.Returning)
            {
                index.Clear();
            }
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 300 //闲置时间300秒
        });


        public static IPoolItem<TempIndex> Borrow()
        {
            return _pool.Borrow();
        }

    }
}
