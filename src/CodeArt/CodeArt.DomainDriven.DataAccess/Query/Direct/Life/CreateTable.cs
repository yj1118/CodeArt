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
    internal class CreateTable : SingleTableOperation
    {
        private string _sql;
        private string _name;

        private CreateTable(DataTable target)
            : base(target)
        {
            _sql = GetSql();
            _name = string.Format("create {0}", target.Name);
        }

        private string GetSql()
        {
            switch (SqlContext.GetDbType())
            {
                case DatabaseType.SQLServer: return SQLServer.SqlStatement.GetCreateTableSql(this.Target);
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

        public static CreateTable Create(DataTable target)
        {
            return _getInstance(target);
        }

        private static Func<DataTable, CreateTable> _getInstance = LazyIndexer.Init<DataTable, CreateTable>((target) =>
        {
            return new CreateTable(target);
        });
    }
}