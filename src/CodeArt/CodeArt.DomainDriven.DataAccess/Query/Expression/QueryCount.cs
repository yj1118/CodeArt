using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.Util;

namespace CodeArt.DomainDriven.DataAccess
{
    public class QueryCount : QueryExpression
    {
        private string _sql;

        private QueryCount(DataTable target, string expression, QueryLevel level)
            : base(target, expression, level)
        {
            if (this.Definition.IsCustom) return;
            _sql = string.Format("select count({0}) from {1}", EntityObject.IdPropertyName, this.GetObjectSql());
        }

        protected override string GetCommandText(DynamicData param)
        {
            return _sql;
        }

        public static QueryCount Create(DataTable target, string expression, QueryLevel level)
        {
            return _cache.GetInstance(target, expression, level);
        }

        private static ExpressionCache<QueryCount> _cache = new ExpressionCache<QueryCount>((target, expression, level) =>
        {
            return new QueryCount(target, expression, level);
        });

    }
}
