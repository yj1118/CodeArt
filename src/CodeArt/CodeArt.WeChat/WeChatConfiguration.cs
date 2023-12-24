using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using CodeArt.AppSetting;
using CodeArt.Util;
using CodeArt.DTO;

namespace CodeArt.WeChat
{
    public class WeChatConfiguration : IConfigurationSectionHandler
    {
        /// <summary>
        /// 微信公众号的编号
        /// </summary>
        public string MPAppId
        {
            get;
            private set;
        }

        /// <summary>
        /// 开发者密码，
        /// 开发者密码是校验公众号开发者身份的密码，具有极高的安全性。切记勿把密码直接交给第三方开发者或直接存储在代码中。如需第三方代开发公众号，请使用授权方式接入。
        /// </summary>
        public string MPAppSecret
        {
            get;
            private set;
        }

        internal WeChatConfiguration()
        {
            this.MPAppId = string.Empty;
            this.MPAppSecret = string.Empty;
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            WeChatConfiguration config = new WeChatConfiguration();
            DeserializeMP(config,section);
            return config;
        }

        private void DeserializeMP(WeChatConfiguration config,XmlNode root)
        {
            var section = root.SelectSingleNode("mp");
            if (section == null) return;

            config.MPAppId = section.GetAttributeValue("appId", string.Empty);
            config.MPAppSecret = section.GetAttributeValue("appSecret", string.Empty);
        }


        #region 全局配置

        /// <summary>
        /// 全局配置信息
        /// </summary>
        public static readonly WeChatConfiguration Current;

        static WeChatConfiguration()
        {
            Current = ConfigurationManager.GetSection("codeArt.weChat") as WeChatConfiguration;
            if (Current == null) Current = new WeChatConfiguration();
        }

        #endregion
    }
}
