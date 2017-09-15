using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.Concurrent.Pattern;
using CodeArt.TestTools;
using System.Diagnostics;

namespace CodeArtTest.Concurrent
{
    [TestClass]
    public class IntervalActionTest
    {
        [TestMethod]
        public void Common()
        {
            AutoResetEvent signal = new AutoResetEvent(false);
            int times = 0;
            List<long> elapseds = new List<long>();
            Stopwatch temp = new Stopwatch();
            IntervalAction action = new IntervalAction(50, handle);
            signal.WaitOne();

            var average = elapseds.Average();

            var range = new NumericalRangeInt(50, 0.2f);
            Assert.IsTrue(range.Contains((int)average));


            void handle(IntervalAction sender)
            {
                times++;
                if (temp.IsRunning)
                {
                    elapseds.Add(temp.ElapsedMilliseconds);
                }
                temp.Restart();
                if (times >= 10)
                {
                    sender.Dispose();
                    signal.Set();
                }
            }
        }
    }
}
