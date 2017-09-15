using System;

using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    /// <summary>
    /// 登录信息
    /// </summary>
    [ObjectRepository(typeof(IAccountRepository))]
    public class LoginInfo : ValueObject
    {
        /// <summary>
        /// 最后一次登录时使用的IP地址
        /// </summary>
        private static readonly DomainProperty LastIPProperty = DomainProperty.Register<string, Account>("LastIP");

        /// <summary>
        /// 登录时使用的IP地址
        /// </summary>
        [PropertyRepository]
        public string LastIP
        {
            get
            {
                return GetValue<string>(LastIPProperty);
            }
            private set
            {
                SetValue(LastIPProperty, value);
            }
        }


        private static readonly DomainProperty LastTimeProperty = DomainProperty.Register<Emptyable<DateTime>, Account>("LastTime");

        /// <summary>
        /// 最后一次登录时间
        /// </summary>
        [PropertyRepository]
        public Emptyable<DateTime> LastTime
        {
            get
            {
                return GetValue<Emptyable<DateTime>>(LastTimeProperty);
            }
            private set
            {
                SetValue(LastTimeProperty, value);
            }
        }


        /// <summary>
        /// 总登录次数
        /// </summary>
        private static readonly DomainProperty TotalProperty = DomainProperty.Register<int, Permission>("Total");

        /// <summary>
        /// 总登录次数
        /// </summary>
        [PropertyRepository]
        public int Total
        {
            get
            {
                return GetValue<int>(TotalProperty);
            }
            private set
            {
                SetValue(TotalProperty, value);
            }
        }

        /// <summary>
        /// 更新登录信息，登录次数会增加，最后一次登录时间和登录IP会被覆盖
        /// </summary>
        /// <returns></returns>
        public LoginInfo Update(string lastIP)
        {
            if (this.IsEmpty()) return new LoginInfo(lastIP, DateTime.Now, 1);
            return new LoginInfo(lastIP, DateTime.Now, this.Total + 1);
        }

        [ConstructorRepository()]
        public LoginInfo(string lastIP, DateTime lastTime, int total)
        {
            this.LastIP = lastIP;
            this.LastTime = lastTime;
            this.Total = total;
            this.OnConstructed();
        }

        #region 空对象

        private class LoginInfoEmpty : LoginInfo
        {
            public LoginInfoEmpty()
                : base(string.Empty, Emptyable<DateTime>.CreateEmpty(), 0)
            {
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly LoginInfo Empty = new LoginInfoEmpty();

        #endregion
    }
}
