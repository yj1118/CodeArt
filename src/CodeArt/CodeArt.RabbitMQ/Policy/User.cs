using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace CodeArt.RabbitMQ
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class User
    {

        /// <summary>
        /// 用户名
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get;
            private set;
        }

        public User(string name, string password)
        {
            this.Name = name;
            this.Password = password;
        }

        public readonly static User Empty = new User(string.Empty, string.Empty);
    }
}
