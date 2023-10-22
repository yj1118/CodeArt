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

        /// <summary>
        /// 获取触发对方事件的结果的事件名称
        /// </summary>
        /// <returns></returns>
        public static string GetRaiseResult(string eventName, Guid eventId)
        {
            return string.Format("{0}-{1}", _getRaiseResult(eventName), eventId);
        }

        public static string GetReverse(string eventName)
        {
            return _getReverse(eventName);
        }


        private static Func<string, string> _getRaise = LazyIndexer.Init<string, string>((eventName) =>
        {
            eventName = _getEffectiveName(eventName);
            return string.Format("{0}Raise", eventName);
        });


        private static Func<string, string> _getRaiseResult = LazyIndexer.Init<string, string>((eventName) =>
        {
            eventName = _getEffectiveName(eventName);
            return string.Format("{0}RaiseResult", eventName);
        });


        /// <summary>
        /// 获得“回逆事件”的事件名称
        /// </summary>
        /// <param name="eventName"></param>

        private static Func<string, string> _getReverse = LazyIndexer.Init<string, string>((eventName) =>
        {
            eventName = _getEffectiveName(eventName);
            return string.Format("{0}Reverse", eventName);
        });


        private static Func<string, string> _getEffectiveName = LazyIndexer.Init<string, string>((eventName) =>
        {
            var pos = eventName.IndexOf("@");
            if (pos == -1) return eventName;
            return eventName.Substring(0, pos);
        });


        public static string GetEffectiveName(string eventName)
        {
            return _getEffectiveName(eventName);
        }

    }
}
