using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.TestTools;
using CodeArt.DTO;

namespace PortalServiceTest
{
    public static class TestUtil
    {
        public static Guid AddPermission(string name,string description,string markedCode)
        {
            var result = LocalServiceUtil.DynamicInvoke("addPermission", (arg) =>
            {
                arg.Name = name;
                arg.Description = description;
                arg.MarkedCode = markedCode;
            });
            return result.Id;
        }

        public static dynamic GetPermission(Guid id)
        {
            return LocalServiceUtil.DynamicInvoke("getPermission", (arg) => { arg.Id = id; });
        }

        public static void DeletePermission(Guid id)
        {
            LocalServiceUtil.DynamicInvoke("deletePermission", (arg) => { arg.Id = id; });
        }

        public static dynamic UpdatePermission(Guid id, string name, string description, string markedCode)
        {
            var result = LocalServiceUtil.DynamicInvoke("updatePermission", (arg) =>
            {
                arg.Id = id;
                arg.Name = name;
                arg.Description = description;
                arg.MarkedCode = markedCode;
            });
            return result.Id;
        }


        public static Guid AddRole(string name, 
                                        string description, 
                                        string markedCode,
                                        Guid organizationId,
                                        IEnumerable<Guid> permissionIds,
                                        bool isSystem)
        {
            var result = LocalServiceUtil.DynamicInvoke("addRole", (arg) =>
            {
                arg.Name = name;
                arg.Description = description;
                arg.MarkedCode = markedCode;
                arg.OrganizationId = organizationId;
                arg.PermissionIds = permissionIds;
                arg.IsSystem = isSystem;
            });
            return result.Id;
        }

        public static dynamic GetRole(Guid id)
        {
            return LocalServiceUtil.DynamicInvoke("getRole", (arg) => { arg.Id = id; });
        }

        public static void DeleteRole(Guid id)
        {
            LocalServiceUtil.DynamicInvoke("deleteRole", (arg) => { arg.Id = id; });
        }

        public static void UpdateRole(Guid id, 
                                        string name,
                                        string description,
                                        string markedCode,
                                        IEnumerable<Guid> permissionIds)
        {
            var result = LocalServiceUtil.DynamicInvoke("updateRole", (arg) =>
            {
                arg.Id = id;
                arg.Name = name;
                arg.Description = description;
                arg.MarkedCode = markedCode;
                arg.PermissionIds = permissionIds;
            });
        }

    }
}
