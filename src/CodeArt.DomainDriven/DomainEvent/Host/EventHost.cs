using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.EasyMQ.Event;

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
            if (IsFiltered(tip)) return;

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
            if (IsFiltered(tip)) return;

            var reverseName = EventUtil.GetReverse(tip.Name);
            EventPortal.Subscribe(reverseName, ReverseEventHandler.Instance);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="tip"></param>
        private static void CancelRaise(EventAttribute tip)
        {
            if (IsFiltered(tip)) return;

            var raiseName = EventUtil.GetRaise(tip.Name);
            EventPortal.Cancel(raiseName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tip"></param>
        private static void CanceleReverse(EventAttribute tip)
        {
            if (IsFiltered(tip)) return;

            var reverseName = EventUtil.GetReverse(tip.Name);
            EventPortal.Cancel(reverseName);
        }

        internal static bool IsFiltered(EventAttribute tip)
        {
            return !_hosts.Contains(tip.Name, StringComparer.OrdinalIgnoreCase);
        }


        #endregion

        #region 初始化和释放

        internal static void Initialize()
        {
            EventAttribute.Initialize();

            var tips = EventAttribute.Tips;
            foreach (var tip in tips)
            {
                SubscribeRaise(tip);
                SubscribeReverse(tip);
            }
        }

        internal static void Cleanup()
        {
            var tips = EventAttribute.Tips;
            foreach (var tip in tips)
            {
                CancelRaise(tip);
                CanceleReverse(tip);
            }
        }

        #endregion


        private static string[] _hosts = Array.Empty<string>(); //_hosts == null说明没有指定hosts说明启动所有的程序集中定义的事件

        /// <summary>
        /// 如果使用该方法，那么只会开启<paramref name="eventNames"/>指定的事件，程序集中EventAttribute定义的事件不再自动启动
        /// </summary>
        /// <param name="eventNames"></param>
        public static void SetHosts(params string[] eventNames)
        {
            _hosts = eventNames;
        }
    }
}