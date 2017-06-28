using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.DomainDriven;
using CodeArt.TestTools;

namespace CodeArt.DomainDrivenTest
{
    [TestClass]
    public class DomainObjectConstructTest : UnitTest
    {
        [TestMethod]
        public void Common()
        {
            int i = 0;
            BoundedContext.Register<InternalObject>(BoundedEvent.Constructed, (s, e) =>
            {
                i++;
            });
            var obj = new InternalObject(1);
            Assert.AreEqual(1, i);

            var obj2 = new InternalObject2(1);   //由于没有注册InternalObject的边界事件，所以i还是1
            Assert.AreEqual(1, i);

            obj = new InternalObject(1);
            Assert.AreEqual(2, i);
        }


        private class InternalObject : EntityObject<InternalObject, int>
        {
            public InternalObject(int id)
                : base(id)
            {
                this.OnConstructed();
            }
        }

        private class InternalObject2 : EntityObject<InternalObject2, int>
        {
            public InternalObject2(int id)
                : base(id)
            {
                this.OnConstructed();
            }
        }

    }
}
