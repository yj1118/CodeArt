using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.Util;
using CodeArt.TestTools;
using CodeArt.Concurrent;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.IO;
using CodeArt.DomainDriven.DataAccess;

using CodeArt.DomainDrivenTest.Demo;

namespace CodeArt.DomainDrivenTest
{
    /// <summary>
    /// UnitTest1 的摘要说明
    /// </summary>
    [TestClass]
    public class OtherTest : UnitTest
    {
        [TestMethod]
        public void SqlParserTest()
        {
            var sql = "select * from book where id=@id and ((etStart >=  @datetime and etStart < dateadd(day,1,@datetime)) or (etStart <=  @datetime and etEnd >= @datetime)) order by number asc,id desc";
            var columns = SqlParser.Parse(sql);
            Assert.AreEqual(1, columns.Select.Count());
            Assert.AreEqual(3, columns.Where.Count());
            Assert.AreEqual(2, columns.Order.Count());
        }

        [TestMethod]
        public void LoopClass()
        {
            var model = DataModel.Create(typeof(Menu));
            var root = model.Root;
            Assert.AreEqual(1, root.BuildtimeChilds.Count());

            var menu_parent = root.BuildtimeChilds.First();
            var menu_parent_parent = menu_parent.BuildtimeChilds.First();

            Assert.AreEqual(menu_parent, menu_parent_parent);
        }


    }
}
