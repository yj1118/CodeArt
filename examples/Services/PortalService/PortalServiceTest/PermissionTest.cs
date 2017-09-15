using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.TestTools;
using CodeArt.DTO;
using CodeArt.DomainDriven.DataAccess;

namespace PortalServiceTest
{
    [TestClass]
    public class PermissionTest : ServiceStage
    {
        private Guid AddPermission()
        {
            var id = TestUtil.AddPermission("发布文章", "编辑人员可以发布文章", "addArticle");
            Assert.IsNotNull(id);
            return id;
        }

        [TestMethod]
        public void AddAndDeletePermission()
        {
            var id = AddPermission();
            var detail = TestUtil.GetPermission(id);

            Assert.AreEqual(id, detail.Id);
            Assert.AreEqual("发布文章", detail.Name);
            Assert.AreEqual("编辑人员可以发布文章", detail.Description);
            Assert.AreEqual("addArticle", detail.MarkedCode);

            TestUtil.DeletePermission(id);

            detail = TestUtil.GetPermission(id);
            Assert.IsNull(detail.Id);
        }

        [TestMethod]
        public void UpdatePermission()
        {
            var id = AddPermission();

            TestUtil.UpdatePermission(id, "发布文章2", "编辑人员可以发布文章2", null);

            var detail = TestUtil.GetPermission(id);
            Assert.AreEqual("发布文章2", detail.Name);
            Assert.AreEqual("编辑人员可以发布文章2", detail.Description);
            Assert.AreEqual("addArticle", detail.MarkedCode);

            TestUtil.DeletePermission(id);

            detail = TestUtil.GetPermission(id);
            Assert.IsNull(detail.Id);
        }

    }
}
