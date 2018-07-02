using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Threading;

using CodeArt.DTO;
using CodeArt.Concurrent.Sync;

namespace CodeArt.Concurrent.Pattern
{
    /// <summary>
    /// 不重复的延迟行为，在指定的时间内重复触发行为也只会造成行为执行一次，而且执行的时机是最后一次调用Raise方法后的时间+延迟的时间
    /// </summary>
    public class NotRepeatDelayAction : IDisposable
    {
        private TimeoutMonitor _monitor;
        private int _delayMilliseconds;
        private Action _action;
        private bool _needRaise = false;

        public NotRepeatDelayAction(int delayMilliseconds, Action action)
        {
            _action = action;
            _monitor = new TimeoutMonitor(_Raise);
            _delayMilliseconds = delayMilliseconds;
        }

        public void Dispose()
        {
            _monitor.Dispose();
        }

        private void _Raise()
        {
            _needRaise = false;
            _monitor.Reset();
            _action();
        }

        /// <summary>
        /// 每次触发行为都会导致内部重新计算时间，当时间到了后行为才会被触发
        /// </summary>
        public void Raise()
        {
            _monitor.Reset();
            _monitor.Start(_delayMilliseconds);
            _needRaise = true;
        }

        /// <summary>
        /// 重置，将需要触发的事件立即触发
        /// </summary>
        public void Reset()
        {
            if (_needRaise)
            {
                _needRaise = false;
                _Raise();
            }
        }

    }
}