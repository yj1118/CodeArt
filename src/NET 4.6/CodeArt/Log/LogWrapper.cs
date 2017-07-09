using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NLog;
using CodeArt.Util;
using CodeArt.Concurrent;

namespace CodeArt.Log
{
    /// <summary>
    /// 该对象是线程安全的
    /// </summary>
    public class LogWrapper
    {
        Pool<Logger> _pool = null;

        public LogWrapper(Func<Logger> createLogger)
        {
            _pool = new Pool<Logger>(() =>
            {
                return createLogger();
            }, (log, phase) =>
            {
                return true;
            }, new PoolConfig()
            {
                LoanCapacity = 20
            });
        }

        /// <summary>
        /// 写入致命错误的消息
        /// </summary>
        /// <param name="message"></param>
        public void Fatal(string message)
        {
            using(var item = _pool.Borrow())
            {
                var log = item.Item;
                log.Fatal(message);
            }
        }

        public void Fatal(Exception ex)
        {
            if (NonLogAttribute.IsDefined(ex)) return;
            var tracker = ex as ITracker;

            if (tracker == null)
                Fatal(string.Format("消息：{0}；堆栈：{1}", ex.GetCompleteMessage(), ex.GetCompleteStackTrace()));
            else
                Fatal(string.Format("消息：{0}；堆栈：{1}", tracker.GetMessage(), ex.GetCompleteStackTrace()));
        }

        /// <summary>
        /// 写入调试的消息
        /// </summary>
        /// <param name="message"></param>
        public void Debug(string message)
        {
            using (var item = _pool.Borrow())
            {
                var log = item.Item;
                log.Debug(message);
            }
        }

        /// <summary>
        /// 写入跟踪的消息
        /// </summary>
        /// <param name="message"></param>
        public void Trace(string message)
        {
            using (var item = _pool.Borrow())
            {
                var log = item.Item;
                log.Trace(message);
            }
        }

        public void Info(string message)
        {
            using (var item = _pool.Borrow())
            {
                var log = item.Item;
                log.Info(message);
            }
        }

        public void Write(LogEventInfo ei)
        {
            using (var item = _pool.Borrow())
            {
                var log = item.Item;
                log.Log(ei);
            }
        }

        #region 静态成员

        public static readonly LogWrapper Default = new LogWrapper(() =>
        {
            return LogManager.GetCurrentClassLogger();
        });

        #endregion
    }
}