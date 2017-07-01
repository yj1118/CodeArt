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
        where TRoot : class, IAggregateRoot,IRepositoryable
    {
        #region 增加数据

        protected void RegisterAdded(IRepositoryable obj)
        {
            DataContext.Current.RegisterAdded(obj, this);
        }

        protected void RegisterRollbackAdd(IRepositoryable obj)
        {
            var args = new RepositoryRollbackEventArgs(obj, this, RepositoryAction.Add);
            DataContext.Current.RegisterRollback(args);
        }

        public void Add(TRoot obj)
        {
            RegisterRollbackAdd(obj);
            BoundedContext.Execute(BoundedEvent.PreAdd, obj);
            obj.OnPreAdd();
            RegisterAdded(obj);
            obj.OnAdded();
            BoundedContext.Execute(BoundedEvent.Added, obj);
        }

        public void AddEntityPro<TMember>(TMember obj) where TMember : class, IEntityObjectPro
        {
            RegisterRollbackAdd(obj);
            BoundedContext.Execute(BoundedEvent.PreAdd, obj);
            obj.OnPreAdd();
            RegisterAdded(obj);
            obj.OnAdded();
            BoundedContext.Execute(BoundedEvent.Added, obj);
        }

        public override void PersistAdd(IRepositoryable obj)
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
            else
            {
                var eop = obj as IEntityObjectPro;
                if (eop == null) throw new DomainDrivenException(string.Format(Strings.PersistTargetTypeError, obj.GetType().FullName));
                if (this.OnPrePersist(obj, RepositoryAction.Add))
                {
                    PersistAddEntityPro(eop);
                }
                this.OnPersisted(obj, RepositoryAction.Add);
            }
        }

        protected abstract void PersistAddRoot(TRoot obj);

        protected virtual void PersistAddEntityPro(IEntityObjectPro eop)
        {
            throw new NotImplementedException(this.GetType().FullName + ".PersistAddMember 方法没有实现对" + eop.GetType().FullName + "的仓储");
        }

        #endregion

        #region 修改数据

        protected void RegisterRollbackUpdate(IRepositoryable obj)
        {
            var args = new RepositoryRollbackEventArgs(obj, this, RepositoryAction.Update);
            DataContext.Current.RegisterRollback(args);
        }

        protected void RegisterUpdated(IRepositoryable obj)
        {
            DataContext.Current.RegisterUpdated(obj, this);
        }

        public void Update(TRoot obj)
        {
            RegisterRollbackUpdate(obj);
            BoundedContext.Execute(BoundedEvent.PreUpdate, obj);
            obj.OnPreUpdate();
            RegisterUpdated(obj);
            obj.OnUpdated();
            BoundedContext.Execute(BoundedEvent.Updated, obj);
        }

        public void UpdateEntityPro<TMember>(TMember obj) where TMember : class, IEntityObjectPro
        {
            RegisterRollbackUpdate(obj);
            BoundedContext.Execute(BoundedEvent.PreUpdate, obj);
            obj.OnPreUpdate();
            RegisterUpdated(obj);
            obj.OnUpdated();
            BoundedContext.Execute(BoundedEvent.Updated, obj);
        }

        public override void PersistUpdate(IRepositoryable obj)
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
            else
            {
                var eop = obj as IEntityObjectPro;
                if (eop == null) throw new DomainDrivenException(string.Format(Strings.PersistTargetTypeError, obj.GetType().FullName));
                if (this.OnPrePersist(obj, RepositoryAction.Update))
                {
                    PersistUpdateEntityPro(eop);
                }
                this.OnPersisted(obj, RepositoryAction.Update);
            }
        }

        protected abstract void PersistUpdateRoot(TRoot obj);

        protected virtual void PersistUpdateEntityPro(IEntityObjectPro eop)
        {
            throw new NotImplementedException(this.GetType().FullName + ".PersistUpdateMember 方法没有实现对" + eop.GetType().FullName + "的仓储");
        }

        #endregion

        #region 删除数据

        protected void RegisterRollbackDelete(IRepositoryable obj)
        {
            var args = new RepositoryRollbackEventArgs(obj, this, RepositoryAction.Delete);
            DataContext.Current.RegisterRollback(args);
        }

        protected void RegisterDeleted(IRepositoryable obj)
        {
            DataContext.Current.RegisterDeleted(obj, this);
        }

        public void Delete(TRoot obj)
        {
            RegisterRollbackDelete(obj);
            BoundedContext.Execute(BoundedEvent.PreDelete, obj);
            obj.OnPreDelete();
            RegisterDeleted(obj);
            obj.OnDeleted();
            BoundedContext.Execute(BoundedEvent.Deleted, obj);
        }

        public void DeleteEntityPro<TMember>(TMember obj) where TMember : class, IEntityObjectPro
        {
            RegisterRollbackDelete(obj);
            BoundedContext.Execute(BoundedEvent.PreDelete, obj);
            obj.OnPreDelete();
            RegisterDeleted(obj);
            obj.OnDeleted();
            BoundedContext.Execute(BoundedEvent.Deleted, obj);
        }

        public override void PersistDelete(IRepositoryable obj)
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
            else
            {
                var eop = obj as IEntityObjectPro;
                if (eop == null) throw new DomainDrivenException(string.Format(Strings.PersistTargetTypeError, obj.GetType().FullName));
                if (this.OnPrePersist(obj, RepositoryAction.Delete))
                {
                    PersistDeleteEntityPro(eop);
                }
                this.OnPersisted(obj, RepositoryAction.Delete);
            }
        }

        protected abstract void PersistDeleteRoot(TRoot obj);

        protected virtual void PersistDeleteEntityPro(IEntityObjectPro eop)
        {
            throw new NotImplementedException(this.GetType().FullName + ".PersistDeleteMember 方法没有实现对" + eop.GetType().FullName + "的仓储");
        }


        #endregion

        #region 锁定数据

        public void Lock(TRoot obj, QueryLevel level)
        {
            if (level == QueryLevel.None) throw new DomainDrivenException("仓储锁操作不允许使用 QueryLevel.None 级别");
            DataContext.Current.OpenLock(level);
            PersistLockRoot(obj, level);
        }

        protected virtual void PersistLockRoot(TRoot obj, QueryLevel level)
        {
            throw new NotImplementedException("没有实现" + typeof(TRoot).FullName + "的锁定方法");
        }

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

        public T FindEntityPro<T>(object rootId, object id) where T : class, IEntityObjectPro
        {
            //对于成员的加载，我们始终都是无锁的模式，只有内聚根的查询才考虑是否加锁
            return DataContext.Current.RegisterQueried<T>(QueryLevel.None, () =>
            {
                return PersistFindEntityPro<T>(rootId, id);
            });
        }

        protected abstract T PersistFindEntityPro<T>(object rootId, object id) where T : class, IEntityObjectPro;

        #endregion

    }
}
