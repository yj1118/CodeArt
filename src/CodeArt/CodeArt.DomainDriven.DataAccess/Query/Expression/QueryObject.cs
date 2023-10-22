using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.AppSetting;
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
            if (this.Definition.IsCustom) return; //自定义查询，由程序员自行翻译
            _sql = GetSql();
        }

        private string GetSql()
        {
            if (this.Definition.IsCustom) return string.Empty;//自定义查询，由程序员自行解析
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
            if(target.IsSessionEnabledMultiTenancy) 
                return _cache.GetInstance(target, expression, level);

            return _cache_session_no_tenant.GetInstance(target, expression, level);
        }

        private static ExpressionCache<QueryObject> _cache = new ExpressionCache<QueryObject>((target, expression, level) =>
        {
            return new QueryObject(target, expression, level);
        });

        private static ExpressionCache<QueryObject> _cache_session_no_tenant = new ExpressionCache<QueryObject>((target, expression, level) =>
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
