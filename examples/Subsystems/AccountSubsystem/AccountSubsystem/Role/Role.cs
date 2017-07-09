using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using CodeArt.DomainDriven;
using CodeArt.Util;

namespace AccountSubsystem
{
    [ObjectRepository(typeof(IRoleRepository))]
    [ObjectValidator(typeof(RoleSpecification))]
    public class Role : AggregateRoot<Role, Guid>
    {

        #region 所属的组织

        private static readonly DomainProperty OrganizationProperty = DomainProperty.Register<Organization, Role>("Organization");

        /// <summary>
        /// 角色隶属的组织，该属于可以为空，意味着角色不属于任何组织
        /// </summary>
        [PropertyRepository(Lazy = true)]
        [PropertyChanged("OnOrganizationChanged")]
        public Organization Organization
        {
            get
            {
                return GetValue<Organization>(OrganizationProperty);
            }
            private set
            {
                SetValue(OrganizationProperty, value);
            }
        }

        protected virtual void OnOrganizationChanged(DomainPropertyChangedEventArgs e)
        {
            var old = e.OldValue as Organization;
            if (!old.IsEmpty()) throw new DomainDrivenException(string.Format(Strings.NotChangeOrganization, this.Name));
            var org = e.NewValue as Organization;
            if (!org.IsEmpty()) org.AddRole(this);
        }


        #endregion


        /// <summary>
        /// 角色名称
        /// </summary>
        private static readonly DomainProperty NameProperty = DomainProperty.Register<string, Role>("Name");

        [PropertyRepository()]
        [NotEmpty()]
        [StringLength(2, 25)]
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

        /// <summary>
        /// 唯一标识符
        /// </summary>
        private static readonly DomainProperty MarkedCodeProperty = DomainProperty.Register<string, Role>("MarkedCode");

        [PropertyRepository()]
        [StringLength(0, 50)]
        public string MarkedCode
        {
            get
            {
                return GetValue<string>(MarkedCodeProperty);
            }
            set
            {
                SetValue(MarkedCodeProperty, value);
            }
        }

        public bool DeclareMarkedCode
        {
            get
            {
                return !string.IsNullOrEmpty(this.MarkedCode);
            }
        }

        /// <summary>
        /// 描述
        /// </summary>
        private static readonly DomainProperty DescriptionProperty = DomainProperty.Register<string, Role>("Description");

        [PropertyRepository()]
        [StringLength(0, 200)]
        public string Description
        {
            get
            {
                return GetValue<string>(DescriptionProperty);
            }
            set
            {
                SetValue(DescriptionProperty, value);
            }
        }

        #region 角色拥有的权限

        private static readonly DomainProperty PermissionsProperty = DomainProperty.RegisterCollection<Permission, Role>("Permissions");

        /// <summary>
        /// 角色拥有的权限
        /// </summary>
        [PropertyRepository(Lazy = true)]
        [NotEmpty()]
        [List(Max = 100)]
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
            PermissionsImpl = new DomainCollection<Permission>(Role.PermissionsProperty, items);
        }


        /// <summary>
        /// 判定角色是否拥有某个权限
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public bool InPermission(Permission permission)
        {
            return this.Permissions.FirstOrDefault((t) =>
            {
                return t.Equals(permission);
            }) != null;
        }

        /// <summary>
        /// 根据权限标示，判定角色是否拥有某个权限
        /// </summary>
        /// <param name="permissionMarkedCode"></param>
        /// <returns></returns>
        public bool InPermission(string permissionMarkedCode)
        {
            return this.Permissions.FirstOrDefault((t) =>
            {
                return t.MarkedCode.EqualsIgnoreCase(permissionMarkedCode);
            }) != null;
        }

        /// <summary>
        /// 判定角色是否拥有指定权限集合的权限之一
        /// </summary>
        /// <param name="roleMarkedCodes"></param>
        /// <returns></returns>
        public bool InPermissionAnyOne(params string[] permissionMarkedCodes)
        {
            foreach (var markedCode in permissionMarkedCodes)
            {
                if (InPermission(markedCode)) return true;
            }
            return false;
        }

        #endregion

        /// <summary>
        /// 是否为系统角色
        /// </summary>
        private static readonly DomainProperty IsSystemProperty = DomainProperty.Register<bool, Role>("IsSystem");


        /// <summary>
        /// 是否为系统角色，系统角色经常会与定制的项目协同工作，这种角色是不能被随便删除的
        /// </summary>
        [PropertyRepository]
        public bool IsSystem
        {
            get
            {
                return GetValue<bool>(IsSystemProperty);
            }
            set
            {
                SetValue(IsSystemProperty, value);
            }
        }

        public Role(Guid id, Organization organization)
            : base(id)
        {
            this.Organization = organization;
            this.OnConstructed();
        }


        [ConstructorRepository()]
        public Role(Guid id)
            : base(id)
        {
            this.OnConstructed();
        }

        private class RoleEmpty : Role
        {
            public RoleEmpty()
                : base(Guid.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }

        }

        public static readonly Role Empty = new RoleEmpty();
    }
}
