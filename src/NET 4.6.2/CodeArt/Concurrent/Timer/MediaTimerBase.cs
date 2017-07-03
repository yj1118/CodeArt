using System;
using System.Linq;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using System.Threading;
using CodeArt.Log;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;


namespace CodeArt.Concurrent
{
    /// <summary>
    /// 媒体定时器，提供高精度的定时器效果
    /// </summary>
    public abstract class MediaTimerBase : IDisposable
    {
        private int _timerId;
        private bool _isRunning;

        private TimerCallback _oneShotCallback;
        private TimerCallback _periodicCallback;


        public MediaTimerBase()
        {
            _timerId = 0;
            _isRunning = false;
            //一定要new TimerCallback(OneShotCallback),而不能直接传递OneShotCallback，否则非托管程序会调用一垃圾回收了的委托
            _oneShotCallback = new TimerCallback(OneShotCallback);
            _periodicCallback = new TimerCallback(PeriodicCallback);
        }

        private void CheckUp(int intervalMilliseconds)
        {
            if ((intervalMilliseconds < _caps.PeriodMin) || (intervalMilliseconds > _caps.PeriodMax))
            {
                throw new Exception(Strings.InvalidPeriod);
            }
        }

        protected void Start(MediaTimerMode mode, int intervalMilliseconds)
        {
            CheckUp(intervalMilliseconds);
            if (mode == MediaTimerMode.OneShot)
            {
                _timerId = timeSetEvent(intervalMilliseconds, _caps.PeriodMin, _oneShotCallback, 0, (int)mode);
            }
            else
            {
                _timerId = timeSetEvent(intervalMilliseconds, _caps.PeriodMin, _periodicCallback, 0, (int)mode);
            }
            if (_timerId == 0) throw new Exception(Strings.UnableStartMediaTimer);
        }

        private void OneShotCallback(int id, int msg, int user, int param1, int param2)
        {
            this.Stop();
            OnElapsed();
        }

        private void PeriodicCallback(int id, int msg, int user, int param1, int param2)
        {
            OnElapsed();
        }

        protected abstract void OnElapsed();


        public void Stop()
        {
            if (this._isRunning)
            {
                _isRunning = false;
                timeKillEvent(_timerId);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MediaTimerBase()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposed)
        {
            timeKillEvent(_timerId);
        }


        #region 静态成员

        private static TimerCaps _caps;

        static MediaTimerBase()
        {
            timeGetDevCaps(ref _caps, Marshal.SizeOf(_caps));
        }

        /// <summary>
        /// 获取操作系统支持的定时器的周期分辨率（最小间隔时间和最大间隔时间）
        /// </summary>
        /// <param name="caps"></param>
        /// <param name="capsSize"></param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        private static extern int timeGetDevCaps(ref TimerCaps caps, int capsSize);


        [DllImport("winmm.dll")]
        private static extern int timeKillEvent(int id);

        [DllImport("winmm.dll")]
        private static extern int timeSetEvent(int delay, int resolution, TimerCallback callback, int user, int mode);

        private delegate void TimerCallback(int id, int msg, int user, int param1, int param2);

        #endregion


        /// <summary>
        /// 操作系统支持的定时器的周期分辨率（最小间隔时间和最大间隔时间）
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct TimerCaps
        {
            public int PeriodMin;
            public int PeriodMax;
        }
    }
}
