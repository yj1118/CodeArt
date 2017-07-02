using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// aggregate聚合的根
    /// 在每个Aggregate中，选择一个Entity作为根，并通过根控制对聚合内其他对象的所有访问。
    /// 只允许外部对象保持对根的引用。对内部成员的临时引用可以被传递出去，但仅在一次操作中有效。
    /// 由于根控制访问，因此不能绕过它来修改内部对象。
    /// 当提交对aggregate边界内部的任何对象的修改时，整个aggregate中的所有固定规则都必须被满足
    /// </summary>
    public interface IAggregateRoot : IEntityObject
    {
        /// <summary>
        /// 内聚根可以具有远程能力
        /// </summary>
        RemotableAttribute RemotableTip
        {
            get;
        }
    }
}
