using System;

using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    /// <summary>
    /// 登录信息
    /// </summary>
    public class LoginInfo : ValueObject
    {
        #region 领域属性

        /// <summary>
        /// 最后一次登录时使用的IP地址
        /// </summary>
        public static readonly DomainProperty LastIPProperty = null;

        /// <summary>
        /// 最后一次登录的时间
        /// </summary>
        public static readonly DomainProperty LastTimeProperty = null;

        /// <summary>
        /// 总登录次数
        /// </summary>
        public static readonly DomainProperty TotalProperty = null;

        static LoginInfo()
        {
            LastIPProperty = DomainProperty.RegisterString<Permission>("LastIP", PropertyAccessLevel.Private);
            LastTimeProperty = DomainProperty.RegisterString<Permission>("LastTime", PropertyAccessLevel.Private);
            TotalProperty = DomainProperty.Register<int, Permission>("Total", PropertyAccessLevel.Private);
        }

        #endregion

        #region 空对象

        public static LoginInfo Empty;

        private class LoginInfoEmpty : LoginInfo
        {
            public LoginInfoEmpty()
                : base(string.Empty, default(DateTime), 0)
            {
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        #endregion


        /// <summary>
        /// 登录时使用的IP地址
        /// </summary>
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

        private DateTime _lastTime;

        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime LastTime
        {
            get
            {
                return GetValue<DateTime>(LastTimeProperty, _lastTime);
            }
            private set
            {
                SetValue<DateTime>(LastTimeProperty, ref _lastTime, value);
            }
        }

        private int _total;

        /// <summary>
        /// 登录时间
        /// </summary>
        public int Total
        {
            get
            {
                return GetValue<int>(TotalProperty, _total);
            }
            private set
            {
                SetValue<int>(TotalProperty, ref _total, value);
            }
        }

        public LoginInfo(string lastIP, DateTime lastTime, int total)
        {
            this.LastIP = lastIP;
            this.LastTime = lastTime;
            this.Total = total;

            this.OnConstructed();
        }
    }
}
