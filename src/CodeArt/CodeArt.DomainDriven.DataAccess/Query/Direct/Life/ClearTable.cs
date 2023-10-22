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
    /// 基于某种目的直接对数据库进行操作
    /// </summary>
    internal class ClearTable : QueryBuilder
    {
        public override bool IsUser => false;

        private string _sql;
        private string _name;

        private ClearTable(string tableName)
            : base(null)
        {
            _sql = GetSql(tableName);
            _name = string.Format("clear {0}", tableName);
        }

        private string GetSql(string tableName)
        {
            switch (SqlContext.GetDbType())
            {
                case DatabaseType.SQLServer: return SQLServer.SqlStatement.GetClearTableSql(tableName);
            }
            return null;
        }

        protected override string GetName()
        {
            return _name;
        }


        protected override string Process(DynamicData param)
        {
            return _sql;
        }

        public static ClearTable Create(string tableName)
        {
            return _getInstance(tableName);
        }

        private static Func<string, ClearTable> _getInstance = LazyIndexer.Init<string, ClearTable>((tableName) =>
        {
            return new ClearTable(tableName);
        });
    }
}