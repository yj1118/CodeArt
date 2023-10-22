using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Data;

using CodeArt.Util;

namespace CodeArt.DomainDriven.DataAccess
{
    /// <summary>
    /// 数据上下文扩展
    /// </summary>
    public static class DataConnectionExtensions
    {
        private static Func<string, Func<string, ISqlPageTemplate>> _getSqlPageTemplate = LazyIndexer.Init<string, Func<string, ISqlPageTemplate>>((fromSql) =>
        {
            return LazyIndexer.Init<string, ISqlPageTemplate>((orderBySql) =>
            {
                var dbType = SqlContext.GetDbType();
                switch (dbType)
                {
                    case DatabaseType.SQLServer:
                        {
                            var template = new SQLServer.SqlPageTemplate();
                            template.Select("*");
                            template.From("("+ fromSql + ") as __pageTemp");//不需要where，因为fromSql内部已经处理了
                            template.OrderBy(orderBySql);
                            return template;
                        }
                }
                throw new DataAccessException("尚不支持的翻页查询"+ dbType);
            });
        });


        /// <summary>
        /// 由数据上下文托管的基于对象表达式的查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static IEnumerable<dynamic> ExecutePage(this DataConnection conn,string fromSql,string orderBySql,int pageIndex,int pageSize,object param=null)
        {
            var template = _getSqlPageTemplate(fromSql)(orderBySql);
            string sql = template.GetCode(pageIndex, pageSize);

            return conn.Query(sql, param);
        }
    }
}
