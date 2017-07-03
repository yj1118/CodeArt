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
    public sealed class MediaTimer : MediaTimerBase
    {
        public MediaTimerMode Mode
        {
            get;
            private set;
        }

        /// <summary>
        /// 时间间隔
        /// </summary>
        public int IntervalMilliseconds
        {
            get;
            private set;
        }

        public MediaTimer(MediaTimerMode mode,int intervalMilliseconds)
        {
            this.Mode = mode;
            this.IntervalMilliseconds = intervalMilliseconds;
        }

        public void Start()
        {
            this.Start(this.Mode, this.IntervalMilliseconds);
        }


        public EventHandler Elapsed;

        protected override void OnElapsed()
        {
            if (this.Elapsed != null)
            {
                this.Elapsed(this, EventArgs.Empty);
            }
        }
    }
}
