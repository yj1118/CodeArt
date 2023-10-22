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
    /// 插入数据
    /// </summary>
    internal class InsertTable : SingleTableOperation
    {
        private string _sql;

        private InsertTable(DataTable target)
            : base(target)
        {
            _sql = GetInsertSql(target);
        }

        protected override string GetName()
        {
            return string.Format("Insert {0}", this.Target.Name);
        }


        protected override string Process(DynamicData param)
        {
            return _sql;
        }

        public static InsertTable Create(DataTable target)
        {
            return _getInstance(target);
        }

        private static Func<DataTable, InsertTable> _getInstance = LazyIndexer.Init<DataTable, InsertTable>((target) =>
        {
            return new InsertTable(target);
        });

        private static string GetInsertSql(DataTable table)
        {
            switch (SqlContext.GetDbType())
            {
                case DatabaseType.SQLServer: return GetSqlBySQLServer(table);
            }
            return null;
        }

        private static string GetSqlBySQLServer(DataTable table)
        {
            SQLServer.SqlInsertBuilder sql = new SQLServer.SqlInsertBuilder(table.Name);
            foreach (var field in table.Fields)
            {
                if (field.IsAdditional) continue; //附加字段由数据映射器维护
                sql.AddField(field.Name);
            }

            if (table.IsSnapshot && table.Type == DataTableType.AggregateRoot)
            {
                sql.AddField("SnapshotTime");
                sql.AddField("SnapshotLifespan");
            }

            if(table.IsSessionEnabledMultiTenancy)
            {
                sql.AddField(GeneratedField.TenantIdName);
            }

            return sql.GetCommandText();
        }
    }
}