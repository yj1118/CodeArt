using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

namespace AccountSubsystem
{
    /// <summary>
    /// 账号的系统状态
    /// </summary>
    [ObjectRepository(typeof(IAccountRepository))]
    public class AccountStatus : EntityObject<AccountStatus,Guid>
    {
        [PropertyRepository()]
        private static readonly DomainProperty IsEnabledProperty = DomainProperty.Register<bool, AccountStatus>("IsEnabled", true);

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return GetValue<bool>(IsEnabledProperty);
            }
            set
            {
                SetValue(IsEnabledProperty, value);
            }
        }

        [PropertyRepository()]
        private static readonly DomainProperty CreateTimeProperty = DomainProperty.Register<DateTime, AccountStatus>("CreateTime");

        /// <summary>
        /// 账号创建的时间
        /// </summary>
        public DateTime CreateTime
        {
            get
            {
                return GetValue<DateTime>(CreateTimeProperty);
            }
            private set
            {
                SetValue(CreateTimeProperty, value);
            }
        }

        /// <summary>
        /// 登录信息
        /// </summary>
        [PropertyRepository()]
        private static readonly DomainProperty LoginInfoProperty = DomainProperty.Register<LoginInfo, AccountStatus>("LoginInfo");

        
        public LoginInfo LoginInfo
        {
            get
            {
                return GetValue<LoginInfo>(LoginInfoProperty);
            }
            private set
            {
                SetValue(LoginInfoProperty, value);
            }
        }

        /// <summary>
        /// 更新登录信息
        /// </summary>
        /// <param name="lastIp"></param>
        public void UpdateLogin(string lastIp)
        {
            this.LoginInfo = this.LoginInfo.Update(lastIp);
        }

        internal AccountStatus(Guid id, LoginInfo loginInfo)
            : base(id)
        {
            this.CreateTime = DateTime.Now;
            this.LoginInfo = loginInfo;
            this.IsEnabled = true;
            this.OnConstructed();
        }

        [ConstructorRepository()]
        internal AccountStatus(Guid id, DateTime createTime, LoginInfo loginInfo)
            : base(id)
        {
            this.CreateTime = createTime;
            this.LoginInfo = loginInfo;
            this.OnConstructed();
        }

        #region 空对象

        private class AccountStatusEmpty : AccountStatus
        {
            public AccountStatusEmpty()
                : base(Guid.Empty, LoginInfo.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly AccountStatus Empty = new AccountStatusEmpty();

        #endregion

    }
}
