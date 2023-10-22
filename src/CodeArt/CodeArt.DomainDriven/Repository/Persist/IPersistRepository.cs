using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven
{
    public interface IPersistRepository
    {
        /// <summary>
        /// 将对象添加到持久层中
        /// </summary>
        /// <param name="obj"></param>
        void PersistAdd(IAggregateRoot obj);

        /// <summary>
        /// 修改对象在持久层中的信息
        /// </summary>
        /// <param name="obj"></param>
        void PersistUpdate(IAggregateRoot obj);

        /// <summary>
        /// 从持久层中删除对象
        /// </summary>
        /// <param name="obj"></param>
        void PersistDelete(IAggregateRoot obj);

        event RepositoryPersistedEventHandler Persisted;

        event RepositoryPrePersistEventHandler PrePersist;

        event RepositoryRollbackEventHandler Rollback;
        void OnRollback(object sender, RepositoryRollbackEventArgs e);

        void OnAddCommited(IAggregateRoot obj);

        void OnUpdateCommited(IAggregateRoot obj);

        void OnDeleteCommited(IAggregateRoot obj);
    }
}
