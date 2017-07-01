using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    /// <summary>
    /// 权限对象
    /// </summary>
    [ObjectRepository(typeof(IPermissionRepository))]
    [ObjectValidator(typeof(PermissionSpecification))]
    public class Permission : AggregateRoot<Permission, Guid>
    {
        internal static readonly DomainProperty NameProperty = DomainProperty.Register<string, Permission>("Name");

        /// <summary>
        /// 权限名称
        /// </summary>
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
        /// <para>权限的唯一标示，可以由用户设置</para>
        /// <para>可以通过唯一标示找到权限对象</para>
        /// <para>该属性可以为空</para>
        /// </summary>
        internal static readonly DomainProperty MarkedCodeProperty = DomainProperty.Register<string, Permission>("MarkedCode");

        /// <summary>
        /// 标识码，可以为空
        /// </summary>
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

        /// <summary>
        /// 是否定义了标识码
        /// </summary>
        public bool DeclareMarkedCode
        {
            get
            {
                return !string.IsNullOrEmpty(this.MarkedCode);
            }
        }


        private static readonly DomainProperty DescriptionProperty = DomainProperty.Register<string, Permission>("Description");

        /// <summary>
        /// <para>描述</para>
        /// </summary>
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

        [ConstructorRepository()]
        public Permission(Guid id)
            : base(id)
        {
            this.OnConstructed();
        }

        #region 空对象

        private class PermissionEmpty : Permission
        {
            public PermissionEmpty()
                : base(Guid.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static Permission Empty = new PermissionEmpty();

        #endregion
    }
}