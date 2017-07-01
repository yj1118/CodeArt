using CodeArt.DomainDriven;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace AccountSubsystem
{
    /// <summary>
    /// 组织有以下特点：
    /// 1.可以为组织分配多个权限，但权限本身是独立的，不隶属于某个组织
    /// 2.组织可以创建多个角色，这些角色是属于该组织的
    /// 3.组织可以为角色分配组织拥有的权限
    /// </summary>
    [ObjectRepository(typeof(IOrganizationRepository))]
    [ObjectValidator(typeof(OrganizationSpecification))]
    public class Organization : AggregateRoot<Organization, Guid>
    {
        private static readonly DomainProperty NameProperty = DomainProperty.Register<string,Organization>("Name");

        /// <summary>
        /// 组织名称，可以为组织起一个名字，但这不是必须的
        /// </summary>
        [PropertyRepository]
        [StringLength(0, 100)]
        public string Name
        {
            get
            {
                return GetValue<string>(NameProperty);
            }
            set
            {
                SetValue(NameProperty, value);
            }
        }


        #region 组织拥有的权限


        private static readonly DomainProperty PermissionsProperty = DomainProperty.RegisterCollection<Permission, Organization>("Permissions");

        /// <summary>
        /// 组织所拥有的权限，可以为组织分配多个权限，但权限本身是独立的，不隶属于某个组织
        /// </summary>
        public IEnumerable<Permission> Permissions
        {
            get
            {
                return PermissionsImpl;
            }
        }

        private DomainCollection<Permission> PermissionsImpl
        {
            get
            {
                return GetValue<DomainCollection<Permission>>(PermissionsProperty);
            }
            set
            {
                SetValue(PermissionsProperty, value);
            }
        }

        internal void SetPermissions(IEnumerable<Permission> items)
        {
            PermissionsImpl = new DomainCollection<Permission>(Organization.PermissionsProperty, items);
        }

        /// <summary>
        /// 判断该组织是否拥有某权限
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool Contains(Permission target)
        {
            return this.Permissions.Contains(target);
        }

        /// <summary>
        /// 判断该组织是否拥有指定的权限集合
        /// </summary>
        /// <param name="targets"></param>
        /// <returns></returns>
        public bool Contains(Permission[] targets)
        {
            foreach (Permission per in targets)
            {
                if (!this.Permissions.Contains(per)) return false;
            }
            return true;
        }

        #endregion

        #region 组织所创建的角色

        /// <summary>
        /// 组织可以创建多个角色，这些角色是属于该组织的
        /// </summary>
        private static readonly DomainProperty RolesProperty = DomainProperty.RegisterCollection<Role, Organization>("Roles");

        [PropertyRepository]
        [List(Max = 100)]
        public IEnumerable<Role> Roles
        {
            get
            {
                return RolesImpl;
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


        private bool ContainsRole(Role role)
        {
            return RolesImpl.FirstOrDefault((item) =>
            {
                return item.Id == role.Id || item.Name.EqualsIgnoreCase(role.Name) || item.MarkedCode.EqualsIgnoreCase(role.MarkedCode);
            }) != null;
        }


        /// <summary>
        /// 该方法仅供role使用，请勿在其他地方使用
        /// </summary>
        /// <param name="role"></param>
        internal void AddRole(Role role)
        {
            if (ContainsRole(role)) throw new DomainDrivenException(string.Format(Strings.RoleInOrganizationRepeated, role.Name));
            RolesImpl.Add(role);
        }

        public Role FindRole(string name)
        {
            var role = RolesImpl.FirstOrDefault((item) =>
            {
                return item.Name.EqualsIgnoreCase(name);
            });
            return role == null ? Role.Empty : role;
        }


        /// <summary>
        /// 删除所有角色，当组织被删除之前，会删除所有的角色
        /// </summary>
        private void DeleteRoles()
        {
            var repository = Repository.Create<IRoleRepository>();
            foreach (var role in RolesImpl)
            {
                repository.Delete(role);
            }
            RolesImpl.Clear();
        }


        #endregion

        [ConstructorRepository]
        public Organization(Guid id)
            : base(id)
        {
            MountEvents();
            this.OnConstructed();
        }

        private void MountEvents()
        {
            this.PreDelete += OnPreDelete;
        }

        private void OnPreDelete(object sender, RepositoryEventArgs e)
        {
            //删除组织之前，需要删除该组织下的所有角色
            DeleteRoles();
        }




        #region 空对象

        private class OrganizationEmpty : Organization
        {
            public OrganizationEmpty()
                : base(Guid.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly Organization Empty = new OrganizationEmpty();

        #endregion


    }
}
