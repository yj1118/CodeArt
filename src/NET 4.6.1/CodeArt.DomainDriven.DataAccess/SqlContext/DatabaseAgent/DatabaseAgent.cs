using System;
using System.Collections.Generic;

using CodeArt.DomainDriven;

namespace CodeArt.DomainDriven.DataAccess
{
    /// <summary>
    /// 数据库代理
    /// </summary>
    public abstract class DatabaseAgent : IDatabaseAgent
    {
        /// <summary>
        /// 数据库的名称
        /// </summary>
        public abstract string Database
        {
            get;
        }

        public virtual string Build(QueryBuilder builder, DynamicData param)
        {
            return null;
        }

    }
}