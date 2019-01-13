using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven.DataAccess
{
    /// <summary>
    /// 记录以根为从表的中间表的信息
    /// </summary>
    internal static class RootIsSlaveIndex
    {
        private static MultiDictionary<string, DataTable> _records = new MultiDictionary<string, DataTable>(false);

        /// <summary>
        /// 当middle的slave是根对象时，追加记录;
        /// 如果从表是根，那么需要记录从表和中间表的联系，当删除根对象时，会删除该中间表的数据
        /// </summary>
        /// <param name="slave"></param>
        /// <param name="middle"></param>
        /// <returns></returns>
        public static void TryAdd(DataTable middle)
        {
            var slave = middle.Slave;
            if (!slave.IsAggregateRoot) return;

            _records.TryAdd(slave.Name, middle, SameTable.Instance);
        }

        public static IEnumerable<DataTable> Get(DataTable slave)
        {
            if (!slave.IsAggregateRoot) return Array.Empty<DataTable>();
            return _records.GetValues(slave.Name);
        }

        private sealed class SameTable : IEqualityComparer<DataTable>
        {
            public bool Equals(DataTable x, DataTable y)
            {
                return x.Name == y.Name;
            }

            public int GetHashCode(DataTable obj)
            {
                return obj.Name.GetHashCode();
            }

            private SameTable() { }

            public static readonly SameTable Instance = new SameTable();
        }

    }
}
