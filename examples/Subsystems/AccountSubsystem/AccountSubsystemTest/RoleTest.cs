using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.TestTools;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;
using AccountSubsystem;

namespace AccountSubsystemTest
{
    [TestClass]
    public class RoleTest : DomainStage
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

        protected override void PreLeaveScene()
        {

        }

        /// <summary>
        /// 测试增加角色
        /// </summary>
        [TestMethod]
        public void Add()
        {
            var cmd = new CreateRole("普通员工", new Guid[] { }, true);
            var role = cmd.Execute();

            Assert.AreEqual("普通员工", role.Name);
        }


    }
}
