using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using CodeArt.AppSetting;
using CodeArt.Authentication;

namespace CodeArt
{
    public class Configuration : IConfigurationSectionHandler
    {
        public AppSettingConfig AppSetting { get; private set; }

        public AuthenticationConfig Authentication { get; private set; }

        internal Configuration()
        {
            this.AppSetting = AppSettingConfig.Default;
            this.Authentication = AuthenticationConfig.Default;
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            Configuration config = new Configuration();
            config.AppSetting = DeserializeAppSettingConfig(section);
            config.Authentication = DeserializeAuthenticationConfig(section);
            return config;
        }

        private AppSettingConfig DeserializeAppSettingConfig(XmlNode root)
        {
            var section = root.SelectSingleNode("appSetting");
            if (section == null) return AppSettingConfig.Default;

            var config = new AppSettingConfig();
            config.LoadFrom(section);
            return config;
        }

        private AuthenticationConfig DeserializeAuthenticationConfig(XmlNode root)
        {
            var section = root.SelectSingleNode("authentication");
            if (section == null) return AuthenticationConfig.Default;

            var config = new AuthenticationConfig();
            config.LoadFrom(section);
            return config;
        }

        #region 全局配置

        /// <summary>
        /// 全局配置信息
        /// </summary>
        public static readonly Configuration Current;

        static Configuration()
        {
            Current = ConfigurationManager.GetSection("codeArt") as Configuration;
            if (Current == null) Current = new Configuration();
        }

        #endregion
    }
}
