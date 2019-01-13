using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.DTO;
using System.Diagnostics;
using CodeArt.Log;

namespace CodeArtTest.DTO
{
    /// <summary>
    /// DTOTest 的摘要说明
    /// </summary>
    [TestClass]
    public class DTOTest
    {
        public DTOTest()
        {

        }

        [TestMethod]
        public void CreateDTO()
        {
            DTObject dto = DTObject.Create("{id,name}");
            dto.SetValue("id", 1);
            dto.SetValue("name", "刘备");

            Assert.AreEqual(1, dto.GetValue<int>("id"));
            Assert.AreEqual("刘备", dto.GetValue<string>("name"));
            //Assert.AreEqual("{\"id\",\"name\"}", dto.GetCode(false));
            Assert.AreEqual("{\"id\":1,\"name\":\"刘备\"}", dto.GetCode());
        }

        [TestMethod]
        public void CreateHaveValueDTO()
        {
            DTObject dto = DTObject.Create("{id:1,name:\"Louis\"}");

            Assert.AreEqual(1, dto.GetValue<int>("id"));
            Assert.AreEqual("Louis", dto.GetValue<string>("name"));
            Assert.AreEqual("{\"id\":1,\"name\":\"Louis\"}", dto.GetCode());
            //Assert.AreEqual("{\"id\",\"name\"}", dto.GetCode(false));
        }

        [TestMethod]
        public void CreateListDTO()
        {
            DTObject dto = DTObject.Create("{id,name,hobby:[{v,n}]}");
            dto.SetValue("id", 1);
            dto.SetValue("name", "Louis");
            DTObject obj = dto.CreateAndPush("hobby");
            obj.SetValue("v", 0);
            obj.SetValue("n", string.Format("LS{0}", 0));

            obj = dto.CreateAndPush("hobby");
            obj.SetValue("v", 1);
            obj.SetValue("n", string.Format("LS{0}", 1));

            DTObjects list = dto.GetList("hobby");
            for (int i = 0; i < list.Count; i++)
            {
                Assert.AreEqual(i, list[i].GetValue<int>("v"));
                Assert.AreEqual(string.Format("LS{0}", i), list[i].GetValue<string>("n"));
            }

            Assert.AreEqual(1, dto.GetValue<int>("id"));
            Assert.AreEqual("Louis", dto.GetValue<string>("name"));
            //Assert.AreEqual("{\"id\",\"name\",\"hobby\":[{\"v\",\"n\"}]}", dto.GetCode(false));
            //Assert.AreEqual("{\"id\":1,\"name\":\"Louis\",\"hobby\":[{\"v\":0,\"n\":\"LS0\"},{\"v\":1,\"n\":\"LS1\"}]}", dto.GetCode());

            var code = dto.GetCode();
            var copy = DTObject.Create(code);
            list = dto.GetList("hobby");
            for (int i = 0; i < list.Count; i++)
            {
                Assert.AreEqual(i, list[i].GetValue<int>("v"));
                Assert.AreEqual(string.Format("LS{0}", i), list[i].GetValue<string>("n"));
            }

            Assert.AreEqual(1, dto.GetValue<int>("id"));
            Assert.AreEqual("Louis", dto.GetValue<string>("name"));

        }

        [TestMethod]
        public void CreateNestListDTO()
        {
            DTObject dto = DTObject.Create("{items:[{v,n,childs:[{v,n}]}]}");

            DTObject objItems = dto.CreateAndPush("items");
            objItems.SetValue("v", 0);
            objItems.SetValue("n", string.Format("item{0}", 0));

            objItems = dto.CreateAndPush("items");
            objItems.SetValue("v", 1);
            objItems.SetValue("n", string.Format("item{0}", 1));

            DTObject objChilds = objItems.CreateAndPush("childs");
            objChilds.SetValue("v", 10);
            objChilds.SetValue("n", string.Format("child{0}", 10));

            objChilds = objItems.CreateAndPush("childs");
            objChilds.SetValue("v", 20);
            objChilds.SetValue("n", string.Format("child{0}", 20));


            //Assert.AreEqual("{\"items\":[{\"v\",\"n\",\"childs\":[{\"v\",\"n\"}]}]}", dto.GetCode(false));
            Assert.AreEqual("{\"items\":[{\"childs\":[],\"n\":\"item0\",\"v\":0},{\"childs\":[{\"n\":\"child10\",\"v\":10},{\"n\":\"child20\",\"v\":20}],\"n\":\"item1\",\"v\":1}]}", dto.GetCode(true));
        }

        [TestMethod]
        public void CreateSymbolDTO()
        {
            DTObject dto = DTObject.Create("{id,name,sex,hobbys:[{v,n}]}");
            dto.SetValue("id", 1);
            dto.SetValue("name", "loui's");
            dto.SetValue("sex", 9);

            DTObject objHobbys = dto.CreateAndPush("hobbys");
            objHobbys.SetValue("v", "1");
            objHobbys.SetValue("n", "！@#09/");

            Assert.AreEqual(1, dto.GetValue<int>("id"));
            Assert.AreEqual("loui's", dto.GetValue<string>("name"));
            Assert.AreEqual(9, dto.GetValue<int>("sex"));
            //Assert.AreEqual("{\"id\",\"name\",\"sex\",\"hobbys\":[{\"v\",\"n\"}]}", dto.GetCode(false));
            Assert.AreEqual("{\"hobbys\":[{\"n\":\"！@#09/\",\"v\":\"1\"}],\"id\":1,\"name\":\"loui's\",\"sex\":9}", dto.GetCode(true));
        }

        [TestMethod]
        public void CreateGuidDTO()
        {
            DTObject dto = DTObject.Create("{id}");
            dto.SetValue("id", Guid.Empty);

            Assert.AreEqual(Guid.Empty, dto.GetValue<Guid>("id"));
        }

        [TestMethod]
        public void CreateStringDTO()
        {
            DTObject dto = DTObject.Create("{name}");
            dto.SetValue("name", string.Empty);

            Assert.AreEqual(string.Empty, dto.GetValue<string>("name"));
        }

        [TestMethod]
        public void CreateBoolDTO()
        {
            DTObject dto = DTObject.Create("{isShow}");
            dto.SetValue("isShow", true);

            Assert.AreEqual(true, dto.GetValue<bool>("isShow"));
        }

        [TestMethod]
        public void CreateDateTimeDTO()
        {
            DTObject dto = DTObject.Create("{time}");
            dto.SetValue("time", DateTime.Parse("2031-08-05"));

            Assert.AreEqual(DateTime.Parse("2031-08-05"), dto.GetValue<DateTime>("time"));
        }

        [TestMethod]
        public void CreateObjectDTO()
        {
            var user = new User(1, "Louis");
            DTObject dto = DTObject.Create("{user}");
            var dtoUser = DTObject.Create("{id,name}", user);
            dto.SetValue("user", dtoUser);

            dynamic result = dto.GetValue("user");

            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Louis", result.Name);
        }

        private class User
        {
            private int _id;

            public int Id
            {
                get { return _id; }
                set { _id = value; }
            }

            private string _name;

            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            public User(int id, string name) { _id = id; _name = name; }
        }



        [TestMethod]
        public void CreateSocketMessageDTO()
        {
            DTObject dto = DTObject.Create("{\"RCN\":\"ControlBigScreenCapability\",\"REN\":\"PlayEvent\",\"MT\":7,\"Ds\":[\"[::ffff:192.168.0.13]:59714\"]}");
            var ds = dto.GetList("Ds");
            var code = dto.GetCode();

            Assert.AreEqual("[::ffff:192.168.0.13]:59714", ds[0].GetValue());
        }

    }
}
