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
            if (builder.ObjectType == null) return string.Empty; //没有定义对象类型就不需要映射
            var mapper = DataMapperFactory.Create(builder.ObjectType);
            return mapper.Build(builder, param);
        }
    }
}