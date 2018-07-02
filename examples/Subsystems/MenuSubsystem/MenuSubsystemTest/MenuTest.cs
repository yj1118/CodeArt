using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.DomainDriven.DataAccess;
using CodeArt.TestTools;

using MenuSubsystem;

namespace MenuSubsystemTest
{
    [TestClass]
    public class MenuTest : DomainStage
    {
        protected override void PreEnterScene()
        {
            DataPortal.RuntimeBuild();
        }

        protected override void LeftScene()
        {
            DataPortal.Dispose();
        }

        [TestMethod]
        public void CreateMenu()
        {
            this.BeginTransaction(true);

            var cmd = new CreateGroupMenu("菜单1", 1, Guid.Empty, "menu1");
            cmd.Execute();

            this.Commit();
        }
    }
}
