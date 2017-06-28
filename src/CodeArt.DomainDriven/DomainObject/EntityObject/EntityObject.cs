using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven
{
    public abstract class EntityObject : DomainObject, IEntityObject, IRepositoryable
    {
        /// <summary>
        /// 统一领域对象中的标识符名称，这样在ORM处理等操作中会比较方便
        /// </summary>
        public const string IdPropertyName = "Id";

        public abstract object GetIdentity();

        protected EntityObject()
        {
            this.OnConstructed();
        }

        public override bool Equals(object obj)
        {
            var target = obj as EntityObject;
            if (target == null) return false;
            return this.GetIdentity() == target.GetIdentity();
        }

        public override int GetHashCode()
        {
            var key = this.GetIdentity();
            if (key != null) return key.GetHashCode();
            return 0;
        }

        public static bool operator ==(EntityObject entity0, EntityObject entity1)
        {
            if ((object)entity0 == null && (object)entity1 == null) return true;
            if ((object)entity0 == null || (object)entity1 == null) return false;
            return entity0.GetIdentity() == entity1.GetIdentity();
        }

        public static bool operator !=(EntityObject entity0, EntityObject entity1)
        {
            return !(entity0 == entity1);
        }


        /// <summary>
        /// 仓储操作回滚事件
        /// </summary>
        public event RepositoryRollbackEventHandler Rollback;

        public virtual void OnRollback(object sender, RepositoryRollbackEventArgs e)
        {
            if (this.Rollback != null)
            {
                this.Rollback(sender, e);
            }
        }

        public event RepositoryEventHandler PreAdd;

        public virtual void OnPreAdd()
        {
            if(this.PreAdd !=null)
            {
                var e = new RepositoryEventArgs(this, BoundedEvent.PreAdd);
                this.PreAdd(this, e);
            }
        }

        public event RepositoryEventHandler Added;
        public virtual void OnAdded()
        {
            if (this.Added != null)
            {
                var e = new RepositoryEventArgs(this, BoundedEvent.Added);
                this.Added(this, e);
            }
            CallOnceRepositoryActions();
        }

        public event RepositoryEventHandler PreUpdate;
        public virtual void OnPreUpdate()
        {
            if (this.PreUpdate != null)
            {
                var e = new RepositoryEventArgs(this, BoundedEvent.PreUpdate);
                this.PreUpdate(this, e);
            }
        }

        public event RepositoryEventHandler Updated;
        public virtual void OnUpdated()
        {
            if (this.Updated != null)
            {
                var e = new RepositoryEventArgs(this, BoundedEvent.Updated);
                this.Updated(this, e);
            }
            CallOnceRepositoryActions();
        }

        public event RepositoryEventHandler PreDelete;
        public virtual void OnPreDelete()
        {
            if (this.PreDelete != null)
            {
                var e = new RepositoryEventArgs(this, BoundedEvent.PreDelete);
                this.PreDelete(this, e);
            }
        }

        public event RepositoryEventHandler Deleted;
        public virtual void OnDeleted()
        {
            if (this.Deleted != null)
            {
                var e = new RepositoryEventArgs(this, BoundedEvent.Deleted);
                this.Deleted(this, e);
            }
            CallOnceRepositoryActions();
        }


        #region 仓储操作回调

        private List<Action> _onceRepositoryCallbackActions = null;

        /// <summary>
        /// 在下次执行完该对象的仓储操作后执行<paramref name="action" />动作
        /// 该动作仅被执行一次
        /// </summary>
        /// <param name="action"></param>
        protected void OnceRepositoryCallback(Action action)
        {
            if (_onceRepositoryCallbackActions == null) _onceRepositoryCallbackActions = new List<Action>();
            _onceRepositoryCallbackActions.Add(action);
        }

        private void CallOnceRepositoryActions()
        {
            if (_onceRepositoryCallbackActions == null) return;
            foreach (var action in _onceRepositoryCallbackActions) action();
            _onceRepositoryCallbackActions.Clear(); //执行完后清空行为集合
        }


        #endregion
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="OT">对象类型</typeparam>
    /// <typeparam name="KT">识别对象的唯一编号的类型</typeparam>
    public abstract class EntityObject<TObject, TIdentity> : EntityObject
        where TObject : EntityObject<TObject, TIdentity>
        where TIdentity : struct
    {
        /// <summary>
        /// 引用对象的唯一标示
        /// 引用对象的标示可以是本地(内聚范围内)唯一也可以是全局唯一
        /// </summary>
        [PropertyRepository(Snapshot = true)]                                               //TObject是子类的类型，EntityObject<TObject, TIdentity>才是当前类的类型
        private static readonly DomainProperty IdProperty = DomainProperty.Register<TIdentity, EntityObject<TObject, TIdentity>>(IdPropertyName, default(TIdentity));

        //private TIdentity _id;

        ///// <summary>
        ///// 引用对象的唯一标示
        ///// 引用对象的标示可以是本地(内聚范围内)唯一也可以是全局唯一
        ///// </summary>
        //[PropertyRepository(Snapshot = true)]
        //public TIdentity Id
        //{
        //    get { return GetValue<TIdentity>(IdProperty, _id); }
        //    private set
        //    {
        //        SetValue(IdProperty, ref _id, value);
        //    }
        //}


        public TIdentity Id
        {
            get { return GetValue<TIdentity>(IdProperty); }
            private set
            {
                SetValue(IdProperty, value);
            }
        }

        public override bool IsEmpty()
        {
            return this.Id.Equals(default(TIdentity));
        }

        public EntityObject(TIdentity id)
        {
            this.Id = id;
            this.OnConstructed();
        }

        public override bool Equals(object obj)
        {
            TObject target = obj as TObject;
            if (target == null) return false;
            return target.Id.Equals(this.Id);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override object GetIdentity()
        {
            return this.Id;
        }

        public static bool operator ==(EntityObject<TObject, TIdentity> entity0, EntityObject<TObject, TIdentity> entity1)
        {
            if ((object)entity0 == null && (object)entity1 == null) return true;
            if ((object)entity0 == null || (object)entity1 == null) return false;
            return entity0.Equals(entity1);
        }

        public static bool operator !=(EntityObject<TObject, TIdentity> entity0, EntityObject<TObject, TIdentity> entity1)
        {
            return !(entity0 == entity1);
        }
    }

}