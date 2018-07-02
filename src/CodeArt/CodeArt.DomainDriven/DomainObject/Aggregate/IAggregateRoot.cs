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
        /// 对象在仓储中的唯一键，这不是领域驱动的概念，是框架为了优化系统性能追加的属性
        /// 在同样的类型下，该值也可以用于判断唯一性
        /// </summary>
        string UniqueKey
        {
            get;
        }

        /// <summary>
        /// 仓储操作回滚
        /// </summary>
        event RepositoryRollbackEventHandler Rollback;

        void OnRollback(object sender, RepositoryRollbackEventArgs e);


        event RepositoryEventHandler PreAdd;
        /// <summary>
        /// 加入仓储之前
        /// </summary>
        void OnPreAdd();

        event RepositoryEventHandler Added;
        /// <summary>
        /// 加入仓储之后
        /// </summary>
        void OnAdded();

        event RepositoryEventHandler PreUpdate;
        /// <summary>
        /// 修改之前
        /// </summary>
        void OnPreUpdate();

        event RepositoryEventHandler Updated;
        /// <summary>
        /// 修改之后
        /// </summary>
        void OnUpdated();

        event RepositoryEventHandler PreDelete;
        /// <summary>
        /// 删除之前
        /// </summary>
        void OnPreDelete();

        event RepositoryEventHandler Deleted;
        /// <summary>
        /// 删除之后
        /// </summary>
        void OnDeleted();

        /// <summary>
        /// 提交增加操作之前
        /// </summary>
        event RepositoryEventHandler AddPreCommit;

        /// <summary>
        void OnAddPreCommit();

        event RepositoryEventHandler AddCommitted;
        void OnAddCommitted();


        /// <summary>
        /// 
        /// </summary>
        event RepositoryEventHandler UpdatePreCommit;

        /// <summary>
        void OnUpdatePreCommit();

        event RepositoryEventHandler UpdateCommitted;
        void OnUpdateCommitted();

        /// <summary>
        /// 
        /// </summary>
        event RepositoryEventHandler DeletePreCommit;

        /// <summary>
        void OnDeletePreCommit();

        event RepositoryEventHandler DeleteCommitted;
        void OnDeleteCommitted();

    }
}
