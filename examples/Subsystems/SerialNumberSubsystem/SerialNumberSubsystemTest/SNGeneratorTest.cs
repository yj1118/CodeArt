using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.DomainDriven;
using CodeArt.TestTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SerialNumberSubsystem;
using CodeArt.DomainDriven.DataAccess;

namespace SerialNumberSubsystemTest
{
    [TestClass]
    public class SNGeneratorTest : DomainStage
    {
        protected override void PreEnterScene()
        {
            //进入场景之前
            DataPortal.RuntimeBuild();
        }

        protected override void LeftScene()
        {
            DataPortal.Dispose();
        }


        protected override void EnteredScene()
        {

        }

        private SNGenerator AddSNGenerator()
        {
            //XY20170807
            //创建规则对象
            var hardcode = DTObject.CreateReusable();
            hardcode["ruleType"] = "hardcode";
            hardcode["content"] = "XY";

            var dateCode = DTObject.CreateReusable();
            dateCode["ruleType"] = "dateCode";

            var rules = new DTObject[] { hardcode, dateCode };

            //创建命令对象并且执行
            var cmd = new CreateGenerator("第一个流水号生成器", rules)
            {
                MarkedCode = "first"
            };
            var g = cmd.Execute();
            return g;
        }


        [TestMethod]
        public void Add()
        {
            var g = AddSNGenerator();

            Assert.AreEqual("第一个流水号生成器", g.Name);
        }


        [TestMethod]
        public void GetSN()
        {
            //XY20170807
            var g = AddSNGenerator();
            var number = g.Generate();

            Assert.AreEqual(string.Format("XY{0}",DateTime.Now.ToString("yyyyMMdd")), number);

        }

    }
}
