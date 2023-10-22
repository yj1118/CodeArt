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
using CodeArt.Concurrent;
using CodeArt.Runtime;

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
            string msg = null;
            using (var temp = StringPool.Borrow())
            {
                var info = temp.Item;
                while (ex != null)
                {
                    if(!(ex is TargetInvocationException))  //忽略“调用的目标发生异常”的提示
                    {
                        info.AppendLine(ex.Message);
                    }
                    ex = ex.InnerException;
                }
                msg = info.ToString().Trim();
            }
            return msg;
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
            string msg = null;
            using (var temp = StringPool.Borrow())
            {
                var stackTrace = temp.Item;
                while (ex != null && !(ex is UserUIException))
                {
                    stackTrace.AppendLine(ex.StackTrace);
                    ex = ex.InnerException;
                }
                msg = stackTrace.ToString().Trim();
            }
            return msg;
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
            string msg = null;
            using (var temp = StringPool.Borrow())
            {
                StringBuilder info = temp.Item;
                info.AppendLine(ex.GetCompleteMessage());
                info.Append(ex.GetCompleteStackTrace());
                msg = info.ToString().Trim();
            }
            return msg;
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

        private static readonly DateTime _1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        /// <summary>
        /// 以1970年至今的总毫秒数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long TotalMillisecond(this DateTime value)
        {
            long valueTicks = value.Ticks;
            return (valueTicks - _1970.Ticks) / 10000;
        }

        /// <summary>
        /// 从1970年的毫秒数得到时间
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime GetDateTime(this long value)
        {
            return new DateTime(value * 10000 + _1970.Ticks);
        }

        public static long Timestamp(this DateTime value)
        {
            return (long)(value.TotalMillisecond() / 1000);
        }


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
            return value.Start().AddHours(23).AddMinutes(59);
        }


        /// <summary>
        /// 获取时间的开始分钟格式，也就是：yyyy/MM/dd mm:00的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime StartMinute(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0);
        }

        /// <summary>
        /// 获取时间的结束分钟格式，也就是：yyyy/MM/dd mm:59的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime EndMinute(this DateTime value)
        {
            return value.StartMinute().AddSeconds(59);
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
        /// 获取时间是月份的第几周
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

        public static (DateTime Start, DateTime End) GetMonthRange(this DateTime value)
        {
            //获取某年某月有多少天
            int monthDay = DateTime.DaysInMonth(value.Year, value.Month);

            DateTime start = new DateTime(value.Year, value.Month, 1).Start();
            DateTime end = new DateTime(value.Year, value.Month, monthDay).End();
            return (start, end);
        }

        /// <summary>
        /// 本月最大天数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int MaxDays(this DateTime value)
        {
            //获取某年某月有多少天
            return DateTime.DaysInMonth(value.Year, value.Month);
        }

        public static (DateTime Start, DateTime End) GetDayRange(this DateTime value)
        {
            return (value.Start(), value.End());
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

        /// <summary>
        /// 是否为同一天
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SameDay(this DateTime value0, DateTime value1)
        {
            return value0.Year == value1.Year && value0.Month == value1.Month && value0.Day == value1.Day;
        }

        private static Func<int, string> _getHumanizeHour = LazyIndexer.Init<int, string>((hour)=>
        {
            if (hour >= 0 && hour <= 12)
            {
                return "上午";
            }
            else
            {
                return "下午";
            }
        });

        /// <summary>
        /// 获得人性化时间
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Humanize(this DateTime value)
        {
            var now = DateTime.Now;
            var yesterday = DateTime.Now.AddDays(-1);

            var tip = _getHumanizeHour(value.Hour);
            if (value.Year == now.Year && value.Month == now.Month && value.Day == now.Day) return string.Format("今天 {0}{1:HH:mm}", tip, value);
            if(value.Year == yesterday.Year && value.Month == yesterday.Month && value.Day == yesterday.Day) return string.Format("昨天 {0}{1:HH:mm}", tip, value);
            return string.Format("{1:MM月dd日} {0}{1:HH:mm}", tip, value);
        }

        /// <summary>
        /// 自动补充时区
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime TimeZone(this DateTime value)
        {
            if(value.Kind == DateTimeKind.Unspecified) //如果没有时区，就指定本地时区
                return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, DateTimeKind.Local);
            return value;
        }

        const long _minute = 60; 
        const long _hour = 60 * _minute;
        const long _day = 24 * _hour;

        public static string Diff(this DateTime self, DateTime value)
        {
            var gap = new TimeSpan(value.Ticks - self.Ticks).TotalSeconds;

            if (gap < _minute) return "0分钟";
            if (gap < _hour) return Math.Round((double)(gap / _minute)) + "分钟";
            if (gap < _day) return Math.Round((double)(gap / _hour)) + "小时";
            return "长时间";
        }

        public static double DiffHour(this DateTime self, DateTime value)
        {
            return new TimeSpan(value.Ticks - self.Ticks).TotalHours;
        }

        public static double DiffMinute(this DateTime self, DateTime value)
        {
            return new TimeSpan(value.Ticks - self.Ticks).TotalMinutes;
        }


        public static DateTime SetMinute(this DateTime value, int minute)
        {
            return new DateTime(value.Year, value.Month, value.Day, value.Hour, minute, value.Second);
        }

        public static DateTime SetMinute(this DateTime value, int minute,int second)
        {
            return new DateTime(value.Year, value.Month, value.Day, value.Hour, minute, second);
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
            foreach (XmlAttribute item in node.Attributes)
            {
                if (item.Name.EqualsIgnoreCase(attributeName))
                {
                    attr = item;
                    break;
                }
            }
            return attr == null ? defaultValue : attr.Value;
        }

        public static void SetAttributeValue(this XmlNode node, string attributeName, string value)
        {
            XmlElement element = node as XmlElement;
            if (element == null) throw new Exception("不能设置属性");
            element.SetAttribute(attributeName, value);
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
            foreach (var item in source)
            {
                if (predicate(item)) return true;
            }
            return false;
        }

        public static IEnumerable<TSource> Last<TSource>(this IEnumerable<TSource> source, int count)
        {
            List<TSource> result = new List<TSource>();
            var max = source.GetCount();
            for (var i = 0; i < count; i++)
            {
                var pos = max - count + i;
                if (pos < 0) continue;
                var e = source.ElementAt(pos);
                result.Add(e);
            }
            return result;
        }

        /// <summary>
        /// 将集合<paramref name="source"/>转变成为<paramref name="target"/>，需要增加哪些元素和需要删除哪些元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static (IEnumerable<T> Adds, IEnumerable<T> Removes, IEnumerable<T> Updates) Transform<T>(this IEnumerable<T> source, IEnumerable<T> target)
        {
            return Transform<T>(source, target, (s, t) =>
            {
                return s.Equals(t);
            });
        }

        public static (IEnumerable<T> Adds, IEnumerable<T> Removes, IEnumerable<T> Updates) Transform<T>(this IEnumerable<T> source, IEnumerable<T> target, Func<T, T, bool> equals)
        {
            return Transform<T, T>(source, target, equals);
        }

        /// <summary>
        /// 将集合<paramref name="source"/>转变成为<paramref name="target"/>，需要增加哪些元素和需要删除哪些元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="equals"></param>
        /// <returns></returns>
        public static (IEnumerable<TT> Adds, IEnumerable<ST> Removes, IEnumerable<TT> Updates) Transform<ST,TT>(this IEnumerable<ST> source, IEnumerable<TT> target, Func<ST, TT, bool> equals)
        {
            List<ST> souceCopy = new List<ST>(source);
            List<TT> targetCopy = new List<TT>(target);

            if (source.Count() == 0)
                return (targetCopy, Array.Empty<ST>(), Array.Empty<TT>());

            if (target.Count() == 0)
                return (Array.Empty<TT>(), souceCopy, Array.Empty<TT>());

            List<TT> sames = new List<TT>(); //需要保留的

            //bool isClass = typeof(TT).IsClass;
            bool isClass = typeof(TT).IsClass || typeof(TT).IsInterface;

            //有相同的
            foreach (var item in source)
            {
                var same = target.FirstOrDefault((t) => { return equals(item, t); });

                //if (!same.Equals(default(TT)))
                //{
                //    sames.Add(same);
                //}

                if (isClass)
                {
                    if (same != null)  //找到相同的保留
                    {
                        sames.Add(same);
                    }
                }
                else
                {
                    if (!same.Equals(default(TT)))
                    {
                        sames.Add(same);
                    }
                }

            }

            foreach (var same in sames)
            {
                souceCopy.Remove((item)=>
                {
                    return equals(item, same);
                });
                targetCopy.Remove(same);
            }

            return (targetCopy, souceCopy, sames);
        }


        /// <summary>
        /// 不四舍五入保留小数点后几位
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="digits">保留位数</param>
        /// <returns></returns>
        public static float LeastRound(this float value, int digits)
        {
            //不能直接从float强制转换double，在某些情况会丢失精确度，比如6.599，结果是6.598999
            return (float)LeastRound(double.Parse(value.ToString()), digits);
        }

        /// <summary>
        /// 不四舍五入保留小数点后几位
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="digits">保留位数</param>
        /// <returns></returns>
        public static double LeastRound(this double value, int digits)
        {
            var pow = Math.Pow(10, digits);
            return Math.Floor(value * pow) / pow;
        }

        /// <summary>
        /// 不四舍五入保留小数点后几位
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="digits">保留位数</param>
        /// <returns></returns>
        public static decimal LeastRound(this decimal value, int digits)
        {
            var pow = (decimal)Math.Pow(10, digits);
            return Math.Floor(value * pow) / pow;
        }

        /// <summary>
        /// 四舍五入保留小数点后几位
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="digits">保留位数</param>
        /// <returns></returns>
        public static float Round(this float value, int digits)
        {
            return (float)Math.Round(value, digits, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 四舍五入保留小数点后几位
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="digits">保留位数</param>
        /// <returns></returns>
        public static double Round(this double value, int digits)
        {
            return Math.Round(value, digits, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 四舍五入保留小数点后几位
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="digits">保留位数</param>
        /// <returns></returns>
        public static decimal Round(this decimal value, int digits)
        {
            return Math.Round(value, digits, MidpointRounding.AwayFromZero);
        }


        /// <summary>
        /// 根据key来过滤重复
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> Distinct<TSource,TKey>(this IEnumerable<TSource> source,Func<TSource, TKey> getKey)
        {
            var comparer = new KeyComparer<TSource, TKey>(getKey);
            return source.Distinct(comparer);
        }

        private class KeyComparer<TSource, TKey> : IEqualityComparer<TSource>
        {
            private Func<TSource, TKey> _getKey;

            public KeyComparer(Func<TSource, TKey> getKey)
            {
                _getKey = getKey;
            }

            public bool Equals(TSource x, TSource y)
            {
                return Object.Equals(_getKey(x), _getKey(y));
            }

            public int GetHashCode(TSource obj)
            {
                return _getKey(obj).GetHashCode();
            }
        }


        /// <summary>
        /// 生成Guid的16位短格式;重复几率很小，大概1亿次内都不会重复，可以用于领域对象编号，就算重复，也会由于主键重复报错，不会提交数据
        /// 当我们想要获得一个唯一的key的时候，通常会想到GUID。这个key非常的长，虽然我们在很多情况下这并不是个问题。但是当我们需要将这个36个字符的字符串放在URL中时，会使的URL非常的丑陋。
        ///想要缩短GUID的长度而不牺牲它的唯一性是不可能的，但是如果我们能够接受一个16位的字符串的话是可以做出这个牺牲的。
        ///我们可以将一个标准的GUID 21726045-e8f7-4b09-abd8-4bcc926e9e28 转换成短的字符串 3c4ebc5f5f2c4edc
        ///下面的方法会生成一个短的字符串，并且这个字符串是唯一的。重复1亿次都不会出现重复的，它也是依照GUID的唯一性来生成这个字符串的。
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string Short(this Guid guid)
        {
            long i = 1;
            foreach (byte b in guid.ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static long Long(this Guid guid)
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }

    }
}
