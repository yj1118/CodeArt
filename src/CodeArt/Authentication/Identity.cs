using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace CodeArt.Authentication
{
    /// <summary>
    /// 关于身份验证等机制在后续版本中提供
    /// </summary>
    public class Identity
    {

        /// <summary>
        /// 身份名
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

        public Identity(string name, string password)
        {
            this.Name = name;
            this.Password = password;
        }

        public readonly static Identity Empty = new Identity(string.Empty, string.Empty);
    }
}
