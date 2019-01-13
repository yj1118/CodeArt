using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using CodeArt.Concurrent;
using CodeArt.Concurrent.Sync;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 超时管理
    /// </summary>
    internal static class TimeoutManager
    {
        private static ConcurrentDictionary<Guid, TimeoutMonitor> _monitors = new ConcurrentDictionary<Guid, TimeoutMonitor>();
        private static PoolWrapper<TimeoutMonitor> _pool = new PoolWrapper<TimeoutMonitor>(() =>
        {
            return new TimeoutMonitor(TimeoutProcess);
        }, (monitor, phase) =>
        {
            if (phase == PoolItemPhase.Returning)
            {
                monitor.Reset();
            }
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 300 //闲置时间300秒
        });

        private static TimeoutMonitor CreateMonitor(Guid eventId)
        {
            TimeoutMonitor monitor = _pool.Borrow();
            _monitors.TryAdd(eventId, monitor);
            return monitor;
        }


        private static void TimeoutProcess(TimeoutMonitor monitor)
        {
            var state = monitor.State;
            var key = (EventKey)state;
            EventListener.Timeout(key);//处理超时
        }

        /// <summary>
        /// 开始事件<paramref name="eventId"/>的等待
        /// </summary>
        /// <param name="eventId"></param>
        public static void Start(EventKey key)
        {
            var monitor = CreateMonitor(key.EventId);
            monitor.State = key;
            monitor.Start(30000); //等待30秒
        }

        /// <summary>
        /// 结束等待
        /// </summary>
        /// <param name="eventId"></param>
        public static bool End(EventKey key)
        {
            if (_monitors.TryRemove(key.EventId, out var monitor))
            {
                monitor.TryComplete();
                _pool.Return(monitor);
                return true;
            }
            return false;
        }

    }
}
