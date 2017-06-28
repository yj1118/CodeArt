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
    /// 媒体延迟器
    /// </summary>
    public sealed class MediaDelayer : MediaTimerBase
    {
        private AutoResetEvent _signal;

        public MediaDelayer()
        {
            _signal = new AutoResetEvent(false);
        }

        public void Sleep(int intervalMilliseconds)
        {
            if (intervalMilliseconds <= 0) return;
            this.Start(MediaTimerMode.OneShot, intervalMilliseconds);
            _signal.WaitOne();
        }

        protected override void OnElapsed()
        {
            _signal.Set();
        }

        protected override void Dispose(bool disposed)
        {
            base.Dispose(disposed);
            _signal.Dispose();
        }

    }
}
