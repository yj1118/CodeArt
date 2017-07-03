using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeArt.TestTools
{
    public class AssertPro
    {
        /// <summary>
        /// 测试委托不会引发异常T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        public static void NotThrowsException<T>(Action action) where T : Exception
        {
            try
            {
                action();
            }
            catch (T)
            {
                Assert.Fail(string.Format(Strings.ThrowsException, typeof(T).FullName));
            }
        }

        public static void AreEqual<T>(IEnumerable<T> a, IEnumerable<T> b) where T : struct
        {
            var al = a.ToList();
            var bl = b.ToList();

            Assert.AreEqual(al.Count, bl.Count);
            for(var i=0;i<al.Count;i++)
            {
                Assert.AreEqual(al[i], bl[i]);
            }
        }

        /// <summary>
        /// 断言日期相等，精确到秒
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void AreEqualSecond(DateTime a, DateTime b)
        {
            Assert.IsTrue(a.Year == b.Year
                    && a.Month == b.Month
                    && a.Day == b.Day
                    && a.Hour == b.Hour
                    && a.Minute == b.Minute
                    && a.Second == b.Second);
        }

        /// <summary>
        /// 断言日期相等，精确到毫秒
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void AreEqualMillisecond(DateTime a, DateTime b)
        {
            AreEqualSecond(a, b);
            Assert.IsTrue(a.Millisecond == b.Millisecond);
        }

    }
}
