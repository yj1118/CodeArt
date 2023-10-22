using System;
using System.Threading;

namespace CodeArt.Diagnostics
{
    /// <summary>
    /// <para>并发探测器</para>
    /// <para>探测在指定的时间内，是否有并发访问</para>
    /// </summary>
    public sealed class ConcurrentDetector : IDisposable
    {
        private int _timeRange;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeRange">探测的时间区间，单位毫秒</param>
        public ConcurrentDetector(int timeRange = 20)
        {
            _timeRange = timeRange;
        }


        private bool _isHappened = false;
        /// <summary>
        /// 是否发生了并发访问
        /// </summary>
        public bool IsHappened
        {
            get { return _isHappened; }
        }

        private object _syncObject = new object();

        public void Access()
        {
            if (_isHappened) return;
            bool isLocked = IsLocked();
            lock (_syncObject)
            {
                if (_isHappened) return;
                _isHappened = isLocked;
            }
        }

        private long _startLockTimeTicks = 0;

        private bool IsLocked()
        {
            if (Monitor.TryEnter(_syncObject))
            {
                _startLockTimeTicks = DateTime.Now.Ticks;
                Thread.Sleep(_timeRange);
                Monitor.Exit(_syncObject);
                return false;
            }
            return true;
        }

        public void Reset()
        {
            WaitReleaseLock();
            _startLockTimeTicks = 0;
            _isHappened = false;
        }


        #region 等待释放锁

        /// <summary>
        /// 等待锁
        /// </summary>
        private void WaitReleaseLock()
        {
            Thread.MemoryBarrier();
            if (_startLockTimeTicks == 0) return;
            int remainTime = GetRemainTime();
            if(remainTime > 0) Thread.Sleep(remainTime);//等待锁释放
        }

        private int GetRemainTime()
        {
            long lockedTimeTicks = DateTime.Now.Ticks - _startLockTimeTicks;
            TimeSpan lockedTime = new TimeSpan(lockedTimeTicks);
            return unchecked(_timeRange - (int)lockedTime.TotalMilliseconds) + 1;//修正1毫秒，避免等待时间不足
        }

        #endregion


        public void Dispose()
        {
            Reset();
        }

    }
}
