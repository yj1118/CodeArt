using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.DTO;

namespace CodeArtTest.DTO
{
    /// <summary>
    /// DTOPerformanceTest 的摘要说明
    /// </summary>
    [TestClass]
    public class DTOPerformanceTest
    {
        public DTOPerformanceTest()
        {

        }

        [TestMethod]
        public void CreateDTOByPerformance()
        {
            DTObject dto = DTObject.CreateReusable("{id,name,childs:[{id,name}]}");

            dto.SetValue("id",1);
            dto.SetValue("name", "刘备");

            var child0 = dto.CreateAndPush("childs");
            child0.SetValue("id", 2);
            child0.SetValue("name", "赵云");

            var child1 = dto.CreateAndPush("childs");
            child1.SetValue("id", 3);
            child1.SetValue("name", "马超");

            //Assert.AreEqual("{\"id\",\"name\",\"childs\":[{\"id\",\"name\"}]}", dto.GetCode(false));

            StringBuilder code = new StringBuilder();
            code.Append("{\"childs\":[");
            code.Append("{\"id\":2,\"name\":\"赵云\"},");
            code.Append("{\"id\":3,\"name\":\"马超\"}");
            code.Append("],\"id\":1,\"name\":\"刘备\"}");

            Assert.AreEqual(code.ToString(), dto.GetCode(true));

            //var data = TimeMonitor.Oversee(() =>
            //{
            //    for (var i = 0; i < 10000; i++)
            //    {
            //        DTObject.Create("{id,name,childs:[{id,name}]}");
            //    }
            //});
            //Assert.IsTrue(false, data.GetTime(0).ElapsedMilliseconds.ToString());
        }


        //[TestMethod]
        //public void CreateDTOBySimpleValue()
        //{
        //    var dto = DTObject.Create("{name:'刘备'}");
        //    Assert.AreEqual("刘备",dto.GetValue<string>("name"));

        //    var data = TimeMonitor.Oversee(() =>
        //    {
        //        for (var i = 0; i < 10000; i++)
        //        {
        //            DTObject.Create("{name:'刘备',childs:[{id,name,sex}]}");
        //        }
        //    });
        //    Assert.IsTrue(false, data.GetTime(0).ElapsedMilliseconds.ToString());

        //}

    }
}
