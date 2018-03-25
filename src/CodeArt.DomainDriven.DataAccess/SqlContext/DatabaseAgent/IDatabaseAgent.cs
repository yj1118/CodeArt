using System;
using System.Collections.Generic;

using CodeArt.DomainDriven;

namespace CodeArt.DomainDriven.DataAccess
{
    /// <summary>
    /// 数据库代理
    /// </summary>
    public interface IDatabaseAgent
    {
        /// <summary>
        /// 数据库的名称
        /// </summary>
        string Database { get; }

        /// <summary>
        /// 作为数据库代理，可以截获构建器的职责，构建查询语句
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        string Build(QueryBuilder builder, DynamicData param, DataTable table);
    }
}