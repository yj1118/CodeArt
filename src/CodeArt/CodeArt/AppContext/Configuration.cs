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
using CodeArt.Log;
using CodeArt.Security;

using CodeArt.Caching.Redis;

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

        internal CacheConfig CacheConfig
        {
            get;
            private set;
        }

        #region 日志


        public LogConfig LogConfig { get; private set; }

        private LogConfig DeserializeLog(XmlNode root)
        {
            LogConfig config = new LogConfig();
            config.LoadFrom(root);
            return config;
        }

        #endregion

        #region 安全性


        public SecurityConfig SecurityConfig { get; private set; }

        private SecurityConfig DeserializeSecurity(XmlNode root)
        {
            SecurityConfig config = new SecurityConfig();
            config.LoadFrom(root);
            return config;
        }

        #endregion


        internal Configuration()
        {
            this.AppSetting = AppSettingConfig.Default;
            this.Language = string.Empty;
            this.IdentityName = string.Empty;
            this.CacheConfig = CacheConfig.Default;
            this.LogConfig = LogConfig.Default;
            this.SecurityConfig = SecurityConfig.Default;
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            Configuration config = new Configuration();
            config.AppSetting = DeserializeAppSettingConfig(section);
            config.IdentityName = section.GetAttributeValue("identityName", string.Empty);
            config.Language = section.GetAttributeValue("language", string.Empty);
            config.CacheConfig = DeserializeCacheConfig(section);
            config.LogConfig = DeserializeLog(section);
            config.SecurityConfig = DeserializeSecurity(section);
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

        private CacheConfig DeserializeCacheConfig(XmlNode root)
        {
            var section = root.SelectSingleNode("cache");
            if (section == null) return CacheConfig.Default;

            var config = new CacheConfig();
            config.LoadFrom(section);
            return config;
        }

        private CacheConfig DeserializeLogConfig(XmlNode root)
        {
            var section = root.SelectSingleNode("log");
            if (section == null) return CacheConfig.Default;

            var config = new CacheConfig();
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


        #region 帮助方法

        public static object GetSection(string sectionName,Func<IConfigurationSectionHandler> getInstance)
        {
            var section = ConfigurationManager.GetSection(sectionName);
            if (section == null)
            {
                ExeConfigurationFileMap map = new ExeConfigurationFileMap();
                map.ExeConfigFilename = Path.Combine(AppContext.ProcessDirectory, "App.config");
                var config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
                section = config.GetSection(sectionName);
                var cs = section as ConfigurationSection;
                if (cs != null)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(cs.SectionInformation.GetRawXml());
                    var instance = getInstance();
                    return instance.Create(null, null, doc.ChildNodes[0]);
                }
            }
            return section;

        }

        #endregion

    }
}
