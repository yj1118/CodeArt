using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    /// <summary>
    /// </summary>
    [ObjectValidator(typeof(AccountSpecification))]
    public class Account : AggregateRoot<Account, Guid>
    {

        #region 领域属性

        public static readonly DomainProperty NameProperty = null;
        public static readonly DomainProperty EmailProperty = null;
        public static readonly DomainProperty MobileNumberProperty = null;
        public static readonly DomainProperty PasswordProperty = null;
        public static readonly DomainProperty IsEnabledProperty = null;
        public static readonly DomainProperty CreateTimeProperty = null;

        /// <summary>
        /// 登录信息
        /// </summary>
        public static readonly DomainProperty LoginsProperty = null;

        public static readonly DomainProperty SIDProperty = null;

        public static Account Empty;

        static Account()
        {
            NameProperty = DomainProperty.RegisterString<Account>("Name");
            EmailProperty = DomainProperty.RegisterString<Account>("Email");
            MobileNumberProperty = DomainProperty.RegisterString<Account>("MobileNumber");
            PasswordProperty = DomainProperty.RegisterString<Account>("Password");
            IsEnabledProperty = DomainProperty.Register<bool, Account>("IsEnabled");
            CreateTimeProperty = DomainProperty.Register<DateTime, Account>("CreateTime");
            LoginsProperty = DomainProperty.Register<LoginInfo, Account>("Logins", LoginInfo.Empty);

            SIDProperty = DomainProperty.Register<SecurityIdentity, Account>("SID", SecurityIdentity.Empty);

            Empty = new Account(Guid.Empty, SecurityLog.Empty);

        }

        #endregion

        #region 账户flag

        /// <summary>
        /// 账户名
        /// </summary>
        [NotEmpty()]
        [StringLengthValidator(2, 25)]
        [StringFormatValidator(StringFormat.Chinese | StringFormat.Letter | StringFormat.Number, "_")]
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
        /// 电子邮箱
        /// </summary>
        [Email()]
        public string Email
        {
            get
            {
                return GetValue<string>(EmailProperty);
            }
            set
            {
                SetValue(EmailProperty, value);
            }
        }

        /// <summary>
        /// 手机号码
        /// </summary>
        [MobileValidator()]
        public string MobileNumber
        {
            get
            {
                return GetValue<string>(MobileNumberProperty);
            }
            set
            {
                SetValue(MobileNumberProperty, value);
            }
        }

        #endregion

        /// <summary>
        /// 密码
        /// </summary>
        [NotNullValidator()]
        [StringLengthValidator(6, 25)]
        public string Password
        {
            get
            {
                return GetValue<string>(PasswordProperty);
            }
            set
            {
                SetValue(PasswordProperty, value);
            }
        }

        private bool _isEnabled = false;
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return GetValue<bool>(IsEnabledProperty, _isEnabled);
            }
            set
            {
                SetValue(IsEnabledProperty, ref _isEnabled, value);
            }
        }

        private DateTime _createTime = DateTime.Now;

        public DateTime CreateTime
        {
            get
            {
                return GetValue<DateTime>(CreateTimeProperty, _createTime);
            }
            set
            {
                SetValue(CreateTimeProperty, ref _createTime, value);
            }
        }

        #region 安全级别

        /// <summary>
        /// 安全级别
        /// </summary>
        public float SecurityLevel
        {
            get
            {
                return SLCRegistrar.Create().Calculate(this);
            }
        }

        public bool HavePassword
        {
            get
            {
                return (!string.IsNullOrEmpty(Password));
            }
        }

        public bool HaveMobile
        {
            get
            {
                return (!string.IsNullOrEmpty(MobileNumber));
            }
        }

        public bool HaveEmail
        {
            get
            {
                return (!string.IsNullOrEmpty(Email));
            }
        }

        #endregion

        #region 登录信息

        public SecurityLog Log
        {
            get
            {
                return GetValue<SecurityLog>(LogProperty);
            }
            private set
            {
                SetValue(LogProperty, value);
            }
        }


        /// <summary>
        /// 更新日志
        /// </summary>
        /// <param name="lastIp"></param>
        public void UpdateLog(string lastIp, bool isNew = false)
        {
            if (isNew)
                this.Log = new SecurityLog(lastIp, DateTime.Now, 0);
            else
                this.Log = new SecurityLog(lastIp, DateTime.Now, Log.Total + 1);
        }

        #endregion

        #region 安全

        /// <summary>
        ///  安全身份
        /// </summary>
        public SecurityIdentity SID
        {
            get
            {
                return GetValue<SecurityIdentity>(SIDProperty);
            }
            set
            {
                SetValue(SIDProperty, value);
            }
        }

        public Role[] GetRoles()
        {
            return this.SID.GetRoles();
        }

        public bool InRole(Role role)
        {
            return this.SID.InRole(role);
        }

        public bool InRole(string roleMarkedCode)
        {
            return this.SID.InRole(roleMarkedCode);
        }

        public bool InRoleAnyOne(params string[] roleMarkedCodes)
        {
            return this.SID.InRoleAnyOne(roleMarkedCodes);
        }

        public bool InPermission(Permission permission)
        {
            return this.SID.InPermission(permission);
        }

        public bool InPermission(string permissionMarkedCode)
        {
            return this.SID.InPermission(permissionMarkedCode);
        }

        /// <summary>
        /// 满足权限之一
        /// </summary>
        /// <param name="roleMarkedCodes"></param>
        /// <returns></returns>
        public bool InPermissionAnyOne(params string[] permissionMarkedCodes)
        {
            return this.SID.InPermissionAnyOne(permissionMarkedCodes);
        }

        public bool Is<T>() where T : class, IIdentityProvider
        {
            return this.SID.Is<T>();
        }

        public void AssignRole(Role role)
        {
            var roles = this.GetRoles().ToList();
            int index = roles.FindIndex(o => o.Equals(role));
            if (index > -1) roles[index] = role;
            else roles.Add(role);
            this.SID = new SecurityIdentity(roles);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="isOverride">是否覆盖已有的角色</param>
        public void AssignRole(IList<Role> roles)
        {
            this.SID = new SecurityIdentity(roles);
        }

        public void RemoveRole(Role role)
        {
            var roles = this.GetRoles().ToList();
            int index = roles.FindIndex(o => o.Equals(role));
            if (index > -1) roles.RemoveAt(index);
            this.SID = new SecurityIdentity(roles);
        }

        #endregion

        public Account(Guid id, SecurityLog log)
            : base(id)
        {
            Log = log;
        }

        public Account(Guid id, SecurityLog log, IDataProxy dataProxy)
            : base(id)
        {
            Log = log;
            this.DataProxy = dataProxy;
        }

        public override bool IsEmpty()
        {
            return this.Equals(Account.Empty);
        }

        /// <summary>
        /// 是否缺失实体对象信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool IsIncomplete<T>() where T : IIdentityProvider
        {
            return IsIncomplete(typeof(T));
        }

        /// <summary>
        /// 是否缺失实体对象信息,即：账户拥有角色，但是确实对应的实体对象信息
        /// </summary>
        /// <param name="identityType"></param>
        /// <returns></returns>
        public bool IsIncomplete(Type identityType)
        {
            var proxy = IdentityRegistrar.GetProxy(identityType);
            return this.InRole(proxy.RoleMC) && proxy.Detect(this).IsEmpty();
        }

        /// <summary>
        /// 获取缺失实体对象信息的角色列表
        /// </summary>
        /// <returns></returns>
        public string[] GetIncompletes()
        {
            List<string> incompleteRoleMCs = new List<string>();
            var identityTypes = IdentityRegistrar.GetIdentityTypes();
            foreach (var identityType in identityTypes)
            {
                if (this.IsIncomplete(identityType))
                {
                    var proxy = IdentityRegistrar.GetProxy(identityType);
                    incompleteRoleMCs.Add(proxy.RoleMC);
                }
            }
            return incompleteRoleMCs.ToArray();
        }

        public void CheckIncomplete<T>() where T : IIdentityProvider
        {
            if (!IsIncomplete<T>()) throw new DomainDrivenException("账号 " + this.Id + " 的 " + typeof(T).FullName + " 信息不全");
        }

        /// <summary>
        /// 是否拥有身份
        /// 即：账户拥有对应的角色，并且实体信息存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool HasIdenity<T>() where T : IIdentityProvider
        {
            Type identityType = typeof(T);
            return HasIdenity(identityType);
        }

        public bool HasIdenity(Type identityType)
        {
            var proxy = IdentityRegistrar.GetProxy(identityType);
            return this.InRole(proxy.RoleMC) && !proxy.Detect(this).IsEmpty();
        }

        /// <summary>
        /// 获取账号拥有的身份对应的角色MC值（即：同时拥有角色和该角色对应的实体对象）
        /// </summary>
        /// <returns></returns>
        public string[] GetIdenities()
        {
            List<string> roleMCs = new List<string>();
            var identityTypes = IdentityRegistrar.GetIdentityTypes();
            foreach (var identityType in identityTypes)
            {
                var proxy = IdentityRegistrar.GetProxy(identityType);
                if (this.InRole(proxy.RoleMC) && !proxy.Detect(this).IsEmpty())
                    roleMCs.Add(proxy.RoleMC);
            }
            return roleMCs.ToArray();
        }

        /// <summary>
        /// 获取身份
        /// </summary>
        /// <param name="roleMC"></param>
        /// <returns></returns>
        public IIdentityProvider GetIdenity(string roleMC)
        {
            if (!this.InRole(roleMC)) return IdentityEmpty.Instance;
            var proxy = IdentityRegistrar.GetProxy(roleMC);
            return proxy.Detect(this);
        }

        /// <summary>
        /// 获取身份
        /// </summary>
        /// <param name="roleMC"></param>
        /// <returns></returns>
        public T GetIdenity<T>() where T : IIdentityProvider
        {
            Type identityType = typeof(T);
            var proxy = IdentityRegistrar.GetProxy(identityType);
            if (!this.InRole(proxy.RoleMC)) return default(T);
            return (T)proxy.Detect(this);
        }

        #region 验证是否能申请成为某个身份

        /// <summary>
        /// 验证账号是否能成为一个身份（即发放的牌照是否有效）
        /// </summary>
        /// <param name="identityType"></param>
        public void CheckLicense(Type identityType)
        {
            var proxy = IdentityRegistrar.GetProxy(identityType);
            var license = proxy.GetLicense(this);
            if (!license.IsValid) throw new IdentityLicenseException(license.Message);
            if (this.HasIdenity(identityType)) throw new IdentityLicenseException("identityRepeated");
            if (this.IsAppling(identityType)) throw new IdentityLicenseException("identityApplyRepeated");
        }

        public void CheckLicense(string identityMC)
        {
            var proxy = IdentityRegistrar.GetProxy(identityMC);
            CheckLicense(proxy.IdentityType);
        }

        /// <summary>
        /// 是否是否正在申请中(审批中)
        /// </summary>
        /// <param name="identityType"></param>
        /// <returns></returns>
        public bool IsAppling(Type identityType)
        {
            var proxy = IdentityRegistrar.GetProxy(identityType);
            return !this.InRole(proxy.RoleMC) && !proxy.Detect(this).IsEmpty(); //当账户不具备该角色，但是已经有对应的实体对象，则表示正在申请中
        }



        #endregion

        ///// <summary>
        ///// 判断账户能否补充某个身份的信息
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <returns></returns>
        //public bool CanComplement<T>() where T : IIdentityProvider
        //{
        //    return this.IsIncomplete<T>();
        //}

        //public bool CanComplement(Type identityType)
        //{
        //    return this.IsIncomplete(identityType);
        //}

        //public void CheckComplement<T>() where T : IIdentityProvider
        //{
        //    if (!this.CanComplement<T>())
        //        throw new DomainDrivenException(string.Format("帐号 {0} 无法补全身份信息 {1}", this.Name, typeof(T).FullName));
        //}

        ///// <summary>
        ///// 判断账户能否申请成为某个身份
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <returns></returns>
        //public bool CanApply<T>() where T : IIdentityProvider
        //{
        //    Type identityType = typeof(T);
        //    return CanApply(identityType);
        //}

        //public bool CanApply(Type identityType)
        //{
        //    var proxy = IdentityRegistrar.GetProxy(identityType);

        //    return !this.InRole(proxy.RoleMC) && proxy.Detect(this).IsEmpty(); //当账户不具备该角色，并且没有对应的实体信息时才能申请成为某个身份
        //}

        //public void CheckApply<T>() where T : IIdentityProvider
        //{
        //    if (!this.CanApply<T>())
        //        throw new DomainDrivenException(string.Format("帐号 {0} 无法申请身份 {1}", this.Name, typeof(T).FullName));
        //}

        //public bool CanApply(string roleMC)
        //{
        //    var proxy = IdentityRegistrar.GetProxy(roleMC);
        //    return !this.InRole(proxy.RoleMC) && proxy.Detect(this).IsEmpty(); //当账户不具备该角色，并且没有对应的实体信息时才能申请成为某个身份
        //}

    }
}
