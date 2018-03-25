using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CodeArt.Concurrent.Sync
{
    /// <summary>
    /// 超时监视器
    /// </summary>
    public class TimeoutMonitor : IDisposable
    {
        private Timer _timer;
        private bool _isComplete;
        private bool _isCancel;
        private bool _isDisposed;
        private object _syncObject = new object();

        public object State
        {
            get;
            set;
        }

        public TimeoutMonitor(Action timeoutCallback)
            :this((state)=>{ timeoutCallback(); },null)
        {
        }

        public TimeoutMonitor(Action<object> timeoutCallback, object state)
        {
            State = state;
            _timer = new Timer(new TimerCallback((s) =>
            {
                timeoutCallback(State);
            }));
            _isDisposed = false;
        }

        /// <summary>
        /// 开始监视超时
        /// </summary>
        /// <param name="timeout">单位：毫秒</param>
        public void Start(int timeout)
        {
            lock (_syncObject)
            {
                _isComplete = false;
                _isCancel = false;
                ChangeTimer(timeout, Timeout.Infinite);
            }
        }

        /// <summary>
        /// 标示永不超时
        /// </summary>
        public void None()
        {
            Start(Timeout.Infinite);
        }

        /// <summary>
        /// 尝试执行取消操作，如果操作已完成或已取消，那么返回false
        /// </summary>
        public bool TryCancel()
        {
            lock (_syncObject)
            {
                if (_isCancel || _isComplete) return false;
                _isCancel = true;
                _isComplete = false;
                this.State = null;
                ChangeTimer(Timeout.Infinite, Timeout.Infinite);
                return true;
            }
        }

        /// <summary>
        /// 尝试执行完成操作，如果操作已完成或已取消，那么返回false
        /// </summary>
        /// <returns></returns>
        public bool TryComplete()
        {
            lock (_syncObject)
            {
                if (_isCancel || _isComplete) return false;
                _isComplete = true;
                this.State = null;
                ChangeTimer(Timeout.Infinite, Timeout.Infinite);
                return true;
            }
        }

        /// <summary>
        /// 重置监视器
        /// </summary>
        public void Reset()
        {
            lock (_syncObject)
            {
                _isComplete = false;
                _isCancel = false;
                this.State = null;
                ChangeTimer(Timeout.Infinite, Timeout.Infinite);
            }
        }

        public void Dispose()
        {
            lock (_syncObject)
            {
                if (_isDisposed) return;
                _timer.Dispose();
                _isDisposed = true;
            }
        }

        private void ChangeTimer(int dueTime, int period)
        {
            if (_isDisposed) return;
            _timer.Change(dueTime, period);
        }


    }
}
