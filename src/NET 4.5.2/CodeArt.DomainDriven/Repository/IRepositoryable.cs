using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 代表对象可以被仓储操作
    /// </summary>
    public interface IRepositoryable : IEntityObject
    {
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


    }
}
