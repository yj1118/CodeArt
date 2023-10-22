using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

using CodeArt.AppSetting;
using CodeArt.Util;

namespace CodeArt.Security
{
	public class SecurityConfig
    {
        /// <summary>
        /// 密钥
        /// </summary>
        public string Secret
        {
            get;
            set;
        }

        /// <summary>
        /// 失效时间（小时）
        /// </summary>
        public float Expired
        {
            get;
            private set;
        }


        /// <summary>
        /// 刷新时间（小时）
        /// </summary>
        public float Refresh
        {
            get;
            private set;
        }

        public SecurityConfig()
        {
        }

        /// <summary>
        /// token 失效时间(小时)，默认24小时
        /// </summary>
        private const float DefaultExpireTime = 24F;

        internal void LoadFrom(XmlNode root)
        {
            var section = root.SelectSingleNode("security");
            if (section == null) return;

            var node = section.SelectSingleNode("token");
            if (node == null) return;

            var secret = node.Attributes["secret"].Value;
            var expired = node.Attributes["expired"].Value;
            if (string.IsNullOrEmpty(secret))
                throw new NoTypeDefinedException(secret);

            this.Secret = secret;
            this.Expired = string.IsNullOrEmpty(expired) ? DefaultExpireTime : float.Parse(expired);
            this.Refresh = this.Expired / 3; //过期时间的三分之一则为令牌的刷新时间
        }

        internal static readonly SecurityConfig Default = new SecurityConfig();

    }
}
