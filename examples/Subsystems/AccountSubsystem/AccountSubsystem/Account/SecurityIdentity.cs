using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    [ObjectRepository(typeof(IAccountRepository))]
    public class SecurityIdentity : EntityObject<SecurityIdentity,Guid>
    {
        [PropertyRepository()]
        [List(Max = 30)]
        private static readonly DomainProperty RolesProperty = DomainProperty.RegisterCollection<Role, Account>("Roles");

        /// <summary>
        /// 为账号分配的角色
        /// </summary>
        public IEnumerable<Role> Roles
        {
            get
            {
                return RolesImpl;
            }
            set
            {
                this.RolesImpl = new DomainCollection<Role>(RolesProperty, value);
            }
        }

        private DomainCollection<Role> RolesImpl
        {
            get
            {
                return GetValue<DomainCollection<Role>>(RolesProperty);
            }
            set
            {
                SetValue(RolesProperty, value);
            }
        }


        public void AssignRole(Role role)
        {
            if (this.RolesImpl.Contains(role)) return;
            this.RolesImpl.Add(role);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="isOverride">是否覆盖已有的角色</param>
        public void AssignRoles(IEnumerable<Role> roles)
        {
            foreach (var role in roles)
            {
                this.AssignRole(role);
            }
        }

        public void RemoveRole(Role role)
        {
            this.RolesImpl.Remove(role);
        }

        [ConstructorRepository()]
        internal SecurityIdentity(Guid id)
            : base(id)
        {
        }

        public bool In(Role role)
        {
            return this.RolesImpl.FirstOrDefault((t) =>
            {
                return t.Equals(role);
            }) != null;
        }

        public bool In(string roleMarkedCode)
        {
            return this.RolesImpl.FirstOrDefault((t) =>
            {
                return t.MarkedCode.EqualsIgnoreCase(roleMarkedCode);
            }) != null;
        }

        /// <summary>
        /// 满足角色之一
        /// </summary>
        /// <param name="roleMarkedCodes"></param>
        /// <returns></returns>
        public bool InAnyOne(params string[] roleMarkedCodes)
        {
            foreach(var markedCode in roleMarkedCodes)
            {
                if (In(markedCode)) return true;
            }
            return false;
        }

        public bool Contains(Permission permission)
        {
            return this.RolesImpl.FirstOrDefault((t) =>
            {
                return t.InPermission(permission);
            }) != null;
        }

        public bool Contains(string permissionMarkedCode)
        {
            return this.RolesImpl.FirstOrDefault((t) =>
            {
                return t.InPermission(permissionMarkedCode);
            }) != null;
        }

        /// <summary>
        /// 满足权限之一
        /// </summary>
        /// <param name="roleMarkedCodes"></param>
        /// <returns></returns>
        public bool ContainsAnyOne(params string[] permissionMarkedCodes)
        {
            return this.RolesImpl.FirstOrDefault((t) =>
            {
                return t.InPermissionAnyOne(permissionMarkedCodes);
            }) != null;
        }

        #region 空对象

        private class SecurityIdentityEmpty : SecurityIdentity
        {
            public SecurityIdentityEmpty()
                : base(Guid.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly SecurityIdentity Empty = new SecurityIdentityEmpty();

        #endregion

    }
}
