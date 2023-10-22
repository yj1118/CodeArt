using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven
{
    public abstract class PersistRepository : IPersistRepository
    {
        /// <summary>
        /// 将对象添加到持久层中
        /// </summary>
        /// <param name="obj"></param>
        public abstract void PersistAdd(IAggregateRoot obj);

        /// <summary>
        /// 修改对象在持久层中的信息
        /// </summary>
        /// <param name="obj"></param>
        public abstract void PersistUpdate(IAggregateRoot obj);

        /// <summary>
        /// 从持久层中删除对象
        /// </summary>
        /// <param name="obj"></param>
        public abstract void PersistDelete(IAggregateRoot obj);

        #region 事件


        public event RepositoryPrePersistEventHandler PrePersist;

        /// <summary>
        /// 执行仓储操作之前
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        protected bool OnPrePersist(IAggregateRoot obj, RepositoryAction action)
        {
            if (this.PrePersist != null)
            {
                RepositoryPrePersistEventArgs args = new RepositoryPrePersistEventArgs(obj, action);
                this.PrePersist(this, args);
                return args.Allow;
            }
            return true;
        }


        public event RepositoryPersistedEventHandler Persisted;

        /// <summary>
        /// 执行仓储操作之后
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="action"></param>
        protected void OnPersisted(IAggregateRoot obj, RepositoryAction action)
        {
            if (this.Persisted != null)
            {
                RepositoryPersistedEventArgs args = new RepositoryPersistedEventArgs(obj, action);
                this.Persisted(this, args);
            }
        }

        public event RepositoryRollbackEventHandler Rollback;
        public virtual void OnRollback(object sender, RepositoryRollbackEventArgs e)
        {
            if (this.Rollback != null)
            {
                this.Rollback(sender, e);
            }
        }




        public virtual void OnAddCommited(IAggregateRoot obj)
        {

        }

        public virtual void OnUpdateCommited(IAggregateRoot obj)
        {

        }

        public virtual void OnDeleteCommited(IAggregateRoot obj)
        {

        }


        #endregion



    }
}
