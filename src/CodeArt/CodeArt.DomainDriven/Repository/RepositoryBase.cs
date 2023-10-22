using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Transactions;

using CodeArt;

namespace CodeArt.DomainDriven
{
    public abstract class Repository<TRoot> : PersistRepository,IRepository<TRoot>
        where TRoot : class, IAggregateRoot
    {
        #region 增加数据

        protected void RegisterAdded(IAggregateRoot obj)
        {
            DataContext.Current.RegisterAdded(obj, this);
        }

        protected void RegisterRollbackAdd(IAggregateRoot obj)
        {
            var args = new RepositoryRollbackEventArgs(obj, this, RepositoryAction.Add);
            DataContext.Current.RegisterRollback(args);
        }

        public virtual void Add(TRoot obj)
        {
            if (obj.IsEmpty()) return;

            DataContext.Using(()=>
            {
                RegisterRollbackAdd(obj);
                StatusEvent.Execute(StatusEventType.PreAdd, obj);
                obj.OnPreAdd();
                RegisterAdded(obj);
                obj.OnAdded();
                StatusEvent.Execute(StatusEventType.Added, obj);
            });
        }

        public void Add(IAggregateRoot obj)
        {
            this.Add((TRoot)obj);
        }


        public override void PersistAdd(IAggregateRoot obj)
        {
            if (obj.IsEmpty()) return;
            TRoot root = obj as TRoot;
            if (root != null)
            {
                if (this.OnPrePersist(obj, RepositoryAction.Add))
                {
                    PersistAddRoot(root);
                }
                this.OnPersisted(obj, RepositoryAction.Add);
            }
        }

        protected abstract void PersistAddRoot(TRoot obj);

        #endregion

        #region 修改数据

        protected void RegisterRollbackUpdate(IAggregateRoot obj)
        {
            var args = new RepositoryRollbackEventArgs(obj, this, RepositoryAction.Update);
            DataContext.Current.RegisterRollback(args);
        }

        protected void RegisterUpdated(IAggregateRoot obj)
        {
            DataContext.Current.RegisterUpdated(obj, this);
        }

        public virtual void Update(TRoot obj)
        {
            if (obj.IsEmpty()) return;

            DataContext.Using(() =>
            {
                RegisterRollbackUpdate(obj);
                StatusEvent.Execute(StatusEventType.PreUpdate, obj);
                obj.OnPreUpdate();
                RegisterUpdated(obj);
                obj.OnUpdated();
                StatusEvent.Execute(StatusEventType.Updated, obj);
            });
        }

        public void Update(IAggregateRoot obj)
        {
            this.Update((TRoot)obj);
        }

        public override void PersistUpdate(IAggregateRoot obj)
        {
            if (obj.IsEmpty()) return;
            TRoot root = obj as TRoot;
            if (root != null)
            {
                if (this.OnPrePersist(obj, RepositoryAction.Update))
                {
                    PersistUpdateRoot(root);
                }
                this.OnPersisted(obj, RepositoryAction.Update);
            }
        }

        protected abstract void PersistUpdateRoot(TRoot obj);

        #endregion

        #region 删除数据

        protected void RegisterRollbackDelete(IAggregateRoot obj)
        {
            var args = new RepositoryRollbackEventArgs(obj, this, RepositoryAction.Delete);
            DataContext.Current.RegisterRollback(args);
        }

        protected void RegisterDeleted(IAggregateRoot obj)
        {
            DataContext.Current.RegisterDeleted(obj, this);
        }

        public virtual void Delete(TRoot obj)
        {
            if (obj.IsEmpty()) return;

            DataContext.Using(() =>
            {
                RegisterRollbackDelete(obj);
                StatusEvent.Execute(StatusEventType.PreDelete, obj);
                obj.OnPreDelete();
                RegisterDeleted(obj);
                obj.OnDeleted();
                StatusEvent.Execute(StatusEventType.Deleted, obj);
            });
        }

        public void Delete(IAggregateRoot obj)
        {
            this.Delete((TRoot)obj);
        }

        public override void PersistDelete(IAggregateRoot obj)
        {
            if (obj.IsEmpty()) return;
            TRoot root = obj as TRoot;
            if (root != null)
            {
                if (this.OnPrePersist(obj, RepositoryAction.Delete))
                {
                    PersistDeleteRoot(root);
                }
                this.OnPersisted(obj, RepositoryAction.Delete);
            }
        }

        protected abstract void PersistDeleteRoot(TRoot obj);

        #endregion

        #region 查询数据

        IAggregateRoot IRepository.Find(object id, QueryLevel level)
        {
            return Find(id, level);
        }

        public virtual TRoot Find(object id, QueryLevel level)
        {
            TRoot result = null;
            DataContext.Using(()=>
            {
                result = DataContext.Current.RegisterQueried<TRoot>(level, () =>
                {
                    return PersistFind(id, level);
                });
            });
            return result;
        }

        protected abstract TRoot PersistFind(object id, QueryLevel level);

        #endregion

    }
}
