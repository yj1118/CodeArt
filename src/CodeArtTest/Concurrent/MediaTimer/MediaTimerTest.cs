using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

using CodeArt.Concurrent;
using CodeArt.TestTools;


namespace CodeArtTest.Concurrent
{
    [TestClass]
    public class MediaTimerTest
    {
        /// <summary>
        /// 高精度的沉睡
        /// </summary>
        [TestMethod]
        public void HighResolutionSleep()
        {
            const int times = 100; //运行次数
            const int sleepTime = 10; //毫秒

            Stopwatch watch = new Stopwatch();

            MediaDelayer delayer = new MediaDelayer();
            long elapsed = 0;
            for (var i = 0; i < times; i++)
            {
                watch.Restart();
                delayer.Sleep(sleepTime);
                elapsed += watch.ElapsedMilliseconds;
            }


            NumericalRangeInt nri = new NumericalRangeInt(sleepTime, 0.2f);
            var avg = (int)(elapsed / times);
            Assert.IsTrue(nri.Contains(avg));
        }


    }
}
