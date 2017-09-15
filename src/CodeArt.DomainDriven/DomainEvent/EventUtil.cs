using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace CodeArt.DomainDriven
{
    internal static class EventUtil
    {
        /// <summary>
        /// 获得“触发事件”的事件名称
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public static string GetRaise(string eventName)
        {
            return _getRaise(eventName);
        }

        private static Func<string, string> _getRaise = LazyIndexer.Init<string, string>((eventName) =>
        {
            return string.Format("{0}Raise", eventName);
        });

        /// <summary>
        /// 获取触发对方事件的结果的事件名称
        /// </summary>
        /// <returns></returns>
        public static string GetRaiseResult(string eventName, Guid eventId)
        {
            return string.Format("{0}RaiseResult - {1}", eventName, eventId);
        }


        /// <summary>
        /// 获得“回逆事件”的事件名称
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public static string GetReverse(string eventName)
        {
            return _getReverse(eventName);
        }

        private static Func<string, string> _getReverse = LazyIndexer.Init<string, string>((eventName) =>
        {
            return string.Format("{0}Reverse", eventName);
        });


    }
}
