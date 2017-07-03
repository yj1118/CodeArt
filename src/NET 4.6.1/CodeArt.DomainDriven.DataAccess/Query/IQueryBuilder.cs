using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace CodeArt.DomainDriven.DataAccess
{
    public interface IQueryBuilder
    {
        /// <summary>
        /// 每个查询都有一个唯一的名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 构建执行语句，在这个过程中有可能改变param的值
        /// </summary>
        /// <returns></returns>
        string Build(DynamicData param);
    }
}
