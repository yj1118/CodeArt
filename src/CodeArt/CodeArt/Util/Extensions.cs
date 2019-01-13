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
using System.Collections;

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
            StringBuilder message = new StringBuilder();
            while (ex != null)
            {
                message.AppendLine(ex.Message);
                ex = ex.InnerException;
            }
            return message.ToString();
        }

        public static IEnumerable<string> GetMessages(this Exception ex)
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
            if (ex == null || ex.IsUserUIException()) return string.Empty;  //不显示UserUIException的调用栈信息
            if (ex.InnerException == null) return ex.StackTrace;
            StringBuilder stackTrace = new StringBuilder();
            while (ex != null && !(ex is UserUIException))
            {
                stackTrace.AppendLine(ex.StackTrace);
                ex = ex.InnerException;
            }
            return stackTrace.ToString();
        }

        public static IEnumerable<string> GetStackTraces(this Exception ex)
        {
            if (ex == null || ex.IsUserUIException()) return Array.Empty<string>();  //不显示UserUIException的调用栈信息
            List<string> traces = new List<string>();
            Exception p = ex;
            while (p != null && !(p is UserUIException))
            {
                traces.Add(p.StackTrace);
                p = p.InnerException;
            }
            return traces;
        }

        public static string GetCompleteInfo(this Exception ex)
        {
            StringBuilder info = new StringBuilder();
            info.AppendLine(ex.GetCompleteMessage());
            info.Append(ex.GetCompleteStackTrace());
            return info.ToString();
        }

        #endregion

        //[Obsolete("请使用GetDayOfWeekCC")]
        //public static string GetDayOfWeekCN(this DateTime value)
        //{
        //    switch (value.DayOfWeek)
        //    {
        //        case DayOfWeek.Friday: return "星期五";
        //        case DayOfWeek.Monday: return "星期一";
        //        case DayOfWeek.Saturday: return "星期六";
        //        case DayOfWeek.Sunday: return "星期日";
        //        case DayOfWeek.Thursday: return "星期四";
        //        case DayOfWeek.Tuesday: return "星期二";
        //        case DayOfWeek.Wednesday: return "星期三";
        //    }
        //    return string.Empty;
        //}

        /// <summary>
        /// 显示汉字的版本
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDayOfWeekCN(this DateTime value)
        {
            switch (value.DayOfWeek)
            {
                case DayOfWeek.Friday: return "五";
                case DayOfWeek.Monday: return "一";
                case DayOfWeek.Saturday: return "六";
                case DayOfWeek.Sunday: return "日";
                case DayOfWeek.Thursday: return "四";
                case DayOfWeek.Tuesday: return "二";
                case DayOfWeek.Wednesday: return "三";
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取时间的开始日期，也就是：yyyy/MM/dd 00:00的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime Start(this DateTime value)
        {
            return value.Date;
        }

        /// <summary>
        /// 获取时间的结束日期，也就是：yyyy/MM/dd 23:59的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime End(this DateTime value)
        {
            return value.AddHours(23).AddMinutes(59);
        }


        /// <summary>
        /// 获取时间是月份的第几周
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetWeekOfMonth(this DateTime value)
        {
            int dayInMonth = value.Day;

            //本月第一天
            DateTime firstDay = value.AddDays(1 - value.Day);

            //本月第一天是周几
            int weekday = (int)firstDay.DayOfWeek == 0 ? 7 : (int)firstDay.DayOfWeek;

            //本月第一周有几天
            int firstWeekEndDay = 7 - (weekday - 1);

            //当前日期和第一周之差
            int diffday = dayInMonth - firstWeekEndDay;

            diffday = diffday > 0 ? diffday : 1;

            //当前是第几周,如果整除7就减一天
            int WeekNumInMonth = ((diffday % 7) == 0
             ? (diffday / 7 - 1)
             : (diffday / 7)) + 1 + (dayInMonth > firstWeekEndDay ? 1 : 0);
            return WeekNumInMonth;
        }

        /// <summary>
        /// 获取时间是月份的第几周，周控制在1-4
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static (int Year, int Month, int Week) GetWeekOfMonthFixed(this DateTime value)
        {
            const int splitter = 3;  //分割器，设置为3，是指：本月第一天在星期1,2,3的，那么本月第一天是本月第一周，否则，本月第一天在星期4,5,6,7的，本月第一天是上个月的最后一周

            var firstDay = new DateTime(value.Year, value.Month, 1); //本月第一天
            var firstDayOfWeek = firstDay.GetDayOfWeekNumber();

            var lastDay = firstDay.AddMonths(1).AddDays(-1); //本月最后一天
            var lastDayOfWeek = lastDay.GetDayOfWeekNumber();


            if (firstDayOfWeek <= splitter)
            {
                //本月的1号，在星期1、星期2,或者星期3，那么本月1号所在的周，就是本月第一周
                var firstWeekRemainingDays = 7 - firstDayOfWeek; //以1号为起始，其余的天数是在第一周的，还有remainingDays
                var firstWeeklastDay = 1 + firstWeekRemainingDays;

                if (value.Day >= 1 && value.Day <= firstWeeklastDay) return (value.Year, value.Month, 1);

                if (lastDayOfWeek < splitter)//本月最后一天，在星期1或星期2,那么本月最后一天所在的周，就是下个月的第1周
                {
                    var lastWeekFirstDay = lastDay.Day - lastDayOfWeek + 1;
                    if (value.Day >= lastWeekFirstDay && value.Day <= lastDay.Day)
                    {
                        var nextMonth = firstDay.AddMonths(1);
                        return (nextMonth.Year, nextMonth.Month, 1);
                    }
                }

                var dis = value.Day - firstWeeklastDay;
                var week = dis / 7 + 2;
                if (dis % 7 == 0) week--;
                return (value.Year, value.Month, week);
            }
            else
            {
                //本月的1号，在周里不是是星期1、星期2,或者星期3，那么本月1号所在的周，就是上个月的最后一周
                var firstWeekRemainingDays = 7 - firstDayOfWeek;
                var firstWeekLastDay = 1 + firstWeekRemainingDays;
                if (value.Day >= 1 && value.Day <= firstWeekLastDay)
                {
                    //上个月的最后一周
                    var prevMonthLastDay = firstDay.AddDays(-1);  //上个月的最后一天
                    return prevMonthLastDay.GetWeekOfMonthFixed();  //本月1号所在的周，等同于上个月最后一天所在的周
                }

                if (lastDayOfWeek < splitter)//本月最后一天，在星期1或星期2,那么本月最后一天所在的周，就是下个月的第1周
                {
                    var lastWeekFirstDay = lastDay.Day - lastDayOfWeek + 1;
                    if (value.Day >= lastWeekFirstDay && value.Day <= lastDay.Day)
                    {
                        var nextMonth = firstDay.AddMonths(1);
                        return (nextMonth.Year, nextMonth.Month, 1);
                    }
                }

                var dis = value.Day - firstWeekLastDay;
                var week = dis / 7 + 1;
                if (dis % 7 == 0) week--;
                return (value.Year, value.Month, week);
            }
        }

        /// <summary>
        /// 获取时间所在的周的时间范围：星期1的凌晨到星期天的11点59分59秒
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static (DateTime Start, DateTime End) GetWeekRange(this DateTime value)
        {
            int dayOfWeek = Convert.ToInt32(value.DayOfWeek);
            DateTime start, end;
            {
                var weeknow = (dayOfWeek == 0 ? (7 - 1) : (dayOfWeek - 1));
                int daydiff = (-1) * weeknow;
                var firstDay = value.AddDays(daydiff);
                start = new DateTime(firstDay.Year, firstDay.Month, firstDay.Day, 0, 0, 0);
            }

            {
                //星期天为最后一天  
                var weeknow = (dayOfWeek == 0 ? 7 : dayOfWeek);
                int daydiff = (7 - weeknow);
                var lastDay = value.AddDays(daydiff);
                end = new DateTime(lastDay.Year, lastDay.Month, lastDay.Day, 23, 59, 59);
            }

            return (start, end);
        }

        public static int GetDayOfWeekNumber(this DateTime value)
        {
            switch (value.DayOfWeek)
            {
                case DayOfWeek.Friday: return 5;
                case DayOfWeek.Monday: return 1;
                case DayOfWeek.Saturday: return 6;
                case DayOfWeek.Sunday: return 7;
                case DayOfWeek.Thursday: return 4;
                case DayOfWeek.Tuesday: return 2;
                case DayOfWeek.Wednesday: return 3;
            }
            throw new ApplicationException("GetDayOfWeekNumber发生未知的异常");
        }


        /// <summary>
        /// 获得当前时间是第几季度
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetQuarter(this DateTime value)
        {
            var quarter = DateTime.Now.Month / 3;
            if (DateTime.Now.Month % 3 > 0) quarter++;
            return quarter;
        }

        public static string GetAttributeValue(this XmlNode node, string attributeName)
        {
            return GetAttributeValue(node, attributeName, string.Empty);
        }

        public static string GetAttributeValue(this XmlNode node, string attributeName, string defaultValue)
        {
            if (node == null || node.Attributes == null) return defaultValue;
            //XmlAttribute attr = node.Attributes[attributeName]; 这个方法会区分大小写，所以改成下面不区分的大小写的算法
            XmlAttribute attr = null;
            foreach(XmlAttribute item in node.Attributes)
            {
                if(item.Name.EqualsIgnoreCase(attributeName))
                {
                    attr = item;
                    break;
                }
            }
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

        public static int GetCount(this IEnumerable e)
        {
            int i = 0;
            foreach (var t in e) i++;
            return i;
        }

        /// <summary>
        /// 集合是否有成员
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool Exists(this IEnumerable e)
        {
            foreach (var t in e) return true;
            return false;
        }

        public static bool Contains<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            foreach(var item in source)
            {
                if (predicate(item)) return true;
            }
            return false;
        }
    }
}
