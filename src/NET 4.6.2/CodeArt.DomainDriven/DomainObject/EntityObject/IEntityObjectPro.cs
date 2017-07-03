using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven
{
    public interface IEntityObjectPro : IRepositoryable
    {
        /// <summary>
        /// 获取成员所属的内聚根
        /// </summary>
        /// <returns></returns>
        IAggregateRoot Root { get; }
    }


    /// <summary>
    /// 实体对象的富版本，可以依赖于内聚根进行单独的对象存储
    /// </summary>
    public interface IEntityObjectPro<TRoot> : IRepositoryable, IEntityObjectPro
        where TRoot : IAggregateRoot
    {
        new TRoot Root { get; }

    }
}