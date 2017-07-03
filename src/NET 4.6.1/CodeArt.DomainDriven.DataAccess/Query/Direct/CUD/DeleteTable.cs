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
    /// 删除数据
    /// </summary>
    internal class DeleteTable : SingleTableOperation
    {
        private string _sql;

        private DeleteTable(DataTable target)
            : base(target)
        {
            _sql = GetSql();
        }

        protected override string GetName()
        {
            return string.Format("Delete {0}", this.Target.Name);
        }

        private string GetSql()
        {
            switch (SqlContext.GetDbType())
            {
                case DatabaseType.SQLServer: return SQLServer.SqlStatement.GetDeleteSql(this.Target);
            }
            return null;
        }

        protected override string Process(DynamicData param)
        {
            return _sql;
        }

        public static DeleteTable Create(DataTable target)
        {
            return _getInstance(target);
        }

        private static Func<DataTable, DeleteTable> _getInstance = LazyIndexer.Init<DataTable, DeleteTable>((target) =>
        {
            return new DeleteTable(target);
        });
    }
}