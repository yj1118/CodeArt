using System;
using System.Linq;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using System.Threading;
using CodeArt.Log;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Xml;

namespace CodeArt.Util
{
    /// <summary>
    /// 常用的扩展
    /// </summary>
    public static class Extensions
    {
        #region 异常

        /// <summary>
        /// 获取整个异常链上完整的异常信息
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetCompleteMessage(this Exception ex)
        {
            if (ex == null) return string.Empty;
            if (ex.InnerException == null) return ex.Message;
            StringBuilder message = new StringBuilder(ex.Message);
            ex = ex.InnerException;
            while (ex != null)
            {
                message.AppendLine(ex.Message);
                ex = ex.InnerException;
            }
            return message.ToString();
        }

        public static IList<string> GetMessages(this Exception ex)
        {
            List<string> msgs = new List<string>();
            Exception p = ex;
            while (p != null)
            {
                msgs.Add(p.Message);
                p = p.InnerException;
            }
            return msgs;
        }

        public static string GetCompleteStackTrace(this Exception ex)
        {
            if (ex == null) return string.Empty;
            if (ex.InnerException == null) return ex.StackTrace;
            StringBuilder stackTrace = new StringBuilder(ex.StackTrace);
            ex = ex.InnerException;
            while (ex != null)
            {
                stackTrace.AppendLine(ex.StackTrace);
                ex = ex.InnerException;
            }
            return stackTrace.ToString();
        }

        public static IList<string> GetStackTraces(this Exception ex)
        {
            List<string> traces = new List<string>();
            Exception p = ex;
            while (p != null)
            {
                traces.Add(p.StackTrace);
                p = p.InnerException;
            }
            return traces;
        }

        #endregion

        public static string GetDayOfWeekCN(this DateTime value)
        {
            switch (value.DayOfWeek)
            {
                case DayOfWeek.Friday: return "星期五";
                case DayOfWeek.Monday: return "星期一";
                case DayOfWeek.Saturday: return "星期六";
                case DayOfWeek.Sunday: return "星期日";
                case DayOfWeek.Thursday: return "星期四";
                case DayOfWeek.Tuesday: return "星期二";
                case DayOfWeek.Wednesday: return "星期三";
            }
            return string.Empty;
        }


        public static string GetAttributeValue(this XmlNode node, string attributeName)
        {
            return GetAttributeValue(node, attributeName, string.Empty);
        }

        public static string GetAttributeValue(this XmlNode node, string attributeName, string defaultValue)
        {
            if (node == null || node.Attributes == null) return defaultValue;
            XmlAttribute attr = node.Attributes[attributeName];
            return attr == null ? defaultValue : attr.Value;
        }

        /// <summary>
        /// 判断对象是否为null，这里会使用INullProxy
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNull(this object obj)
        {
            if (obj == null) return true;
            var proxy = obj as INullProxy;
            if (proxy != null) return proxy.IsNull();
            return false;
        }


        public static T Remove<T>(this IList<T> list, Func<T, bool> predicate)
        {
            var target = list.FirstOrDefault(predicate);
            if (target != null)
            {
                list.Remove(target);
                return target;
            }
            return default(T);
        }

    }
}
