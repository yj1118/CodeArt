using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

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
            :this((getState)=> { timeoutCallback(); })
        {
        }

        public TimeoutMonitor(Action<TimeoutMonitor> timeoutCallback)
        {
            _timer = new Timer();
            _timer.Elapsed += (s,e)=>
            {
                timeoutCallback(this);
            };
            _timer.AutoReset = false;//设置是执行一次（false）还是一直执行(true)
            _timer.Enabled = false;//是否执行System.Timers.Timer.Elapsed事件
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
                StartTimer(timeout);
            }
        }

        ///// <summary>
        ///// 标示永不超时
        ///// </summary>
        //public void None()
        //{
        //    Start(Timeout.Infinite);
        //}

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
                StopTimer();
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
                StopTimer();
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
                StopTimer();
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

        private void StartTimer(int timeout)
        {
            if (_isDisposed) return;
            _timer.Interval = timeout;
            _timer.Start();
        }

        private void StopTimer()
        {
            if (_isDisposed) return;
            _timer.Stop();
        }
    }
}
