using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.Util;
using System.Diagnostics;
using CodeArt.Log;
using System.Collections;

namespace CodeArtTest.Util
{
    /// <summary>
    /// UnitTest1 的摘要说明
    /// </summary>
    [TestClass]
    public class ExtensionsTest
    {
        [TestMethod]
        public void GetWeekOfMonthFixed()
        {
            var _2018_10 = new DateTime(2018, 10, 1);

            CheckWeekOfMonthFixed(_2018_10, 1,7, 1);
            CheckWeekOfMonthFixed(_2018_10, 8,14, 2);
            CheckWeekOfMonthFixed(_2018_10, 15,21, 3);
            CheckWeekOfMonthFixed(_2018_10, 22, 28, 4);
            CheckWeekOfMonthFixed(_2018_10, 29, 31, 5);

            var _2018_11 = new DateTime(2018, 11, 1);
            CheckWeekOfMonthFixed(_2018_11, 1, 4, 10, 5);
            CheckWeekOfMonthFixed(_2018_11, 5, 11, 11, 1);
            CheckWeekOfMonthFixed(_2018_11, 12, 18, 11, 2);
            CheckWeekOfMonthFixed(_2018_11, 19, 25, 11, 3);
            CheckWeekOfMonthFixed(_2018_11, 26, 30, 11, 4);

            var _2018_12 = new DateTime(2018, 12, 1);
            CheckWeekOfMonthFixed(_2018_12, 1, 2, 11, 4);
            CheckWeekOfMonthFixed(_2018_12, 3, 9, 12, 1);
            CheckWeekOfMonthFixed(_2018_12, 10, 16, 12, 2);
            CheckWeekOfMonthFixed(_2018_12, 17, 23, 12, 3);
            CheckWeekOfMonthFixed(_2018_12, 24, 30, 12, 4);
            CheckWeekOfMonthFixed(_2018_12, 31, 31, 2019, 1, 1);

            var _2019_1 = new DateTime(2019, 1, 1);
            CheckWeekOfMonthFixed(_2019_1, 6, 1, 1, 1);
            CheckWeekOfMonthFixed(_2019_1, 7, 13, 1, 2);
            CheckWeekOfMonthFixed(_2019_1, 14, 20, 1, 3);
            CheckWeekOfMonthFixed(_2019_1, 21, 27, 1, 4);
            CheckWeekOfMonthFixed(_2019_1, 28, 31, 1, 5);

            var _2019_2 = new DateTime(2019, 2, 1);
            CheckWeekOfMonthFixed(_2019_2, 1, 3, 1, 5);
            CheckWeekOfMonthFixed(_2019_2, 4, 10, 2, 1);
            CheckWeekOfMonthFixed(_2019_2, 11, 17, 2, 2);
            CheckWeekOfMonthFixed(_2019_2, 18, 24, 2, 3);
            CheckWeekOfMonthFixed(_2019_2, 25, 28, 2, 4);


            var _2019_3 = new DateTime(2019, 3, 1);
            CheckWeekOfMonthFixed(_2019_3, 1, 3, 2, 4);
            CheckWeekOfMonthFixed(_2019_3, 4, 10, 3, 1);
            CheckWeekOfMonthFixed(_2019_3, 11, 17, 3, 2);
            CheckWeekOfMonthFixed(_2019_3, 18, 24, 3, 3);
            CheckWeekOfMonthFixed(_2019_3, 25, 31, 3, 4);

            var _2019_4 = new DateTime(2019, 4, 1);
            CheckWeekOfMonthFixed(_2019_4, 1, 7, 4, 1);
            CheckWeekOfMonthFixed(_2019_4, 8, 14, 4, 2);
            CheckWeekOfMonthFixed(_2019_4, 15, 21, 4, 3);
            CheckWeekOfMonthFixed(_2019_4, 22, 28, 4, 4);
            CheckWeekOfMonthFixed(_2019_4, 29, 30, 5, 1);

            var _2019_5 = new DateTime(2019, 5, 1);
            CheckWeekOfMonthFixed(_2019_5, 1, 5, 5, 1);
            CheckWeekOfMonthFixed(_2019_5, 6, 12, 5, 2);
            CheckWeekOfMonthFixed(_2019_5, 13, 19, 5, 3);
            CheckWeekOfMonthFixed(_2019_5, 20, 26, 5, 4);
            CheckWeekOfMonthFixed(_2019_5, 27, 31, 5, 5);

            var _2019_6 = new DateTime(2019, 6, 1);
            CheckWeekOfMonthFixed(_2019_6, 1, 2, 5, 5);
            CheckWeekOfMonthFixed(_2019_6, 3, 9, 6, 1);
            CheckWeekOfMonthFixed(_2019_6, 10, 16, 6, 2);
            CheckWeekOfMonthFixed(_2019_6, 17, 23, 6, 3);
            CheckWeekOfMonthFixed(_2019_6, 24, 30, 6, 4);

            var _2019_7 = new DateTime(2019, 7, 1);
            CheckWeekOfMonthFixed(_2019_7, 1, 7, 7, 1);
            CheckWeekOfMonthFixed(_2019_7, 8, 14, 7, 2);
            CheckWeekOfMonthFixed(_2019_7, 15, 21, 7, 3);
            CheckWeekOfMonthFixed(_2019_7, 22, 28, 7, 4);
            CheckWeekOfMonthFixed(_2019_7, 29, 31, 7, 5);

            var _2019_8 = new DateTime(2019, 8, 1);
            CheckWeekOfMonthFixed(_2019_8, 1, 4, 7, 5);
            CheckWeekOfMonthFixed(_2019_8, 5, 11, 8, 1);
            CheckWeekOfMonthFixed(_2019_8, 12, 18, 8, 2);
            CheckWeekOfMonthFixed(_2019_8, 19, 25, 8, 3);
            CheckWeekOfMonthFixed(_2019_8, 26, 31, 8, 4);

            var _2019_9 = new DateTime(2019, 9, 1);
            CheckWeekOfMonthFixed(_2019_9, 1, 1, 8, 4);
            CheckWeekOfMonthFixed(_2019_9, 2, 8, 9, 1);
            CheckWeekOfMonthFixed(_2019_9, 9, 15, 9, 2);
            CheckWeekOfMonthFixed(_2019_9, 16, 22, 9, 3);
            CheckWeekOfMonthFixed(_2019_9, 23, 29, 9, 4);
            CheckWeekOfMonthFixed(_2019_9, 30, 30, 10, 1);

            var _2019_10 = new DateTime(2019, 10, 1);
            CheckWeekOfMonthFixed(_2019_10, 1, 6, 10, 1);
            CheckWeekOfMonthFixed(_2019_10, 7, 13, 10, 2);
            CheckWeekOfMonthFixed(_2019_10, 14, 20, 10, 3);
            CheckWeekOfMonthFixed(_2019_10, 21, 27, 10, 4);
            CheckWeekOfMonthFixed(_2019_10, 28, 31, 10, 5);

            var _2019_11 = new DateTime(2019, 11, 1);
            CheckWeekOfMonthFixed(_2019_11, 1, 3, 10, 5);
            CheckWeekOfMonthFixed(_2019_11, 4, 10, 11, 1);
            CheckWeekOfMonthFixed(_2019_11, 11, 17, 11, 2);
            CheckWeekOfMonthFixed(_2019_11, 18, 24, 11, 3);
            CheckWeekOfMonthFixed(_2019_11, 25, 30, 11, 4);

            var _2019_12 = new DateTime(2019, 12, 1);
            CheckWeekOfMonthFixed(_2019_12, 1, 1, 11, 4);
            CheckWeekOfMonthFixed(_2019_12, 2, 8, 12, 1);
            CheckWeekOfMonthFixed(_2019_12, 9, 15, 12, 2);
            CheckWeekOfMonthFixed(_2019_12, 16, 22, 12, 3);
            CheckWeekOfMonthFixed(_2019_12, 23, 29, 12, 4);
            CheckWeekOfMonthFixed(_2019_12, 30, 31, 2020, 1, 1);

            var _2020_1 = new DateTime(2020, 1, 1);
            CheckWeekOfMonthFixed(_2020_1, 1, 5, 1, 1);
            CheckWeekOfMonthFixed(_2020_1, 6, 12, 1, 2);
            CheckWeekOfMonthFixed(_2020_1, 13, 19, 1, 3);
            CheckWeekOfMonthFixed(_2020_1, 20, 26, 1, 4);
            CheckWeekOfMonthFixed(_2020_1, 27, 31, 1, 5);

            var _2020_2 = new DateTime(2020, 2, 1);
            CheckWeekOfMonthFixed(_2020_2, 1, 2, 1, 5);
            CheckWeekOfMonthFixed(_2020_2, 3, 9, 2, 1);
            CheckWeekOfMonthFixed(_2020_2, 10, 16, 2, 2);
            CheckWeekOfMonthFixed(_2020_2, 17, 23, 2, 3);
            CheckWeekOfMonthFixed(_2020_2, 24, 29, 2, 4);

            var _2020_3 = new DateTime(2020, 3, 1);
            CheckWeekOfMonthFixed(_2020_3, 1, 1, 2, 4);
            CheckWeekOfMonthFixed(_2020_3, 2, 8, 3, 1);
            CheckWeekOfMonthFixed(_2020_3, 9, 15, 3, 2);
            CheckWeekOfMonthFixed(_2020_3, 16, 22, 3, 3);
            CheckWeekOfMonthFixed(_2020_3, 23, 29, 3, 4);
            CheckWeekOfMonthFixed(_2020_3, 30, 31, 4, 1);

            var _2020_4 = new DateTime(2020, 4, 1);
            CheckWeekOfMonthFixed(_2020_4, 1, 5, 4, 1);
            CheckWeekOfMonthFixed(_2020_4, 6, 12, 4, 2);
            CheckWeekOfMonthFixed(_2020_4, 13, 19, 4, 3);
            CheckWeekOfMonthFixed(_2020_4, 20, 26, 4, 4);
            CheckWeekOfMonthFixed(_2020_4, 27, 30, 4, 5);

            var _2020_5 = new DateTime(2020, 5, 1);
            CheckWeekOfMonthFixed(_2020_5, 1, 3, 4, 5);
            CheckWeekOfMonthFixed(_2020_5, 4, 10, 5, 1);
            CheckWeekOfMonthFixed(_2020_5, 11, 17, 5, 2);
            CheckWeekOfMonthFixed(_2020_5, 18, 24, 5, 3);
            CheckWeekOfMonthFixed(_2020_5, 25, 31, 5, 4);

            var _2020_6 = new DateTime(2020, 6, 1);
            CheckWeekOfMonthFixed(_2020_6, 1, 7, 6, 1);
            CheckWeekOfMonthFixed(_2020_6, 8, 14, 6, 2);
            CheckWeekOfMonthFixed(_2020_6, 15, 21, 6, 3);
            CheckWeekOfMonthFixed(_2020_6, 22, 28, 6, 4);
            CheckWeekOfMonthFixed(_2020_6, 29, 30, 7, 1);

        }

        private void CheckWeekOfMonthFixed(DateTime root, int minDay, int maxDay, int week)
        {
            for (var i = minDay; i <= maxDay; i++)
            {
                var time = root.AddDays(i - 1);
                var value = time.GetWeekOfMonthFixed();
                Assert.IsTrue(value.Week == week);
            }
        }

        private void CheckWeekOfMonthFixed(DateTime root, int minDay, int maxDay, int month, int week)
        {
            for (var i = minDay; i <=maxDay; i++)
            {
                var time = root.AddDays(i - 1);
                var value = time.GetWeekOfMonthFixed();
                Assert.IsTrue(value.Month == month);
                Assert.IsTrue(value.Week == week);
            }
        }


        private void CheckWeekOfMonthFixed(DateTime root, int minDay, int maxDay,int year, int month, int week)
        {
            for (var i = minDay; i <=maxDay; i++)
            {
                var time = root.AddDays(i - 1);
                var value = time.GetWeekOfMonthFixed();
                Assert.IsTrue(value.Year == year);
                Assert.IsTrue(value.Month == month);
                Assert.IsTrue(value.Week == week);
            }
        }


        [TestMethod]
        public void ArrayCompose()
        {
            List<string> temp0 = new List<string>() { "1", "2", "3", "4", "5" };
           // var result = temp0.Compose(2, (item) => item);

            List<string> temp1 = new List<string>() { "a","b","c"};

            List<string> temp2 = new List<string>() { "~", "!", "@" };

            var t = new List<object[]> { temp0.ToArray(), temp1.ToArray(), temp2.ToArray() }.GroupCompose();


        }

        

    }
}
