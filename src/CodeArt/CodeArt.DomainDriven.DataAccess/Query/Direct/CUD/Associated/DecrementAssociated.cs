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
    /// 减少引用次数
    /// </summary>
    internal class DecrementAssociated : SingleTableOperation
    {
        private string _sql;

        private DecrementAssociated(DataTable target)
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
            throw new NotSupportDatabaseException("GetDecrementAssociatedSql - " + this.Target.Type.ToString());
        }

        protected override string Process(DynamicData param)
        {
            return _sql;
        }

        public static DecrementAssociated Create(DataTable target)
        {
            return _getInstance(target);
        }

        private static Func<DataTable, DecrementAssociated> _getInstance = LazyIndexer.Init<DataTable, DecrementAssociated>((target) =>
        {
            return new DecrementAssociated(target);
        });




        private string GetSqlBySQLServer()
        {
            return string.Format("update [{0}] set [{3}]=[{3}]-1 where [{1}]=@{1} and [{2}]=@{2};",
                                                       this.Target.Name
                                                       , GeneratedField.RootIdName
                                                       , EntityObject.IdPropertyName
                                                       , GeneratedField.AssociatedCountName);
        }


    }
}