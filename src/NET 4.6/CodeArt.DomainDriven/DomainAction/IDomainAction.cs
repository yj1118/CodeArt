using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDomainAction
    {
        /// <summary>
        /// 行为名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 拥有该行为的类型
        /// </summary>
        Type OwnerType { get;}

        /// <summary>
        /// 默认元数据
        /// </summary>
        ActionMetadata DefaultMetadata { get; }

    }
}
