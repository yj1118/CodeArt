using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Text;
using System.Transactions;

using CodeArt.Runtime;
using CodeArt.AppSetting;
using CodeArt.Concurrent;
using System.Threading;

namespace CodeArt.DomainDriven
{
    public class DataContext : IDataContext, IDisposable
    {
        ///// <summary>
        ///// 打开数据上文的次数
        ///// <para>
        ///// 使用此计数,可以嵌套使用数据上下文
        ///// </para>
        ///// </summary>
        //internal int OpenTimes
        //{
        //    get;
        //    private set;
        //}

        private DataContext()
        {
            //this.OpenTimes = 0;
            //this.RequiresNew = false;
            InitializeTransaction();
            InitializeSchedule();
            InitializeRollback();
            InitializeMirror();
            InitializeBuffer();
            InitializeItems();
        }

        ///// <summary>
        ///// 是否新开独立的事务
        ///// </summary>
        //public bool RequiresNew
        //{
        //    get;
        //    internal set;
        //}

        #region 镜像

        private List<IAggregateRoot> _mirrors;
        private bool _lockedMirrors = false;


        private void InitializeMirror()
        {
            _mirrors = new List<IAggregateRoot>();
            _lockedMirrors = false;
        }

        private void DisposeMirror()
        {
            _mirrors.Clear();
            _lockedMirrors = false;
        }

        internal void AddMirror(IAggregateRoot obj)
        {
            if (IsCommiting)
                throw new DataContextException(Strings.CanNotAddMirror);
            if (_mirrors.Exists((t) => { return t.UniqueKey == obj.UniqueKey; })) return;
            _mirrors.Add(obj);
        }

        private void LockMirrors()
        {
            if (_lockedMirrors) return; //不重复锁定镜像对象
            LockManager.Lock(_mirrors);
            _lockedMirrors = true;
        }

        /// <summary>
        /// 判定对象是否为镜像（判断obj是否在_mirrors中）
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal bool IsMirror(object obj)
        {
            return _mirrors.Exists((t) => { return object.ReferenceEquals(t, obj); });
        }

        internal IEnumerable<IAggregateRoot> GetMirrorObjects()
        {
            return _mirrors;
        }

        #endregion

        #region 当前被加载的对象集合（不包括镜像对象）

        private List<IAggregateRoot> _buffer;

        private void InitializeBuffer()
        {
            _buffer = new List<IAggregateRoot>();
        }

        private void DisposeBuffer()
        {
            _buffer.Clear();
        }

        internal void AddBuffer(IAggregateRoot obj)
        {
            if (_buffer.Exists((t) => { return t.UniqueKey == obj.UniqueKey; })) return;
            _buffer.Add(obj);
        }

        /// <summary>
        /// 获取数据上下文中存放的缓冲对象
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<IAggregateRoot> GetBufferObjects()
        {
            return _buffer;
        }


        #endregion

        #region CUD

        public void RegisterAdded<T>(T item, IPersistRepository repository) where T : IAggregateRoot
        {
            ProcessAction(ScheduledAction.Borrow(item, repository, ScheduledActionType.Create));
            item.SaveState();
            item.MarkClean();//无论是延迟执行，还是立即执行，我们都需要提供统一的状态给领域层使用
        }

        public void RegisterUpdated<T>(T item, IPersistRepository repository) where T : IAggregateRoot
        {
            ProcessAction(ScheduledAction.Borrow(item, repository, ScheduledActionType.Update));
            item.SaveState();
            item.MarkClean();//无论是延迟执行，还是立即执行，我们都需要提供统一的状态给领域层使用
        }

        public void RegisterDeleted<T>(T item, IPersistRepository repository) where T : IAggregateRoot
        {
            ProcessAction(ScheduledAction.Borrow(item, repository, ScheduledActionType.Delete));
            item.SaveState();
            item.MarkDirty();//无论是延迟执行，还是立即执行，我们都需要提供统一的状态给领域层使用
        }

        #endregion

        #region 锁

        public void OpenLock(QueryLevel level)
        {
            if (IsLockQuery(level))
                OpenTimelyMode();
        }

        private bool IsLockQuery(QueryLevel level)
        {
            return level == QueryLevel.HoldSingle || level == QueryLevel.Single || level == QueryLevel.Share;
        }

        #endregion

        #region 领域对象查询服务

        /// <summary>
        /// 向数据上下文注册查询，该方法会控制锁和同步查询结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        public T RegisterQueried<T>(QueryLevel level, Func<T> persistQuery) where T : IAggregateRoot
        {
            this.OpenLock(level);
            return persistQuery();
        }

        /// <summary>
        /// 向数据上下文注册查询，该方法会控制锁和同步查询结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        public IEnumerable<T> RegisterQueried<T>(QueryLevel level, Func<IEnumerable<T>> persistQuery) where T : IAggregateRoot
        {
            this.OpenLock(level);
            return persistQuery();
        }

        /// <summary>
        /// 向数据上下文注册查询，该方法会控制锁和同步查询结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        public Page<T> RegisterQueried<T>(QueryLevel level, Func<Page<T>> persistQuery) where T : IAggregateRoot
        {
            this.OpenLock(level);
            return persistQuery();
        }

        #endregion

        #region 执行计划

        private List<ScheduledAction> _actions;

        private void InitializeSchedule()
        {
            _actions = new List<ScheduledAction>();
        }

        private void DisposeSchedule()
        {
            foreach (var action in _actions)
            {
                ScheduledAction.Return(action);
            }
            _actions.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns>如果延迟执行，返回true</returns>
        private void ProcessAction(ScheduledAction action)
        {
            if (this._transactionStatus == TransactionStatus.Delay)
            {
                _actions.Add(action);//若处于延迟模式的事务中，那么将该操作暂存
                return;
            }

            if (this._transactionStatus == TransactionStatus.Timely)
            {
                //若已经开启全局事务，直接执行
                _actions.Add(action); //直接执行也要加入到actions集合中
                ExecuteAction(action);
                return;
            }

            if (this._transactionStatus == TransactionStatus.None)
            {
                //没有开启事务，立即执行
                _conn.Begin();

                ExecuteAction(action);
                RaisePreCommit(action);

                _conn.Commit();
                RaiseCommitted(action);
                //using (ITransactionManager manager = GetTransactionManager())
                //{
                //    manager.Begin();
                //    ExecuteAction(action);
                //    //提交事务
                //    RaisePreCommit(action);
                //    manager.Commit();
                //    RaiseCommitted(action);
                //}
                return;
            }
        }

        private void RaisePreCommit(ScheduledAction action)
        {
            switch (action.Type)
            {
                case ScheduledActionType.Create:
                    action.Target.OnAddPreCommit();
                    StatusEventExecute(StatusEventType.AddPreCommit, action.Target);
                    break;
                case ScheduledActionType.Update:
                    action.Target.OnUpdatePreCommit();
                    StatusEventExecute(StatusEventType.UpdatePreCommit, action.Target);
                    break;
                case ScheduledActionType.Delete:
                    action.Target.OnDeletePreCommit();
                    StatusEventExecute(StatusEventType.DeletePreCommit, action.Target);
                    break;
            }
        }

        private void StatusEventExecute(StatusEventType type, IAggregateRoot target)
        {
            StatusEvent.Execute((target as DomainObject).ObjectType, type, target);
        }


        private void RaiseCommitted(ScheduledAction action)
        {
            switch (action.Type)
            {
                case ScheduledActionType.Create:
                    action.Target.OnAddCommitted();
                    action.Repository.OnAddCommited(action.Target);
                    StatusEventExecute(StatusEventType.AddCommitted, action.Target);
                    break;
                case ScheduledActionType.Update:
                    action.Target.OnUpdateCommitted();
                    action.Repository.OnUpdateCommited(action.Target);
                    StatusEventExecute(StatusEventType.UpdateCommitted, action.Target);
                    break;
                case ScheduledActionType.Delete:
                    action.Target.OnDeleteCommitted();
                    action.Repository.OnDeleteCommited(action.Target);
                    StatusEventExecute(StatusEventType.DeleteCommitted, action.Target);
                    break;
            }
        }

        private void RaisePreCommitQueue()
        {
            foreach (var action in _actions)
            {
                RaisePreCommit(action);
            }
        }

        private void RaiseCommittedQueue()
        {
            foreach (var action in _actions)
            {
                RaiseCommitted(action);
            }
        }

        /// <summary>
        /// 检验执行的计划
        /// </summary>
        /// <param name="action"></param>
        private void ValidateAction(ScheduledAction action)
        {
            if (action.Target.IsEmpty())
                throw new ActionTargetIsEmptyException("对象为空，不能执行持久化操作!对象类型：" + action.Target.GetType().FullName);

            if (action.Type == ScheduledActionType.Delete) return; //删除操作，不需要验证固定规则

            ValidationResult result = action.Validate();
            if (!result.IsSatisfied)
                throw new ValidationException(result);
        }

        /// <summary>
        /// 执行计划
        /// </summary>
        /// <param name="action"></param>
        private void ExecuteAction(ScheduledAction action)
        {
            if (action.Expired) return;

            action.Target.LoadState();
            this.ValidateAction(action);

            var repository = action.Repository;
            switch (action.Type)
            {
                case ScheduledActionType.Create:
                    repository.PersistAdd(action.Target);
                    action.Target.MarkClean();
                    break;
                case ScheduledActionType.Update:
                    repository.PersistUpdate(action.Target);
                    action.Target.MarkClean();
                    break;
                case ScheduledActionType.Delete:
                    repository.PersistDelete(action.Target);
                    action.Target.MarkDirty();
                    break;
            }

            action.MarkExpired();
        }

        #endregion

        #region 事务管理

        private TransactionStatus _transactionStatus;
        private int _transactionCount;
        //private ITransactionManager _transaction;
        private DataConnection _conn;

        /// <summary>
        /// 是否正在执行提交操作
        /// </summary>
        public bool IsCommiting
        {
            get;
            private set;
        }

        public DataConnection Connection
        {
            get
            {
                return _conn;
            }
        }


        private void InitializeTransaction()
        {
            _transactionStatus = TransactionStatus.None;
            _transactionCount = 0;
            //_transaction = null;
            IsCommiting = false;
            _conn = new DataConnection();
        }

        private void DisposeTransaction()
        {
            _transactionStatus = TransactionStatus.None;
            _transactionCount = 0;
            //if (_transaction != null)
            //{
            //    _transaction.Dispose();
            //    _transaction = null;
            //}
            IsCommiting = false;
            if(_conn!=null)
            {
                _conn.Dispose();
            }
        }

        public bool InTransaction
        {
            get
            {
                return _transactionStatus != TransactionStatus.None;
            }
        }

        /// <summary>
        /// 开启即时事务,并且锁定事务
        /// </summary>
        public void OpenTimelyMode()
        {
            if (_transactionStatus != TransactionStatus.Timely)
            {
                if (!this.InTransaction)
                    throw new NotBeginTransactionException(Strings.NotOpenTransaction);

                //开启即时事务
                this._transactionStatus = TransactionStatus.Timely;

                _conn.Begin();

                //_transaction = GetTransactionManager();
                //_transaction.Begin();

                if (!IsCommiting)
                {
                    //没有之前的队列要执行
                    ExecuteActionQueue();//在提交时更改了事务模式,只有可能是在验证行为时发生，该队列会在稍后立即执行，因此此处不执行队列
                }
            }
        }

        //private static ITransactionManager GetTransactionManager()
        //{
        //    var factory = TransactionManagerFactory.CreateFactory();
        //    return factory.CreateManager();
        //}

        /// <summary>
        /// 开启事务BeginTransaction和提交事务Commit必须成对出现
        /// </summary>
        public void BeginTransaction()
        {
            if (this.InTransaction)
            {
                _transactionCount++;
            }
            else
            {
                _transactionStatus = TransactionStatus.Delay;
                _actions.Clear();
                _transactionCount++;
                _conn.Initialize();
            }
        }

        public void Commit()
        {
            if (!this.InTransaction)
                throw new NotBeginTransactionException(Strings.NotOpenTransaction);
            else
            {
                _transactionCount--;
                if (_transactionCount == 0)
                {
                    if (IsCommiting)
                        throw new RepeatedCommitException(Strings.TransactionRepeatedCommit);

                    //if (IsCommiting)
                    //{
                    //    已在提交阶段，此处不用提交，留待上一个提交
                    //    代码能进入这里，说明在提交阶段，又有新的提交加入，那么这个新的提交就不用提交了，而是留给主线去提交
                    //    return;
                    //}

                    IsCommiting = true;

                    try
                    {
                        if (_transactionStatus == TransactionStatus.Delay)
                        {
                            _transactionStatus = TransactionStatus.Timely; //开启即时事务

                            _conn.Begin();
                            ExecuteActionQueue();
                            RaisePreCommitQueue();
                            _conn.Commit();

                            RaiseCommittedQueue();
                            //using (ITransactionManager manager = GetTransactionManager())
                            //{
                            //    manager.Begin();
                            //    ExecuteActionQueue();
                            //    RaisePreCommitQueue();

                            //    manager.Commit();

                            //    RaiseCommittedQueue();
                            //}
                        }
                        else if (_transactionStatus == TransactionStatus.Timely)
                        {
                            ExecuteActionQueue();
                            RaisePreCommitQueue();

                            _conn.Commit();

                            RaiseCommittedQueue();
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        Clear();
                        IsCommiting = false;
                    }
                }
            }
        }

        private void ExecuteActionQueue()
        {
            //执行行为队列之前，我们会对镜像进行锁定
            LockMirrors();
            foreach (ScheduledAction action in _actions)
            {
                this.ExecuteAction(action);
            }
        }

        public bool IsDirty
        {
            get
            {
                return _actions.Count > 0;
            }
        }

        private int _inBuildObjectCount = 0;

        /// <summary>
        /// 指示是否通过数据上下文在构建对象，这意味着是从数据仓储中创建并加载对象属性
        /// </summary>
        public bool InBuildObject
        {
            get
            {
                return _inBuildObjectCount > 0;
            }
            set
            {
                if (value)
                    Interlocked.Increment(ref _inBuildObjectCount);
                else
                    Interlocked.Decrement(ref _inBuildObjectCount);
            }
        }


        /// <summary>
        /// 清理资源
        /// </summary>
        internal void Clear()
        {
            //this.RequiresNew = false;
            _inBuildObjectCount = 0;
            DisposeSchedule();
            DisposeRollback();
            DisposeTransaction();
            DisposeMirror();
            DisposeBuffer();
            DisposeItems();
        }

        public void Dispose()
        {
            if (this.InTransaction)
            {
                this.Rollback();
            }
            else
            {
                Clear();
            }
        }

        #endregion

        #region 回滚

        private RollbackCollection _rollbacks;

        private void InitializeRollback()
        {
            _rollbacks = new RollbackCollection();
        }

        private void DisposeRollback()
        {
            _rollbacks.Clear();
        }

        public void Rollback()
        {
            if (!this.InTransaction)
                throw new NotBeginTransactionException(Strings.NotOpenTransaction);
            else
            {
                try
                {
                    _rollbacks.Execute(this);
                    RaiseRolledBack(this);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    Clear();
                }
            }
        }

        public void RegisterRollback(RepositoryRollbackEventArgs e)
        {
            _rollbacks.Add(e);
        }

        public static event RolledBackEventHandler RolledBack;

        private static void RaiseRolledBack(DataContext context)
        {
            if (RolledBack != null)
            {
                RolledBack(context, new RolledBackEventArgs(context));
            }
        }


        #endregion

        #region 额外项

        private Dictionary<string, object> _items = null;

        private void InitializeItems()
        {

        }

        public void SetItem(string name, object item)
        {
            if (_items == null) _items = new Dictionary<string, object>();
            _items[name] = item;
        }

        public object GetItem(string name)
        {
            if (_items == null) return null;
            if(_items.TryGetValue(name,out var item))
            {
                return item;
            }
            return null;
        }

        public bool HasItem(string name)
        {
            if (_items == null) return false;
            return _items.ContainsKey(name);
        }

        private void DisposeItems()
        {
            if (_items != null)
            {
                _items.Clear();
                _items = null;
            }
        }

        #endregion

        #region 基于当前应用程序会话的数据上下文


        private const string _sessionKey = "DataContext.Current";

        /// <summary>
        /// 获取或设置当前会话的数据上下文
        /// </summary>
        public static DataContext Current
        {
            get
            {
                var dataContext = AppSession.GetItem<DataContext>(_sessionKey);
                if (dataContext == null)
                    throw new InvalidOperationException("DataContext.Current为null,无法使用仓储对象");
                return dataContext;

                //var dataContext = AppSession.GetOrAddItem<DataContext>(
                //    _sessionKey,
                //    () =>
                //    {
                //        return Symbiosis.TryMark<DataContext>(_pool, () => { return new DataContext(); });
                //    });
                //if (dataContext == null) throw new InvalidOperationException("DataContext.Current为null,无法使用仓储对象");
                //return dataContext;
            }
            internal set
            {
                AppSession.SetItem<DataContext>(_sessionKey, value);
            }
        }

        public static bool ExistCurrent()
        {
            return AppSession.GetItem<DataContext>(_sessionKey) != null;
        }


        #endregion

        #region 对象池

        private static Pool<DataContext> _pool;


        static DataContext()
        {
            _pool = new Pool<DataContext>(() =>
            {
                return new DataContext();
            }, (ctx, phase) =>
            {
                if (phase == PoolItemPhase.Returning)
                {
                    ctx.Clear();
                }
                return true;
            }, new PoolConfig()
            {
                MaxRemainTime = 300 //5分钟之内未被使用，就移除
            });
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 使用事务，在<paramref name="action"/>执行之前会开启一个新的事务并在<paramref name="action"/>执行完毕后结束事务
        /// </summary>
        /// <param name="action"></param>
        public static void UseTransactionScope(Action action)
        {
            TransactionOptions option = new TransactionOptions();
            option.IsolationLevel = IsolationLevel.ReadUncommitted;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
            {
                //注意，这里用DataContext.NewScope修复了一个BUG
                //当使用UseTransactionScope时，有可能已经有DataContext.Current了
                //这时候的调用，DataContext必须也新建一个上下文，否则独立事务就无效了
                DataContext.NewScope(() =>
                {
                    action();
                });
                scope.Complete();
            }
        }

        #endregion

        private static void Using(DataContext dataContext, Action<DataConnection> action, bool timely)
        {
            try
            {
                bool isCommiting = dataContext.IsCommiting;
                if (isCommiting)
                {
                    //事务上下文已进入提交阶段，那么不必重复开启事务
                    action(dataContext.Connection);
                }
                else
                {
                    dataContext.BeginTransaction();
                    if (timely) dataContext.OpenTimelyMode();

                    action(dataContext.Connection);
                    dataContext.Commit();
                }
            }
            catch (Exception)
            {
                if (dataContext.InTransaction)
                {
                    dataContext.Rollback();
                }
                throw;
            }
        }

        public static void Using(Action action, bool timely = false)
        {
            Using((conn) =>
            {
                action();
            }, timely);
        }

        public static void Using(Action<DataConnection> action, bool timely = false)
        {
            if (DataContext.ExistCurrent())
            {
                var dataContext = DataContext.Current;
                Using(dataContext, action, timely);
            }
            else
            {
                using (var temp = _pool.Borrow())
                {
                    var dataContext = temp.Item;
                    DataContext.Current = dataContext;
                    try
                    {
                        Using(dataContext, action, timely);
                    }
                    catch(Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        DataContext.Current = null; //执行完后，释放
                    }
                }
            }
        }

        public static void NewScope(Action action)
        {
            NewScope((conn) =>
            {
                action();
            });
        }

        /// <summary>
        /// 新建一个范围数据上下文，该数据上下文会创建一个新的独立事务
        /// </summary>
        /// <param name="action"></param>
        /// <param name="timely"></param>
        public static void NewScope(Action<DataConnection> action)
        {
            DataContext prev = null;
            if (DataContext.ExistCurrent())
            {
                prev = DataContext.Current; //保留当前的数据上下文对象
            }

            try
            {
                using (var temp = _pool.Borrow())
                {
                    var dataContext = temp.Item;
                    DataContext.Current = dataContext;
                    Using(dataContext, action, true);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (prev != null)
                {
                    DataContext.Current = prev; //还原当前数据上下文
                }
                else
                {
                    DataContext.Current = null;
                }
            }
        }
    }



    /// <summary>
    /// 事务状态
    /// </summary>
    internal enum TransactionStatus : byte
    {
        /// <summary>
        /// 不开启事务，性能高
        /// </summary>
        None = 1,
        /// <summary>
        /// 延迟事务，只为关键的任务（CUD）开启事务并且这些任务都在最后集中执行
        /// 此模式不能将非持久层的查询操作事务化，只能保证CUD任务是事务性的
        /// </summary>
        Delay = 2,
        /// <summary>
        /// 即时事务，可以保证所有任务在事务范围内，性能最差，但是可以保证应用层的所有任务都在事务之内
        /// </summary>
        Timely = 3
    }


}
