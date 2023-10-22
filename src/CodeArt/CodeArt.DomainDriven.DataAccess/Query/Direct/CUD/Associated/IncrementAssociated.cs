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
    internal class IncrementAssociated : SingleTableOperation
    {
        private string _sql;

        private IncrementAssociated(DataTable target)
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
            throw new NotSupportDatabaseException("GetIncrementAssociatedSql - " + this.Target.Type.ToString());
        }

        protected override string Process(DynamicData param)
        {
            return _sql;
        }

        public static IncrementAssociated Create(DataTable target)
        {
            return _getInstance(target);
        }

        private static Func<DataTable, IncrementAssociated> _getInstance = LazyIndexer.Init<DataTable, IncrementAssociated>((target) =>
        {
            return new IncrementAssociated(target);
        });

        private string GetSqlBySQLServer()
        {
            if(this.Target.IsSessionEnabledMultiTenancy)
            {
                return string.Format("update [{0}] set [{3}]=[{3}]+1 where [{1}]=@{1} and [{2}]=@{2}  and [{4}]=@{4} ;",
                           this.Target.Name
                           , GeneratedField.RootIdName
                           , EntityObject.IdPropertyName
                           , GeneratedField.AssociatedCountName
                           , GeneratedField.TenantIdName);
            }
            else
            {
                return string.Format("update [{0}] set [{3}]=[{3}]+1 where [{1}]=@{1} and [{2}]=@{2};",
                           this.Target.Name
                           , GeneratedField.RootIdName
                           , EntityObject.IdPropertyName
                           , GeneratedField.AssociatedCountName);
            }

        }

    }
}