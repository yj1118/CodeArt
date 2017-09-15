using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using CodeArt.Util;
using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    [ObjectRepository(typeof(IAccountRepository))]
    [ObjectValidator(typeof(AccountSpecification))]
    public class Account : AggregateRoot<Account, Guid>
    {
        [PropertyRepository()]
        [NotEmpty()]
        [StringLength(2, 25)]
        [StringFormat(StringFormat.Chinese | StringFormat.Letter | StringFormat.Number, "_")]
        internal static readonly DomainProperty NameProperty = DomainProperty.Register<string, Account>("Name");

        /// <summary>
        /// 账户名
        /// </summary>
        public string Name
        {
            get
            {
                return GetValue<string>(NameProperty);
            }
            private set
            {
                SetValue(NameProperty, value);
            }
        }

        [PropertyRepository()]
        [Email()]
        internal static readonly DomainProperty EmailProperty = DomainProperty.Register<string, Account>("Email");

        /// <summary>
        /// 电子邮箱
        /// </summary>
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

        [PropertyRepository()]
        private static readonly DomainProperty EmailVerifiedProperty = DomainProperty.Register<bool, Account>("EmailVerified");

        /// <summary>
        /// 邮箱是否已认证
        /// </summary>
        public bool EmailVerified
        {
            get
            {
                return GetValue<bool>(EmailVerifiedProperty);
            }
            set
            {
                SetValue(EmailVerifiedProperty, value);
            }
        }

        [PropertyRepository()]
        [MobileNumber.CN()]
        internal static readonly DomainProperty MobileNumberProperty = DomainProperty.Register<string, Account>("MobileNumber");

        /// <summary>
        /// 手机号码
        /// </summary>
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

        [PropertyRepository()]
        private static readonly DomainProperty MobileVerifiedProperty = DomainProperty.Register<bool, Account>("MobileVerified");

        /// <summary>
        /// 手机号是否已认证
        /// </summary>
        public bool MobileVerified
        {
            get
            {
                return GetValue<bool>(EmailVerifiedProperty);
            }
            set
            {
                SetValue(EmailVerifiedProperty, value);
            }
        }


        [PropertyRepository()]
        [NotEmpty()]
        [StringLength(6, 25)]
        private static readonly DomainProperty PasswordProperty = DomainProperty.Register<string, Account>("Password");

        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get
            {
                return GetValue<string>(PasswordProperty);
            }
            private set
            {
                SetValue(PasswordProperty, value);
            }
        }

        /// <summary>
        /// 设置密码
        /// </summary>
        public void SetPassword(string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                throw new AccountException(Strings.TwicePasswordDifferent);
            }
            this.Password = password;
        }


        #region 账号的安全级别

        /// <summary>
        /// 安全级别
        /// </summary>
        public float SafetyLevel
        {
            get
            {
                return Calculate();
            }
        }

        private float Calculate()
        {
            float level = 0;

            // 登录密码是否设置
            if (this.HasPassword) level = level + 0.25f;

            // 是否绑定了手机号码
            if (this.HaveMobile) level = level + 0.25f;

            // 是否绑定了邮箱
            if (this.HasEmail) level = level + 0.25f;

            return level;
        }



        public bool HasPassword
        {
            get
            {
                return !string.IsNullOrEmpty(Password);
            }
        }

        public bool HaveMobile
        {
            get
            {
                return !string.IsNullOrEmpty(MobileNumber) && this.MobileVerified;
            }
        }

        public bool HasEmail
        {
            get
            {
                return !string.IsNullOrEmpty(Email) && this.EmailVerified;
            }
        }

        #endregion

        [PropertyRepository(Lazy = true)]
        [NotEmpty()]
        private static readonly DomainProperty StatusProperty = DomainProperty.Register<AccountStatus, Account>("Status");

        /// <summary>
        /// 账号的系统状态
        /// </summary>
        public AccountStatus Status
        {
            get
            {
                return GetValue<AccountStatus>(StatusProperty);
            }
            private set
            {
                SetValue(StatusProperty, value);
            }
        }


        [PropertyRepository(Lazy = true)]
        [NotEmpty()]
        private static readonly DomainProperty IdentityProperty = DomainProperty.Register<SecurityIdentity, Account>("Identity");

        /// <summary>
        /// 安全信息
        /// </summary>
        public SecurityIdentity Identity
        {
            get
            {
                return GetValue<SecurityIdentity>(IdentityProperty);
            }
            private set
            {
                SetValue(IdentityProperty, value);
            }
        }

        public Account(string name, string password, IEnumerable<Role> roles)
            : base(Guid.NewGuid())
        {
            this.Name = name;
            this.Password = password;
            this.Identity = new SecurityIdentity(this.Id)
            {
                Roles = roles
            };
            this.Status = new AccountStatus(this.Id, DateTime.Now, LoginInfo.Empty);
            this.OnConstructed();
        }

        [ConstructorRepository]
        internal Account(Guid id, string name, string password)
            : base(id)
        {
            this.Name = name;
            this.Password = password;
            this.OnConstructed();
        }

        #region 空对象

        private class AccountEmpty : Account
        {
            public AccountEmpty()
                : base(string.Empty, string.Empty, Array.Empty<Role>())
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly Account Empty = new AccountEmpty();

        #endregion
    }
}
