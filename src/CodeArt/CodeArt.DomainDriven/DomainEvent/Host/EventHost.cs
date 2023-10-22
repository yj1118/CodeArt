using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

using CodeArt.AppSetting;
using CodeArt.EasyMQ.Event;
using CodeArt.DTO;
using CodeArt.Util;
using CodeArt.Log;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 事件宿主，接收外界对事件的调度请求
    /// </summary>
    public static class EventHost
    {
        #region 驻留和取消事件

        /// <summary>
        /// 订阅触发
        /// </summary>
        /// <param name="tip"></param>
        private static void SubscribeRaise(EventAttribute tip)
        {
            if (!tip.RequierdOpen) return;

            var raiseName = EventUtil.GetRaise(tip.Name);
            //作为事件的提供方，我们订阅了触发事件，这样当外界发布了“触发事件”后，这里就可以收到消息并且执行事件
            EventPortal.Subscribe(raiseName, RaiseEventHandler.Instance);
        }

        /// <summary>
        /// 订阅回逆
        /// </summary>
        /// <param name="tip"></param>
        private static void SubscribeReverse(EventAttribute tip)
        {
            if (!tip.RequierdOpen) return;

            var reverseName = EventUtil.GetReverse(tip.Name);
            EventPortal.Subscribe(reverseName, ReverseEventHandler.Instance);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="tip"></param>
        private static void CancelRaise(EventAttribute tip)
        {
            if (!tip.RequierdOpen) return;

            var raiseName = EventUtil.GetRaise(tip.Name);
            EventPortal.Cancel(raiseName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tip"></param>
        private static void CanceleReverse(EventAttribute tip)
        {
            if (!tip.RequierdOpen) return;

            var reverseName = EventUtil.GetReverse(tip.Name);
            EventPortal.Cancel(reverseName);
        }

        #endregion

        #region 初始化和释放

        internal static void Initialize()
        {
            EventAttribute.Initialize();

            //领域事件初始化
            DomainEvent.Initialize();

            //订阅事件
            SubscribeEvents();
        }


        internal static void Cleanup()
        {
            //取消订阅
            CancelEvents();
            ClearTimer();
        }

        #region 订阅/取消订阅事件

        private static void SubscribeEvents()
        {
            var tips = EventAttribute.Tips;
            foreach (var tip in tips)
            {
                SubscribeRaise(tip);
                SubscribeReverse(tip);
            }
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        private static void CancelEvents()
        {
            var tips = EventAttribute.Tips;
            foreach (var tip in tips)
            {
                CancelRaise(tip);
                CanceleReverse(tip);
            }
        }


        #endregion



        #endregion

        #region 初始化之后

        internal static void Initialized()
        {
            EventProtector.RestoreAsync();
            InitTimer();
        }

        #endregion


        #region 事件的启用和禁用

        /// <summary>
        /// 开启事件，有些事件不是自动启动的，所以需要手工开启
        /// </summary>
        /// <param name="eventNames"></param>
        public static void EnableEvent(params string[] eventNames)
        {
            var tips = EventAttribute.Tips;
            foreach(var eventName in eventNames)
            {
                var tip = EventAttribute.GetTip(eventName, false);
                if (tip != null) tip.IsEnabled = true;
            }
        }

        /// <summary>
        /// 禁用事件
        /// </summary>
        /// <param name="eventNames"></param>
        public static void DisabledEvent(params string[] eventNames)
        {
            var tips = EventAttribute.Tips;
            foreach (var eventName in eventNames)
            {
                var tip = EventAttribute.GetTip(eventName, false);
                if (tip != null) tip.IsEnabled = false;
            }
        }

        #endregion

        #region 定时清理

        private static Timer _timer;

        private static void InitTimer()
        {
            _timer = new Timer(24 * 3600 * 1000); //每间隔24小时执行一次
            _timer.Elapsed += OnElapsed;
            _timer.AutoReset = false;//设置是执行一次（false）还是一直执行(true)
            _timer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件
        }

        private static void ClearTimer()
        {
            _timer.Close();
            _timer.Dispose();
        }

        private static void OnElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Clear();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }
            finally
            {
                _timer.Start();
            }
        }


        /// <summary>
        /// 移除超过24小时已完成的事件锁、事件监视器、队列信息
        /// 我们不能在执行完领域事件后立即删除这些信息，因为有可能是外界调用本地的事件，稍后可能外界要求回逆事件
        /// 因此我们只删除24小时过期的信息，因为外界不可能过了24小时后又要求回逆
        /// </summary>
        /// <param name="minutes"></param>
        private static void Clear()
        {
            DataContext.NewScope(() =>
            {
                var repository = EventLockRepository.Instance;
                var locks = repository.FindExpireds(24);
                foreach (var @lock in locks)
                {
                    var queueId = @lock.Id;

                    var queue = EventQueue.Find(queueId);
                    if (!queue.IsSucceeded) continue; //对于没有执行成功的队列，我们不删除日志等信息，这样管理员可以排查错误

                    DataContext.NewScope(() =>
                    {
                        var monitor = EventMonitor.Find(queueId);
                        if (!monitor.IsEmpty()) EventMonitor.Delete(monitor);

                        EventQueue.Delete(queueId);
                        EventLogEntry.Deletes(queueId); //删除日志的所有条目
                        EventLog.Delete(queueId);
                    });
                    EventLock.Delete(@lock);
                }
            });
           
        }


        #endregion

    }
}