using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.TestTools;
using CodeArt.DTO;

namespace PortalServiceTest
{
    [TestClass]
    public class RoleTest : ServiceStage
    {
        [TestMethod]
        public void AddAndDeleteRoleWithNoPermissionNoOrg()
        {
            var roleId = TestUtil.AddRole("普通用户", null, null, default(Guid), null, false);
            var role = TestUtil.GetRole(roleId);
            Assert.AreEqual("普通用户", role.Name);
            TestUtil.DeleteRole(roleId);
        }


        private void AddRoleWithNoOrg()
        {
            var permissionId0 = TestUtil.AddPermission("发布文章", null, null);
            var permissionId1 = TestUtil.AddPermission("编辑文章", null, null);

            var permissionIds = new Guid[] { permissionId0, permissionId1 }.OrderBy(v => v).ToList();

            var roleId = TestUtil.AddRole("编辑人员", "站点编辑人员的角色", "editor", default(Guid),
                                        permissionIds, true);
            var role = TestUtil.GetRole(roleId);
            Assert.AreEqual("编辑人员", role.Name);
            Assert.AreEqual("站点编辑人员的角色", role.Description);
            Assert.AreEqual("editor", role.MarkedCode);
            Assert.AreEqual(true, role.IsSystem);
            Assert.AreEqual(2, role.Permissions.Count);

            var actualPermissions = ((DTObjects)role.Permissions).OrderBy((item) => ((dynamic)item).Id).ToList();


            Assert.AreEqual(permissionIds[0], ((dynamic)actualPermissions[0]).Id);
            Assert.AreEqual(permissionIds[1], ((dynamic)actualPermissions[1]).Id);

            this.Fixture.Add("roleId", roleId);
            this.Fixture.Add("permissionIds", permissionIds);
        }
        private void DeleteWithNoOrg()
        {
            var roleId = this.Fixture.Get<Guid>("roleId");
            var permissionIds = this.Fixture.Get<IEnumerable<Guid>>("permissionIds");

            TestUtil.DeleteRole(roleId);
            foreach (var permissionId in permissionIds)
            {
                TestUtil.DeletePermission(permissionId);
            }
        }

        [TestMethod]
        public void AddAndDeleteRoleWithNoOrg()
        {
            AddRoleWithNoOrg();
            DeleteWithNoOrg();
        }

        [TestMethod]
        public void UpdateRoleBase()
        {
            AddRoleWithNoOrg();

            var roleId = this.Fixture.Get<Guid>("roleId");

            TestUtil.UpdateRole(roleId, "编辑人员2", "站点编辑人员的角色2", null, null);

            var role = TestUtil.GetRole(roleId);
            Assert.AreEqual("编辑人员2", role.Name);
            Assert.AreEqual("站点编辑人员的角色2", role.Description);
            Assert.AreEqual(2, role.Permissions.Count);


            var actualPermissions = ((DTObjects)role.Permissions).OrderBy((item) => ((dynamic)item).Id).ToList();
            var permissionIds = this.Fixture.Get<IEnumerable<Guid>>("permissionIds").ToList();

            Assert.AreEqual(permissionIds[0], ((dynamic)actualPermissions[0]).Id);
            Assert.AreEqual(permissionIds[1], ((dynamic)actualPermissions[1]).Id);


            DeleteWithNoOrg();
        }

    }
}