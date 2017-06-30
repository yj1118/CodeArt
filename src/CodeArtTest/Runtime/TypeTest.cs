using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.Runtime;

namespace CodeArtTest.Runtime
{
    /// <summary>
    /// UnitTest1 的摘要说明
    /// </summary>
    [TestClass]
    public class TypeTest
    {
        [TestMethod]
        public void Depth()
        {
            var obj = new object();
            var objDepth = obj.GetType().GetDepth();
            Assert.AreEqual(0, objDepth);

            var d1 = new Depth1();
            var d1Depth = d1.GetType().GetDepth();
            Assert.AreEqual(1, d1Depth);

            var d2 = new Depth2();
            var d2Depth = d2.GetType().GetDepth();
            Assert.AreEqual(2, d2Depth);
        }


        private class Depth1
        {

        }

        private class Depth2 : Depth1
        {

        }

        [TestMethod]
        public void ResolveElementType()
        {
            IEnumerable<int> numbers = new int[] { 1, 2, 3 };
            var type =  numbers.GetType().ResolveElementType();
            Assert.AreEqual(type, typeof(int));

            Assert.IsTrue(numbers.GetType().IsList());
            Assert.IsFalse(numbers.GetType().IsDictionary());

            Assert.IsFalse(typeof(string).IsList());
        }

        [TestMethod]
        public void IsImplementOrEquals()
        {
            var t1 = typeof(BaseType<int, int, int>);
            Assert.IsTrue(t1.IsImplementOrEquals(typeof(BaseType<,,>)));

            var t2 = typeof(BaseType2<int, int>);
            Assert.IsTrue(t1.IsImplementOrEquals(typeof(BaseType<,,>)));

            var t3 = typeof(BaseType3);
            Assert.IsTrue(t3.IsImplementOrEquals(typeof(BaseType<,,>)));
        }

        private class BaseType<T1,T2,T3>
        {

        }

        private class BaseType2<T1,T2> : BaseType<T1,T2,int>
        {

        }

        private class BaseType3 : BaseType<string, int, int>
        {

        }


    }
}
