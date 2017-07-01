using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using CodeArt.Runtime;
using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    public class SecurityIdentity : EntityObject
    {
        private List<Role> _roles;

        public SecurityIdentity(IList<Role> roles)
        {
            _roles = new List<Role>(roles);
        }

        public Role[] GetRoles()
        {
            return _roles.ToArray();
        }

        public bool InRole(Role role)
        {
            return _roles.FirstOrDefault((t) =>
            {
                return t.Equals(role);
            }) != null;
        }

        public bool InRole(string roleMarkedCode)
        {
            return _roles.FirstOrDefault((t) =>
            {
                return t.MarkedCode.EqualsIgnoreCase(roleMarkedCode);
            }) != null;
        }

        /// <summary>
        /// 满足角色之一
        /// </summary>
        /// <param name="roleMarkedCodes"></param>
        /// <returns></returns>
        public bool InRoleAnyOne(params string[] roleMarkedCodes)
        {
            foreach(var markedCode in roleMarkedCodes)
            {
                if (InRole(markedCode)) return true;
            }
            return false;
        }

        public bool InPermission(Permission permission)
        {
            return _roles.FirstOrDefault((t) =>
            {
                return t.InPermission(permission);
            }) != null;
        }

        public bool InPermission(string permissionMarkedCode)
        {
            return _roles.FirstOrDefault((t) =>
            {
                return t.InPermission(permissionMarkedCode);
            }) != null;
        }

        /// <summary>
        /// 满足权限之一
        /// </summary>
        /// <param name="roleMarkedCodes"></param>
        /// <returns></returns>
        public bool InPermissionAnyOne(params string[] permissionMarkedCodes)
        {
            return _roles.FirstOrDefault((t) =>
            {
                return t.InPermissionAnyOne(permissionMarkedCodes);
            }) != null;
        }

        public bool Is<T>() where T : class,IIdentityProvider
        {
            var proxy = IdentityRegistrar.GetProxy<T>();
            return InRole(proxy.RoleMC);
        }

        public override bool IsEmpty()
        {
            return _roles.Count == 0;
        }

        public SecurityIdentity AddRoles(IList<Role> roles)
        {
            List<Role> newRoles = _roles.Concat(roles).Distinct().ToList();
            return new SecurityIdentity(newRoles);
        }

        public SecurityIdentity RemoveRole(Role role)
        {
            if (!InRole(role)) throw new DomainDrivenException("无法移除不存在的角色");
            _roles.Remove(role);
            return new SecurityIdentity(_roles);
        }

        public static readonly SecurityIdentity Empty = new SecurityIdentity(new List<Role>());

    }
}
