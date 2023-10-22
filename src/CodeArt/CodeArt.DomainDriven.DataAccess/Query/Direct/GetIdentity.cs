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
    /// 获取自增编号
    /// </summary>
    internal class GetIncrementIdentity : SingleTableOperation
    {
        private string _sql;

        private GetIncrementIdentity(DataTable target)
            : base(target)
        {
            _sql = GetSql();
        }

        private string GetSql()
        {
            switch (SqlContext.GetDbType())
            {
                case DatabaseType.SQLServer: return SQLServer.SqlStatement.GetIncrementIdentitySql(this.Target.Name);
            }
            return null;
        }

        protected override string Process(DynamicData param)
        {
            return _sql;
        }

        public static GetIncrementIdentity Create(DataTable target)
        {
            return _getInstance(target);
        }

        private static Func<DataTable, GetIncrementIdentity> _getInstance = LazyIndexer.Init<DataTable, GetIncrementIdentity>((target) =>
        {
            return new GetIncrementIdentity(target);
        });
    }
}