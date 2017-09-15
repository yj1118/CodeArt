using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Runtime.Caching;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.Util;
using CodeArt.TestTools;
using CodeArt.Concurrent;
using System.IO;

namespace CodeArtTest.Util
{
    /// <summary>
    /// UnitTest1 的摘要说明
    /// </summary>
    [TestClass]
    public class OtherTest : UnitTest
    {
        [TestMethod]
        public void BoxedAndUnboxed()
        {
            const int max = 100000000; //1亿次

            Stopwatch temp = new Stopwatch();
            temp.Restart();
            for(var i=0;i< max; i++)
            {
                //直接赋值
                int id = 0;
                int id2 = id;
            }
            var elasped1 = temp.ElapsedMilliseconds;

            temp.Restart();

            for (var i = 0; i < max; i++)
            {
                //装箱后，再拆箱赋值
                object id = 0;
                int id2 = (int)id;
            }
            var elasped2 = temp.ElapsedMilliseconds;
        }

        [TestMethod]
        public void IsASCII()
        {
            var a = "abc";
            Assert.IsTrue(a.IsASCII());

            a = "哈aa哈哈";
            Assert.IsFalse(a.IsASCII());
        }

        /// <summary>
        /// 测试静态构造
        /// 结论：当new一个实例时，静态构造会从基类依次执行,当时如果仅仅只是获得子类的静态成员，那么是不会触发基类的静态构造函数的
        /// </summary>
        [TestMethod]
        public void StaticConstructor()
        {
            var class2 = StaticClass2.Name;
            Assert.AreEqual(0, _staticClassCount);
            Assert.AreEqual(1, _staticClass2Count);
        }


        private static int _staticClassCount = 0;


        private class StaticClass
        {
            static StaticClass()
            {
                _staticClassCount++;
            }
        }


        private static int _staticClass2Count = 0;

        private class StaticClass2 : StaticClass
        {
            public static string Name = "xx";

            static StaticClass2()
            {
                _staticClass2Count++;
            }
        }

        /// <summary>
        /// 测试弱引用
        /// </summary>
        [TestMethod]
        public void WeakReferenceAndCache()
        {
            var value = new object();
            WeakReference<object> wr = new WeakReference<object>(value);

            MemoryCache cache = new MemoryCache("test");

            CacheItemPolicy policy = new CacheItemPolicy();
            policy.SlidingExpiration = TimeSpan.FromSeconds(1);

            cache.Add(new CacheItem("1", value), policy);

            //测试MemoryCache缓存是否受弱引用影响
            value = null;

            Thread.Sleep(2000);

            Assert.IsFalse(cache.Contains("1"));
            GC.Collect();
            Assert.IsFalse(wr.TryGetTarget(out value));

        }

    }
}
