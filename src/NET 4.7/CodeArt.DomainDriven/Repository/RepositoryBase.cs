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

        public void Add(TRoot obj)
        {
            RegisterRollbackAdd(obj);
            DomainContext.Execute(DomainEvent.PreAdd, obj);
            obj.OnPreAdd();
            RegisterAdded(obj);
            obj.OnAdded();
            DomainContext.Execute(DomainEvent.Added, obj);
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
            DomainContext.Execute(DomainEvent.AddCommitted, obj);
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

        public void Update(TRoot obj)
        {
            RegisterRollbackUpdate(obj);
            DomainContext.Execute(DomainEvent.PreUpdate, obj);
            obj.OnPreUpdate();
            RegisterUpdated(obj);
            obj.OnUpdated();
            DomainContext.Execute(DomainEvent.Updated, obj);
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
            DomainContext.Execute(DomainEvent.UpdateCommitted, obj);
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

        public void Delete(TRoot obj)
        {
            RegisterRollbackDelete(obj);
            DomainContext.Execute(DomainEvent.PreDelete, obj);
            obj.OnPreDelete();
            RegisterDeleted(obj);
            obj.OnDeleted();
            DomainContext.Execute(DomainEvent.Deleted, obj);
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
            DomainContext.Execute(DomainEvent.DeleteCommitted, obj);
        }

        protected abstract void PersistDeleteRoot(TRoot obj);

        #endregion

        #region 查询数据

        IAggregateRoot IRepository.Find(object id, QueryLevel level)
        {
            return Find(id, level);
        }

        public TRoot Find(object id, QueryLevel level)
        {
            return DataContext.Current.RegisterQueried<TRoot>(level, () =>
            {
                return PersistFind(id, level);
            });
        }

        protected abstract TRoot PersistFind(object id, QueryLevel level);

        #endregion

    }
}
