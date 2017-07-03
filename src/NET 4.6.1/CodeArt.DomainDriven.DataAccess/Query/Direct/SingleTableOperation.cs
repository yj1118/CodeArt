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
    /// 单表操作
    /// </summary>
    public abstract class SingleTableOperation : QueryBuilder
    {
        public DataTable Target
        {
            get;
            private set;
        }

        public SingleTableOperation(DataTable target)
        {
            this.Target = target;
        }

        protected override string GetName()
        {
            return QueryBuilder.InternalQuery;
        }
    }
}
