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
        public override bool IsUser => false;

        public DataTable Target
        {
            get;
            private set;
        }

        public SingleTableOperation(DataTable target)
            : base(target.ObjectType)
        {
            this.Target = target;
        }

        protected override string GetName()
        {
            return QueryBuilder.InternalQuery;
        }
    }
}
