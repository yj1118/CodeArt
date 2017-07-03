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
    /// 查询1个或多个对象
    /// </summary>
    public class QueryObject : QueryExpression
    {
        private string _sql;

        private QueryObject(DataTable target, string expression, QueryLevel level)
            : base(target, expression, level)
        {
            _sql = GetSql();
        }

        private string GetSql()
        {
            switch (SqlContext.GetDbType())
            {
                case DatabaseType.SQLServer: return GetSqlBySQLServer();
            }
            return null;
        }


        protected override string GetCommandText(DynamicData param)
        {
            return _sql;
        }

        public static QueryObject Create(DataTable target, string expression, QueryLevel level)
        {
            return _cache.GetInstance(target, expression, level);
        }

        private static ExpressionCache<QueryObject> _cache = new ExpressionCache<QueryObject>((target, expression, level) =>
        {
            return new QueryObject(target, expression, level);
        });


        private string GetSqlBySQLServer()
        {
            return string.Format("select {0} * from {1} {2}",
                                        this.Definition.Top,
                                        this.GetObjectSql(),
                                        this.Definition.Order);
        }





    }
}
