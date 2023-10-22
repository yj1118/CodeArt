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
    internal class GetSerialNumber : SingleTableOperation
    {
        private string _sql;

        private GetSerialNumber(DataTable target)
            : base(target)
        {
            _sql = GetSql();
        }

        private string GetSql()
        {
            switch (SqlContext.GetDbType())
            {
                case DatabaseType.SQLServer: return SQLServer.SqlStatement.GetSerialNumberSql(this.Target);
            }
            return null;
        }

        protected override string Process(DynamicData param)
        {
            return _sql;
        }

        public static GetSerialNumber Create(DataTable target)
        {
            return _getInstance(target);
        }

        private static Func<DataTable, GetSerialNumber> _getInstance = LazyIndexer.Init<DataTable, GetSerialNumber>((target) =>
        {
            return new GetSerialNumber(target);
        });
    }
}