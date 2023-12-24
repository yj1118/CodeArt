using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.Concurrent;
using CodeArt.TestTools;


namespace CodeArtTest.Concurrent
{
    [TestClass]
    public class PoolingTest
    {
        [TestMethod]
        public void BorrowCommon()
        {
            var item0 = _pool.Borrow();
            var item1 = _pool.Borrow();
            Assert.AreNotEqual(item0.Id, item1.Id);

            //还进去后再借出来，是同一个对象
            _pool.Return(item0);
            var item2 = _pool.Borrow();
            Assert.AreEqual(item0.Id, item2.Id);


            //新借第3次对象
            var item3 = _pool.Borrow();
            Assert.AreNotEqual(item2.Id, item3.Id);

        }


        private PoolWrapper<InternalClass> _pool = new PoolWrapper<InternalClass>(() =>
        {
            return new InternalClass();
        }, (item, phase) =>
          {
              return true;
          }, new PoolConfig()
          {
              LoanCapacity = 5
          });


        private class InternalClass : IDisposable
        {
            public Guid Id
            {
                get;
                private set;
            }

            public InternalClass()
            {
                this.Id = Guid.NewGuid();
            }

            public void Dispose()
            {
                this.Id = Guid.Empty;
            }

        }


        [TestMethod]
        public void TenItem()
        {
            List<object> array = new List<object>();
            Pool<object> pool = new Pool<object>(() => { return new object(); },
                                                 (obj, phase) => { return true; },
                                                 new PoolConfig { LoanCapacity = 10 });//最多有10个可以借出
            int maxBorrowedCount = 0;
            int maxWaiterCount = 0;
            //18个异步任务，访问池
            Parallel.For(0, 100, (i) =>
            {
                pool.Using(obj =>
                {
                    Thread.Sleep(5);
                    lock (this) //List<object>不是线程安全的
                    {
                        array.Add(obj);
                    }
                    maxBorrowedCount = pool.BorrowedCount > maxBorrowedCount ? pool.BorrowedCount : maxBorrowedCount;
                    maxWaiterCount = pool.WaiterCount > maxWaiterCount ? pool.WaiterCount : maxWaiterCount;
                });
            });
            Assert.AreEqual(100, array.Count);//执行了18次操作
            Assert.IsTrue(maxBorrowedCount <= 10); //最多同时进行10个操作
            Assert.IsTrue(maxWaiterCount < 90); //最多有90个操作等待池
        }

        [TestMethod]
        public void TenItemByWrapper()
        {
            List<object> array = new List<object>();
            PoolWrapper<object> pool = new PoolWrapper<object>(() => { return new object(); },
                                                 (obj, phase) => { return true; },
                                                 new PoolConfig { LoanCapacity = 10 });//最多有10个可以借出
            int maxBorrowedCount = 0;
            int maxWaiterCount = 0;
            //18个异步任务，访问池
            Parallel.For(0, 100, (i) =>
            {
                var obj = pool.Borrow();
                Thread.Sleep(5);
                lock (this) //List<object>不是线程安全的
                {
                    array.Add(obj);
                }
                maxBorrowedCount = pool.BorrowedCount > maxBorrowedCount ? pool.BorrowedCount : maxBorrowedCount;
                maxWaiterCount = pool.WaiterCount > maxWaiterCount ? pool.WaiterCount : maxWaiterCount;
                pool.Return(obj);
            });
            Assert.AreEqual(100, array.Count);//执行了100次操作
            Assert.IsTrue(maxBorrowedCount <= 10); //最多同时进行10个操作
            Assert.IsTrue(maxWaiterCount < 90); //最多有90个操作等待池
        }



        [TestMethod]
        public void OverstepRemainTime()
        {
            Pool<object> pool = new Pool<object>(() => { return new object(); },
                                                 (obj, phase) => { return true; },
                                                 new PoolConfig { MaxRemainTime = 3 });

            var item0 = pool.Borrow();
            Thread.Sleep(1000); //使用一秒
            pool.Return(item0);

            Thread.Sleep(1000);//在池中停留一秒
            var item1 = pool.Borrow();
            Assert.AreEqual(item1, item0);
            pool.Return(item1);

            //在池中停留两秒
            Thread.Sleep(2000);
            var item2 = pool.Borrow();
            Assert.AreEqual(item2, item1);
            pool.Return(item2);

            //在池中停留4秒
            Thread.Sleep(4000);
            var item3 = pool.Borrow();
            Assert.AreNotEqual(item3, item2); //超时，所以新建，并销毁原有的项
            pool.Return(item3);

        }

        [TestMethod]
        public void OverstepRemainTimeWrapper()
        {
            PoolWrapper<object> pool = new PoolWrapper<object>(() => { return new object(); },
                                                 (obj, phase) => { return true; },
                                                 new PoolConfig { MaxRemainTime = 3 });

            var item0 = pool.Borrow();
            Thread.Sleep(1000); //使用一秒
            pool.Return(item0);

            Thread.Sleep(1000);//在池中停留一秒
            var item1 = pool.Borrow();
            Assert.AreEqual(item1, item0);
            pool.Return(item1);

            //在池中停留两秒
            Thread.Sleep(2000);
            var item2 = pool.Borrow();
            Assert.AreEqual(item2, item1);
            pool.Return(item2);

            //在池中停留4秒
            Thread.Sleep(4000);
            var item3 = pool.Borrow();
            Assert.AreNotEqual(item3, item2); //超时，所以新建，并销毁原有的项
            pool.Return(item3);

        }

    }
}
