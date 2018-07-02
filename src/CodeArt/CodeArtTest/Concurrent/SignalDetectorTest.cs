using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.Concurrent.Pattern;
using CodeArt.TestTools;

namespace CodeArtTest.Concurrent
{
    [TestClass]
    public class SignalDetectorTest
    {
        /// <summary>
        /// 检测到信号
        /// </summary>
        [TestMethod]
        public void Detected()
        {
            _Detected(5, 2);
            _Detected(10, 5);
            _Detected(10, 8);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="intervalMilliseconds">检测信号的时间间隔</param>
        /// <param name="sleep">发出信号的线程等待时间</param>
        private void _Detected(int intervalMilliseconds,int sleep)
        {
            using (var signal = new RepeatedDetector(() =>
             {
                 Assert.Fail("信号中断");
             }, intervalMilliseconds))
            {
                for (var i = 0; i < 5; i++)
                {
                    signal.Receive();
                    Thread.Sleep(sleep);
                }
            }
        }


        /// <summary>
        /// 没有检测到信号
        /// </summary>
        [TestMethod]
        public void NotDetected()
        {
            var intervalMilliseconds = 10;
            var sleep = intervalMilliseconds * 3;
            int _count = 0;
            using (var signal = new RepeatedDetector(() =>
             {
                 Interlocked.Increment(ref _count);
             }, intervalMilliseconds))
            {
                signal.Receive();
                Thread.Sleep(sleep);
                Assert.AreEqual(1, _count); //信号中断后，不再探测，所以就1次
            } 
        }

        /// <summary>
        /// 重复探测
        /// </summary>
        [TestMethod]
        public void RepeatedDetected()
        {
            var intervalMilliseconds = 10;
            var sleep = intervalMilliseconds * 5;
            int _count = 0;
            using (var signal = new RepeatedDetector(() =>
             {
                 Interlocked.Increment(ref _count);
             }, intervalMilliseconds, false))
            {
                signal.Receive();
                Thread.Sleep(sleep);
                Assert.IsTrue(_count > 1); //信号中断后，会重复，所以大于1
            }                
        }

        /// <summary>
        /// 一次性探测
        /// </summary>
        [TestMethod]
        public void OneOffDetected()
        {
            var intervalMilliseconds = 10;
            var sleep = intervalMilliseconds * 2;
            int _count = 0;
            using (var signal = new OneOffDetector(() =>
            {
                Interlocked.Increment(ref _count);
            }, intervalMilliseconds))
            {
                for(var i=0;i<3;i++)
                {
                    signal.Receive();
                    Thread.Sleep(sleep);
                }

                Assert.AreEqual(1, _count); //一次性探测，信号中断后，探测器不再工作，所以就1次
            }
        }

    }
}
