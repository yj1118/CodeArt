using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.Concurrent;
using CodeArt.TestTools;

namespace CodeArtTest.Concurrent
{
    [TestClass]
    public class SafeAccessTest
    {
        [TestMethod]
        public void IsSafe()
        {
            //InternalSafeClass是并发访问安全的
            Assert.IsTrue(SafeAccessAttribute.IsDefined(typeof(SafeInternalClass)));

            //InternalUnsafeClass不是并发访问安全的
            Assert.IsFalse(SafeAccessAttribute.IsDefined(typeof(UnsafeInternalClass)));
        }

        [TestMethod]
        public void CheckUp()
        {
            //检查InternalSafeClass,不会引起非并发访问安全的异常
            AssertPro.NotThrowsException<TypeUnsafeAccessException>(() =>
            {
                SafeAccessAttribute.CheckUp(typeof(SafeInternalClass));
            });

            //检查InternalUnsafeClass，会引起非并发访问安全的异常
            Assert.ThrowsException<TypeUnsafeAccessException>(() =>
            {
                SafeAccessAttribute.CheckUp(typeof(UnsafeInternalClass));
            });

        }


        [TestMethod]
        public void CreateInstance()
        {
            //不会重复创建打上并发访问安全类型特性的对象
            AssertPro.NotThrowsException<TypeUnsafeAccessException>(()=>
            {
                var obj0 = SafeAccessAttribute.CreateSingleton<SafeInternalClass>();
                var obj1 = SafeAccessAttribute.CreateSingleton<SafeInternalClass>();
                Assert.AreEqual(obj0.Id, obj1.Id);
            });

            //不能以单例形式创建没有打上并发访问安全类型特性的对象
            Assert.ThrowsException<TypeUnsafeAccessException>(() =>
            {
                SafeAccessAttribute.CreateSingleton<UnsafeInternalClass>();
            });

            //自动创建实例，也不会重复创建打上并发访问安全类型特性的对象
            {
                var obj0 = SafeAccessAttribute.CreateInstance<SafeInternalClass>();
                var obj1 = SafeAccessAttribute.CreateInstance<SafeInternalClass>();
                Assert.AreEqual(obj0.Id, obj1.Id);
            }

            //自动创建实例，会重复创建没有打上并发访问安全类型的对象
            {
                var obj0 = SafeAccessAttribute.CreateInstance<UnsafeInternalClass>();
                var obj1 = SafeAccessAttribute.CreateInstance<UnsafeInternalClass>();
                Assert.AreNotEqual(obj0.Id, obj1.Id);
            }
        }


        [SafeAccess]
        private class SafeInternalClass
        {
            public Guid Id = Guid.NewGuid();
        }


        private class UnsafeInternalClass
        {
            public Guid Id = Guid.NewGuid();
        }

    }
}
