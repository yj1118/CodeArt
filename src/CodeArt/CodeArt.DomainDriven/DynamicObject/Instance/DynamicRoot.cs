using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Dynamic;

using CodeArt.Util;
using CodeArt.DTO;


namespace CodeArt.DomainDriven
{
    public class DynamicRoot : DynamicEntity, IAggregateRoot
    {
        [ConstructorRepository()]
        public DynamicRoot(TypeDefine define, bool isEmpty)
            : base(define, isEmpty)
        {
            this.OnConstructed();
        }

        #region 实现内聚根接口

        private string _uniqueKey;

        public string UniqueKey
        {
            get
            {
                if (_uniqueKey == null)
                {
                    _uniqueKey = UniqueKeyCalculator.GetUniqueKey(this);
                }
                return _uniqueKey;
            }
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
            if (this.PreAdd != null)
            {
                var e = new RepositoryEventArgs(this, StatusEventType.PreAdd);
                this.PreAdd(this, e);
            }
        }

        public event RepositoryEventHandler Added;
        public virtual void OnAdded()
        {
            if (this.Added != null)
            {
                var e = new RepositoryEventArgs(this, StatusEventType.Added);
                this.Added(this, e);
            }
            CallOnceRepositoryActions();
        }

        public event RepositoryEventHandler AddPreCommit;
        public virtual void OnAddPreCommit()
        {
            if (this.AddPreCommit != null)
            {
                var e = new RepositoryEventArgs(this, StatusEventType.AddPreCommit);
                this.AddPreCommit(this, e);
            }
            CallOnceRepositoryActions();
        }


        public event RepositoryEventHandler AddCommitted;
        public virtual void OnAddCommitted()
        {
            if (this.AddCommitted != null)
            {
                var e = new RepositoryEventArgs(this, StatusEventType.AddCommitted);
                this.AddCommitted(this, e);
            }
            CallOnceRepositoryActions();
        }



        public event RepositoryEventHandler PreUpdate;
        public virtual void OnPreUpdate()
        {
            if (this.PreUpdate != null)
            {
                var e = new RepositoryEventArgs(this, StatusEventType.PreUpdate);
                this.PreUpdate(this, e);
            }
        }

        public event RepositoryEventHandler Updated;
        public virtual void OnUpdated()
        {
            if (this.Updated != null)
            {
                var e = new RepositoryEventArgs(this, StatusEventType.Updated);
                this.Updated(this, e);
            }
            CallOnceRepositoryActions();
        }

        public event RepositoryEventHandler UpdatePreCommit;
        public virtual void OnUpdatePreCommit()
        {
            if (this.UpdatePreCommit != null)
            {
                var e = new RepositoryEventArgs(this, StatusEventType.UpdatePreCommit);
                this.UpdatePreCommit(this, e);
            }
            CallOnceRepositoryActions();
        }


        public event RepositoryEventHandler UpdateCommitted;
        public virtual void OnUpdateCommitted()
        {
            if (this.UpdateCommitted != null)
            {
                var e = new RepositoryEventArgs(this, StatusEventType.UpdateCommitted);
                this.UpdateCommitted(this, e);
            }
            CallOnceRepositoryActions();
        }


        public event RepositoryEventHandler PreDelete;
        public virtual void OnPreDelete()
        {
            if (this.PreDelete != null)
            {
                var e = new RepositoryEventArgs(this, StatusEventType.PreDelete);
                this.PreDelete(this, e);
            }
        }

        public event RepositoryEventHandler Deleted;

        public virtual void OnDeleted()
        {
            if (this.Deleted != null)
            {
                var e = new RepositoryEventArgs(this, StatusEventType.Deleted);
                this.Deleted(this, e);
            }
            CallOnceRepositoryActions();
        }

        public event RepositoryEventHandler DeletePreCommit;
        public virtual void OnDeletePreCommit()
        {
            if (this.DeletePreCommit != null)
            {
                var e = new RepositoryEventArgs(this, StatusEventType.DeletePreCommit);
                this.DeletePreCommit(this, e);
            }
            CallOnceRepositoryActions();
        }

        /// <summary>
        /// 对象被真实提交到仓储删除后
        /// </summary>
        public event RepositoryEventHandler DeleteCommitted;
        public virtual void OnDeleteCommitted()
        {
            if (this.DeleteCommitted != null)
            {
                var e = new RepositoryEventArgs(this, StatusEventType.DeleteCommitted);
                this.DeleteCommitted(this, e);
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

        #endregion

        public virtual IEnumerable<(string Type, string Id)> GetObservers()
        {
            return Array.Empty<(string Type, string Id)>();
        }

        public static DynamicRoot CreateEmpty<RT>() where RT : TypeDefine
        {
            var type = TypeDefine.GetDefine<RT>();
            return new DynamicRoot(type, true);
        }

    }
}