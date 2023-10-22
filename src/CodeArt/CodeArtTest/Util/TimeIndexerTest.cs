using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.Util;

namespace CodeArtTest.Util
{

    /// <summary>
    /// 时间缓存器测试单元
    /// </summary>
    [TestClass]
    public class TimeIndexerTest
    {
        private static TimeIndexer<int, Guid> _getFifteen = new TimeIndexer<int, Guid>((value) =>
        {
            return Guid.NewGuid();

        }, IndexTime.Fifteen, IndexTimeType.Second);

        private static TimeIndexer<int, Guid> _getOne = new TimeIndexer<int, Guid>((value) =>
        {
            return Guid.NewGuid();

        }, IndexTime.One, IndexTimeType.Second);

        private static TimeIndexer<int, Guid> _getThirty = new TimeIndexer<int, Guid>((value) =>
        {
            return Guid.NewGuid();

        }, IndexTime.Thirty);

        private static TimeIndexer<int, Guid> _getTen = new TimeIndexer<int, Guid>((value) =>
        {
            return Guid.NewGuid();

        }, IndexTime.Ten, IndexTimeType.Second);

        private static TimeIndexer<int, Guid> _getFive = new TimeIndexer<int, Guid>((value) =>
        {
            return Guid.NewGuid();

        }, IndexTime.Five, IndexTimeType.Second);

        private static TimeIndexer<int, Guid> _getTwenty = new TimeIndexer<int, Guid>((value) =>
        {
            return Guid.NewGuid();
        }, IndexTime.Twenty, IndexTimeType.Second);

        const int value = 1;

        [TestMethod]
        public void One()
        {
            {
                var obj = _getOne.Get(value);
                Thread.Sleep(1000);
                Assert.AreNotEqual(obj, _getOne.Get(value));
            }


            {
                var obj = _getOne.Get(value);
                Thread.Sleep(100);
                Assert.AreEqual(obj, _getOne.Get(value));
            }

            {
                var obj = _getOne.Get(value);
                Thread.Sleep(300);
                Assert.AreEqual(obj, _getOne.Get(value));
            }


        }


        [TestMethod]
        public void Five()
        {
            {
                var obj = _getFive.Get(value);
                Thread.Sleep(6000);
                Assert.AreNotEqual(obj, _getFive.Get(value));
            }

            {
                var obj = _getFive.Get(value);
                Thread.Sleep(7000);
                Assert.AreNotEqual(obj, _getFive.Get(value));
            }

            {
                var obj = _getFive.Get(value);
                Thread.Sleep(5000);
                Assert.AreNotEqual(obj, _getFive.Get(value));
            }
        }


        [TestMethod]
        public void Ten()
        {
            {
                var obj = _getTen.Get(value);
                Thread.Sleep(10000);
                Assert.AreNotEqual(obj, _getTen.Get(value));
            }

            {
                var obj = _getTen.Get(value);
                Thread.Sleep(20000);
                Assert.AreNotEqual(obj, _getTen.Get(value));
            }

        }


        [TestMethod]
        public void Fifteen()
        {
            {
                var obj = _getFifteen.Get(value);
                Thread.Sleep(2000);
                Assert.AreEqual(obj, _getFifteen.Get(value));
            }

            {
                var obj = _getFifteen.Get(value);
                Thread.Sleep(16000);
                Assert.AreNotEqual(obj, _getFifteen.Get(value));
            }

        }

        /// <summary>
        /// 测试20分钟
        /// </summary>
        [TestMethod]
        public void Twenty()
        {
            {
                var obj = _getTwenty.Get(value);
                Thread.Sleep(21000);
                Assert.AreNotEqual(obj, _getTwenty.Get(value));
            }

            {
                var obj = _getTwenty.Get(value);
                Thread.Sleep(5000);
                Assert.AreEqual(obj, _getTwenty.Get(value));
            }
        }

        /// <summary>
        /// 测试30分钟
        /// </summary>
        [TestMethod]
        public void Thirty()
        {
            {
                var obj = _getThirty.Get(value);
                Thread.Sleep(5000);
                Assert.AreEqual(obj, _getThirty.Get(value));
            }

            {
                var obj = _getThirty.Get(value);
                Thread.Sleep(31000);
                Assert.AreNotEqual(obj, _getThirty.Get(value));
            }

            {
                var obj = _getThirty.Get(value);
                Thread.Sleep(41000);
                Assert.AreEqual(obj, _getThirty.Get(value));
            }
        }

    }
}
