using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.Runtime;
using CodeArt.TestTools;

namespace CodeArtTest.Runtime
{



    [TestClass]
    public class RuntimeUtilTest : UnitTest
    {
        [TestMethod]
        public void PropertyValue()
        {
            var obj = new InternalClass()
            {
                Id= 1,
                Name = "haha"
            };

            var id = obj.GetPropertyValue<int>("Id");
            Assert.AreEqual(obj.Id, id);

            var name =  obj.GetPropertyValue<string>("Name");
            Assert.AreEqual(obj.Name, name);


            var id2 = 2;
            var name2 = "heihei";


            obj.SetPropertyValue("Id", id2);
            Assert.AreEqual(obj.Id, id2);

            obj.SetPropertyValue("Name", name2);
            Assert.AreEqual(obj.Name, name2);


            //id = obj.GetPropertyValue<int>("id");
            //Assert.AreEqual(obj.Id, id);

            //name = obj.GetPropertyValue<string>("Name");
            //Assert.AreEqual(obj.Name, name);

        }


        private class InternalClass
        {
            public int Id
            {
                get;
                set;
            }

            public string Name
            {
                get;
                set;
            }




            public InternalClass()
            {

            }
        }




        [TestMethod]
        public void Sleep()
        {
            //RuntimeUtil.Sleep(10000);
        }
    }


}
