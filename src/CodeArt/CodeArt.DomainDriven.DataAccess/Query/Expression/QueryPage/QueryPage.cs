using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.Util;

namespace CodeArt.DomainDriven.DataAccess
{
    public class QueryPage : QueryExpression
    {
        private ISqlPageTemplate _template;

        private QueryPage(DataTable target, string expression)
            : base(target, expression, QueryLevel.None)
        {
            if (this.Definition.IsCustom) return; //自定义查询，由程序员自行翻译
            _template = GetTemplate();
            CheckUpOrder();
        }

        private void CheckUpOrder()
        {
            if (this.Definition.Columns.Order.Count() == 0)
                throw new DataAccessException(Strings.PageNeedOrder);
        }


        private ISqlPageTemplate GetTemplate()
        {
            switch(SqlContext.GetDbType())
            {
                case DatabaseType.SQLServer: return GetSQLBySQLServer();
            }

            return null;
        }


        protected override string GetCommandText(DynamicData param)
        {
            if (_template != null)
            {
                var pageIndex = (int)param.Get("pageIndex");
                var pageSize = (int)param.Get("pageSize");
                return _template.GetCode(pageIndex, pageSize);
            }
            return null;
        }

        public static QueryPage Create(DataTable target, string expression)
        {
            return _getInstance(target)(expression);
        }

        private static Func<DataTable, Func<string, QueryPage>> _getInstance = LazyIndexer.Init<DataTable, Func<string, QueryPage>>((target) =>
        {
            return LazyIndexer.Init<string, QueryPage>((expression) =>
            {
                return new QueryPage(target, expression);
            });
        });


        private ISqlPageTemplate GetSQLBySQLServer()
        {
            var sql = new SQLServer.SqlPageTemplate();
            sql.Select("*");
            sql.From(this.GetObjectSql());//不需要where，因为GetObjectSql内部已经处理了
            sql.OrderBy(this.Definition);
            return sql;
        }


    }
}
