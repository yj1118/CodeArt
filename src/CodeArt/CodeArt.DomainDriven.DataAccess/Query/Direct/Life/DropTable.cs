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
    internal class DropTable : QueryBuilder
    {
        public override bool IsUser => false;

        private string _sql;
        private string _name;

        private DropTable(string tableName)
            : base(null)
        {
            _sql = GetSql(tableName);
            _name = string.Format("drop {0}", tableName);
        }

        private string GetSql(string tableName)
        {
            switch (SqlContext.GetDbType())
            {
                case DatabaseType.SQLServer: return SQLServer.SqlStatement.GetDropTableSql(tableName);
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

        public static DropTable Create(string tableName)
        {
            return _getInstance(tableName);
        }

        private static Func<string, DropTable> _getInstance = LazyIndexer.Init<string, DropTable>((tableName) =>
        {
            return new DropTable(tableName);
        });
    }
}