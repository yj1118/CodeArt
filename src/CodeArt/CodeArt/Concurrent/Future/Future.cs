using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace CodeArt.Concurrent
{
    /// <summary>
    /// 封装了在未来某个时间将要完成的事情
    /// </summary>
    public abstract class Future : IFuture
    {
        protected readonly object _syncRoot = new object();

        /// <summary>
        /// 使用volatile关键字优化器在用到这个变量时必须每次都小心地重新读取这个变量的值，而不是使用保存在寄存器里的备份
        /// </summary>
        protected volatile FutureStatus _state;
        protected Exception _error;
        protected Action _onComplete;
        protected int _waiterCount;

        public Future()
        {
            this.Reset();
        }

        /// <summary>
        /// 等待future完成
        /// </summary>
        public void Wait()
        {
            if (this.IsComplete) return;

            //如果释放了锁并且其他线程处于该对象的就绪队列中，则其中一个线程将获取该锁。 
            //如果其他线程处于等待队列中等待获取锁，则它们不会在锁的所有者调用 Exit 时自动移动到就绪队列中。 
            //若要将一个或多个等待线程移动到就绪队列中，请在调用 Exit 之前调用 Pulse 或 PulseAll
            lock (_syncRoot)
            {
                while (!this.IsComplete)
                {
                    ++_waiterCount;
                    try
                    {
                        //释放当前线程对_syncRoot的锁，流放当前线程到等待队列,消费者的线程此时会被阻塞
                        Monitor.Wait(_syncRoot);
                    }
                    catch (ThreadAbortException ex)
                    {
                        int availableWorkers, availableIocp;
                        ThreadPool.GetAvailableThreads(out availableWorkers, out availableIocp);
                        int maxWorkers, maxIocp;
                        ThreadPool.GetMaxThreads(out maxWorkers, out maxIocp);
                        int minWorkers, minIocp;
                        ThreadPool.GetMinThreads(out minWorkers, out minIocp);
                        var builder = new StringBuilder();
                        builder.AppendLine("ThreadAbortException In Future.Wait()");
                        builder.AppendLine("AvailableWorkers = " + availableWorkers);
                        builder.AppendLine("AvailableIocp = " + availableIocp);
                        builder.AppendLine("MaxWorkers = " + maxWorkers);
                        builder.AppendLine("MaxIocp = " + maxIocp);
                        builder.AppendLine("MinWorkers = " + minWorkers);
                        builder.AppendLine("MinIocp = " + minIocp);
                        throw ex;
                    }
                    finally
                    {
                        --_waiterCount;
                    }
                }
            }
        }

        /// <summary>
        /// 获取当前状态，该属性不会阻塞线程
        /// </summary>
        public FutureStatus Status
        {
            get { return _state; }
        }

        /// <summary>
        /// 获取一个值，指示future是否已经完成
        /// </summary>
        /// <value>
        /// 	<see langword="true"/> 实例完成; 否则, <see langword="false"/>.
        /// </value>
        public bool IsComplete
        {
            get { return _state != FutureStatus.Incomplete; }
        }

        /// <summary>
        /// 获取一个值，指示实例是否已被取消
        /// </summary>
        /// <value>
        /// 	<see langword="true"/> 已被消费者取消; 否则, <see langword="false"/>.
        /// </value>
        public bool IsCanceled
        {
            get { return _state == FutureStatus.Canceled; }
        }

        /// <summary>
        /// 获取一个值，指示实例是否发生错误。如果实例没有完成，那么线程将阻塞。
        /// </summary>
        /// <value>
        /// 	<see langword="true"/> 实例发生错误; 否则, <see langword="false"/>.
        /// </value>
        public bool HasError
        {
            get
            {
                Wait();
                return _state == FutureStatus.Failure;
            }
        }

        /// <summary>
        /// 获取一个值，指示实例是否已经有结果。如果实例没有完成，那么线程将阻塞。
        /// </summary>
        /// <value>
        /// 	<see langword="true"/> 实例有结果; 否则, <see langword="false"/>.
        /// </value>
        public bool HasResult
        {
            get
            {
                Wait();
                return _state == FutureStatus.Success;
            }
        }

        protected Exception GetErrorForRethrow()
        {
            Thread.MemoryBarrier();
            return _error;
        }

        /// <summary>
        /// 获取实例的错误。如果实例没有完成，那么线程将阻塞。
        /// </summary>
        public Exception Error
        {
            get
            {
                Wait();
                return HasError ? _error : null;
            }
        }

        /// <summary>
        /// 指定发生错误时，需要触发的动作.
        /// </summary>
        public void OnError(Action<Exception> onError)
        {
            OnComplete(() => { if (HasError) onError(Error); });
        }

        void IFuture.OnComplete(Action onComplete)
        {
            OnComplete(onComplete);
        }

        /// <summary>
        /// 指定完成时，需要触发的动作.
        /// 此操作是线程安全的
        /// ，即：要么直接执行onComplete，要么记录onComplete，在完成时执行onComplete
        /// </summary>
        public void OnComplete(Action onComplete)
        {
            if (!this.IsComplete)
            {
                lock (_syncRoot)
                {
                    if (!this.IsComplete)
                    {
                        //如果没有完成，则将onComplete行为保留，在完成的时候触发
                        //以下操作模拟了_onComplete += onComplete
                        //这比直接使用 _onComplete += onComplete 会快一点.
                        if (_onComplete == null)
                            _onComplete = onComplete;
                        else
                        {
                            var a = _onComplete;
                            var b = onComplete;
                            _onComplete = () => { try { a(); } finally { b(); } };
                        }
                        onComplete = null;//没有完成，不执行，只记录，此处为null，后面就不会执行了
                    }
                }
            }
            //如果已经完成了，则直接触发onComplete行为
            if (onComplete != null)
            {
                onComplete();
            }
        }

        /// <summary>
        /// 指定成功完成时，需要触发的动作.
        /// </summary>
        public void OnSuccess(Action onSuccess)
        {
            OnComplete(() => { if (Status == FutureStatus.Success) onSuccess(); });
        }

        /// <summary>
        /// 指定取消时，需要触发的动作.
        /// </summary>
        public void OnCancel(Action onCancel)
        {
            OnComplete(() => { if (this.IsCanceled) onCancel(); });
        }

        protected abstract bool CompleteFromInherit(FutureStatus state, Exception error);

        /// <summary>
        /// 尝试取消. 返回 <see langword="true"/> 表示成功， <see langword="false"/> 表示取消失败
        /// </summary>
        public bool Cancel()
        {
            return CompleteFromInherit(FutureStatus.Canceled, null);
        }

        /// <summary>
        /// 开始工作
        /// </summary>
        public void Start()
        {
            if (this.IsStarted) return;
            _Start();
        }

        protected virtual  void _Start()
        {
            if (!this.IsComplete) this.Cancel();
            _state = FutureStatus.Incomplete;
            _error = null;
            _onComplete = null;
            _waiterCount = 0;
        }


        public bool IsStarted
        {
            get
            {
                return _state == FutureStatus.Incomplete;
            }
        }

        public virtual void Reset()
        {
            if (!this.IsComplete) this.Cancel();
            _state = FutureStatus.Initial;
            _error = null;
            _onComplete = null;
            _waiterCount = 0;
        }

    }

    /// <summary>
    /// 封装了将在未来某些时刻会被计算的值
    /// </summary>
    public class Future<T> : Future, IFuture<T>
    {
        private T _result;

        /// <summary>
        /// 初始化一个新的 <see cref="Future{T}"/> 实例
        /// </summary>
        public Future() { }

        /// <summary>
        /// 获取结果。如果实例没有完成计算，那么将阻塞线程。
        /// </summary>
        public T Result
        {
            get
            {
                Wait();
                if (HasResult) return _result;
                if (HasError) throw GetErrorForRethrow();
                throw new InvalidOperationException(string.Format(Strings.FutureResultNotValid, typeof(T).Name));
            }
        }

        protected override bool CompleteFromInherit(FutureStatus state, Exception error)
        {
            return Complete(state, default(T), error);
        }

        protected bool Complete(FutureStatus state, T value, Exception error)
        {
            if (state == FutureStatus.Incomplete)
                throw new ArgumentException(Strings.FutureCompleteError, "state");

            if (state == FutureStatus.Failure && error == null)
                error = new Exception(Strings.UnknownErrorOccurred);

            Action onComplete = null;
            lock (_syncRoot)
            {
                if (IsComplete) return false;

                _error = error;
                _result = value;

                if (state != FutureStatus.Canceled)
                {
                    //不是取消，就触发事件，否则，在取消的情况下，不触发事件
                    onComplete = _onComplete;
                    _onComplete = null;
                }

                //注意，由于并发原因，必须设置其他状态变量
                _state = state;

                if (_waiterCount > 0)
                {
                    //全部等待线程被移动到就绪队列中。 在调用 PulseAll 的线程释放锁后，就绪队列中的下一个线程将获取该锁。
                    Monitor.PulseAll(_syncRoot);
                }
            }
            if (onComplete != null) onComplete();
            return true;
        }

        public bool SetResult(T value)
        {
            return Complete(FutureStatus.Success, value, null);
        }

        public bool SetError(Exception error)
        {
            return Complete(FutureStatus.Failure, default(T), error);
        }

        /// <summary>
        /// 开始工作
        /// </summary>
        protected override void _Start()
        {
            base._Start();
            _result = default(T);
        }

        public override void Reset()
        {
            base.Reset();
            _result = default(T);
        }


        public static implicit operator T(Future<T> future)
        {
            return future.Result;
        }

        public static readonly Future<T> Empty = new FutureEmpty<T>();

    }


    public class FutureEmpty<T> : Future<T>
    {
        /// <summary>
        /// 初始化一个新的 <see cref="Future{T}"/> 实例
        /// </summary>
        public FutureEmpty() { }

        /// <summary>
        /// 获取结果。如果实例没有完成计算，那么将阻塞线程。
        /// </summary>
        public new T Result
        {
            get
            {
                return default(T);
            }
        }

        protected override bool CompleteFromInherit(FutureStatus state, Exception error)
        {
            return Complete(state, default(T), error);
        }


        public new bool SetResult(T value)
        {
            return false;
        }

        public new bool SetError(Exception error)
        {
            return false;
        }

        public new bool HasError
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 开始工作
        /// </summary>
        protected override void _Start()
        {
        }

        public override void Reset()
        {
        }

        public new void Wait()
        {

        }


        public static implicit operator T(FutureEmpty<T> future)
        {
            return future.Result;
        }

    }

}
