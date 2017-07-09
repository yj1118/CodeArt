using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.TestTools;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;
using AccountSubsystem;

namespace AccountSubsystemTest
{
    [TestClass]
    public class PermissionTest : DomainStage
    {
        protected override void PreEnterScene()
        {
            //进入场景之前，注册仓储
            DataPortal.RuntimeBuild();
        }

        protected override void LeftScene()
        {
            DataPortal.Dispose();
        }

        protected override void EnteredScene()
        {
            AddPermission();
        }

        protected override void PreLeaveScene()
        {

        }

        private void AddPermission()
        {
            var cmd = new CreatePermission("发布文章")
            {
                MarkedCode="addArticle",
                Description = "站点编辑人员可以发布文章"
            };
            var permission = cmd.Execute();
            this.Fixture.Add(permission);
        }

        private void DeletePermission()
        {
            var permission = this.Fixture.Get<Permission>();
            var cmd = new DeletePermission(permission.Id);
            cmd.Execute();
        }



        [TestMethod]
        public void Add()
        {
            var permission = PermissionCommon.FindByMarkedCode("addArticle", QueryLevel.None);
            Assert.AreEqual("发布文章", permission.Name);
            Assert.AreEqual("站点编辑人员可以发布文章", permission.Description);
        }


        [TestMethod]
        public void Update()
        {
            var permissionId = this.Fixture.Get<Permission>().Id;

            {
                this.BeginTransaction();

                var cmd = new UpdatePermission(permissionId)
                {
                    Description = "编辑描述",
                    MarkedCode = "addArticle"
                };
                var permission = cmd.Execute();
                this.Commit();
            }

            {
                this.BeginTransaction();

                var permission = PermissionCommon.FindByMarkedCode("addArticle", QueryLevel.None);
                Assert.AreEqual("发布文章", permission.Name);
                Assert.AreEqual("编辑描述", permission.Description);
                Assert.AreEqual("addArticle", permission.MarkedCode);

                this.Commit();
            }
        }


        [TestMethod]
        public void Delete()
        {
            var permissionId = Guid.Empty;

            {
                this.BeginTransaction();

                var cmd = new CreatePermission("发布文章2");
                var permission = cmd.Execute();
                permissionId = permission.Id;

                this.Commit();
            }

            {
                var cmd = new DeletePermission(permissionId);
                cmd.Execute();
            }


            {
                this.BeginTransaction();

                var permission = PermissionCommon.FindById(permissionId, QueryLevel.None);
                Assert.IsTrue(permission.IsEmpty());

                this.Commit();
            }
        }

    }
}
