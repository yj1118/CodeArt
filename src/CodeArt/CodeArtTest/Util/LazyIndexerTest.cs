using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.Util;

namespace CodeArtTest.Util
{
    /// <summary>
    /// UnitTest1 的摘要说明
    /// </summary>
    [TestClass]
    public class LazyIndexerTest
    {
        [TestMethod]
        public void LazyCreate()
        {
            //在并发的状态下，多次创建数量为instanceCount的懒惰对象
            //每个对象的id为小于instanceCount的数字
            const int instanceCount = 20;
            Parallel.For(0, instanceCount, (id) =>
            {
                //以下代码是在并发状态下运行1000次获取实例的代码，主要测试
                //LazyIndexer.Init在并发状态下也能正常工作
                Parallel.For(0, 1000, (j) =>
                {
                    var obj = _getInstance(id);
                });
            });

            Assert.AreEqual(InternalClass.ConstructCount, instanceCount);
        }

        private Func<int, InternalClass> _getInstance = LazyIndexer.Init<int, InternalClass>((id)=>
        {
            return new InternalClass(id);
        });

        private class InternalClass
        {
            private static object _syncObject = new object();
            public static int _constructCount = 0;

            public static int ConstructCount
            {
                get
                {
                    return _constructCount;
                }
            }


            public int Number { get; private set; }
            public InternalClass(int number)
            {
                this.Number = number;
                Interlocked.Increment(ref _constructCount);
            }
        }


        #region LazyCache

        [TestMethod]
        public void LazyCache()
        {
            var indexer = new LazyIndexer<int, InternalObject>((id) =>
            {
                return new InternalObject(id);
            });

            const int instanceCount = 20;
            Parallel.For(0, instanceCount, (id) =>
            {
                //以下代码是在并发状态下运行1000次获取实例的代码，主要测试
                //LazyIndexer.Init在并发状态下也能正常工作
                Parallel.For(0, 1000, (j) =>
                {
                    var obj = indexer.Get(id);
                });
            });

            Assert.AreEqual(indexer.Keys.Length, instanceCount);

            var halfInstanceCount = instanceCount / 2;

            //减去一半
            Parallel.For(0, halfInstanceCount, (id) =>
            {
                Parallel.For(0, 1000, (j) =>
                {
                    indexer.Remove(id);
                });
            });

            Assert.AreEqual(indexer.Keys.Length, halfInstanceCount);
        }


        private class InternalObject
        {
            private static object _syncObject = new object();


            public int Number { get; private set; }
            public InternalObject(int number)
            {
                this.Number = number;
            }
        }

        #endregion

        #region LazyFilter


        [TestMethod]
        public void LazyFilter()
        {
            var indexer = new LazyIndexer<int, InternalFilterClass>((id) =>
            {
                return new InternalFilterClass(id);
            },(item)=>
            {
                if (item.Number % 2 == 0) return false; //不保留2的倍数
                return true;
            });

            const int instanceCount = 20;
            Parallel.For(0, instanceCount, (id) =>
            {
                Parallel.For(0, 20, (j) =>
                {
                    var obj = indexer.Get(id);
                });
            });

            foreach(var value in indexer.Values)
            {
                Assert.IsTrue(value.Number % 2 > 0);
            }

            Assert.AreEqual(indexer.Keys.Length, instanceCount / 2);


            for (var i= 0; i < instanceCount;i++)
            {
                var obj = _getFilterInstance(i);
                if(i % 2 == 0)
                {
                    //由于是不缓存的项，所以两次编号不同
                    var obj2 = _getFilterInstance(i);
                    Assert.AreNotEqual(obj.Id, obj2.Id);
                }
                else
                {
                    //由于是缓存的项，所以两次编号相同
                    var obj2 = _getFilterInstance(i);
                    Assert.AreEqual(obj.Id, obj2.Id);
                }
            }
        }


        private Func<int, InternalFilterClass> _getFilterInstance = LazyIndexer.Init<int, InternalFilterClass>((id) =>
        {
            return new InternalFilterClass(id);
        }, (item) =>
        {
            if (item.Number % 2 == 0) return false; //不保留2的倍数
            return true;
        });



        private class InternalFilterClass
        {
            private static object _syncObject = new object();
            public Guid Id
            {
                get;
                private set;
            }

            public int Number { get; private set; }
            public InternalFilterClass(int number)
            {
                this.Id = Guid.NewGuid();
                this.Number = number;
            }
        }

        #endregion


        #region 辅助


        private sealed class CalculateNumberCls
        {
            private Func<string, int> _getNumber;

            private int _calculateTimes = 0;//计算次数
            public int CalculateTimes
            {
                get { return _calculateTimes; }
            }

            private int CalculateNumber(string key)
            {
                Thread.Sleep(_waite);//模拟计算key->int需要_waite毫秒时间
                _calculateTimes++;
                return int.Parse(key);
            }

            private int _waite = 0;
            public CalculateNumberCls(int waite)
            {
                _waite = waite;
                _getNumber = LazyIndexer.Init<string, int>(CalculateNumber);
            }

            public int GetNumber(string key)
            {
                return _getNumber(key);
            }

        }





        #endregion


        /// <summary>
        /// 并发访问
        /// </summary>
        [TestMethod]
        public void ConcurrentAccess()
        {
            CalculateNumberCls cls = new CalculateNumberCls(10);
            //第一次，是写
            Parallel.For(0, 10, (index) =>
            {
                for (var i = 0; i < 10; i++)
                {
                    int value = cls.GetNumber(i.ToString());
                    Assert.AreEqual(i, value);
                }
            });

            //第二次，是直接从缓冲区中读
            Parallel.For(0, 10, (index) =>
            {
                for (var i = 0; i < 10; i++)
                {
                    int value = cls.GetNumber(i.ToString());
                    Assert.AreEqual(i, value);
                }
            });

           
            Assert.AreEqual(10, cls.CalculateTimes);//实际只计算了10次
        }

    }
}
