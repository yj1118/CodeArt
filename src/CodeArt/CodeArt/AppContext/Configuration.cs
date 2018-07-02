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

namespace CodeArt
{
    public class Configuration : IConfigurationSectionHandler
    {
        public AppSettingConfig AppSetting { get; private set; }

        /// <summary>
        /// 身份名称
        /// </summary>
        public string IdentityName
        {
            get;
            private set;
        }

        /// <summary>
        /// 通过配置文件配置的语言选项
        /// </summary>
        public string Language { get; private set; }

        internal Configuration()
        {
            this.AppSetting = AppSettingConfig.Default;
            this.Language = string.Empty;
            this.IdentityName = string.Empty;
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            Configuration config = new Configuration();
            config.AppSetting = DeserializeAppSettingConfig(section);
            config.IdentityName = section.GetAttributeValue("identityName", string.Empty);
            config.Language = section.GetAttributeValue("language", string.Empty);
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
