using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.Util;

namespace CodeArt.DomainDriven.DataAccess
{
    /// <summary>
    /// 一个简单的表达式缓存，用于缓存查询表达式
    /// </summary>
    internal class ExpressionCache<T> where T : QueryExpression
    {
        private Func<DataTable, Func<string, Func<QueryLevel, T>>> _getInstance;

        public ExpressionCache(Func<DataTable, string, QueryLevel, T> factory)
        {
            _getInstance = LazyIndexer.Init<DataTable, Func<string, Func<QueryLevel, T>>>((table) =>
            {
                return LazyIndexer.Init<string, Func<QueryLevel, T>>((expression) =>
                {
                    return LazyIndexer.Init<QueryLevel, T>((level) =>
                    {
                        return factory(table, expression, level);
                    });
                });
            });
        }

        public T GetInstance(DataTable target, string expression, QueryLevel level)
        {
            return _getInstance(target)(expression)(level);
        }
    }
}
