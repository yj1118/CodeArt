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
    /// 递增引用次数
    /// </summary>
    internal class SelectAssociated : SingleTableOperation
    {
        private string _sql;

        private SelectAssociated(DataTable target)
            : base(target)
        {
            _sql = GetSql();
        }

        private string GetSql()
        {
            if (this.Target.Type == DataTableType.EntityObject || this.Target.Type == DataTableType.ValueObject)
            {
                switch (SqlContext.GetDbType())
                {
                    case DatabaseType.SQLServer: return GetSqlBySQLServer();
                }
            }
            throw new NotSupportDatabaseException("GetSelectAssociated - " + this.Target.Type.ToString());
        }

        protected override string Process(DynamicData param)
        {
            return _sql;
        }

        public static SelectAssociated Create(DataTable target)
        {
            return _getInstance(target);
        }

        private static Func<DataTable, SelectAssociated> _getInstance = LazyIndexer.Init<DataTable, SelectAssociated>((target) =>
        {
            return new SelectAssociated(target);
        });

        private string GetSqlBySQLServer()
        {
            return string.Format("select [{0}] from [{1}] where [{2}]=@{2} and [{3}]=@{3};",
                                                                    GeneratedField.AssociatedCountName
                                                                   , this.Target.Name
                                                                   , GeneratedField.RootIdName
                                                                   , EntityObject.IdPropertyName);
        }
    }
}