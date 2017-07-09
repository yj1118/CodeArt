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
    internal class SingleLock : SingleTableOperation
    {
        private string _sql;

        private SingleLock(DataTable target)
            : base(target)
        {
            _sql = GetSql();
        }

        private string GetSql()
        {
            switch (SqlContext.GetDbType())
            {
                case DatabaseType.SQLServer:
                    {
                        return SQLServer.SqlStatement.GetSingleLockSql(this.Target);
                    }
            }
            return null;
        }

        protected override string GetName()
        {
            return "SingleLock";
        }

        protected override string Process(DynamicData param)
        {
            return _sql;
        }

        public static SingleLock Create(DataTable target)
        {
            return _getInstance(target);
        }

        private static Func<DataTable, SingleLock> _getInstance = LazyIndexer.Init<DataTable, SingleLock>((target) =>
        {
            return new SingleLock(target);
        });
    }
}