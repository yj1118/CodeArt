using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using CodeArt.AppSetting;
using CodeArt.Runtime;

namespace CodeArt.Selenium
{
    public class SeleniumConfiguration : IConfigurationSectionHandler
    {
        public DriverConfig DriverConfig { get; private set; }

        internal SeleniumConfiguration()
        {
            this.DriverConfig = new DriverConfig();
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            SeleniumConfiguration config = new SeleniumConfiguration();
            config.DriverConfig = DeserializeDriverConfig(section);
            return config;
        }

        private DriverConfig DeserializeDriverConfig(XmlNode root)
        {
            var config = new DriverConfig();
            config.LoadFrom(root);
            return config;
        }

        #region 全局配置

        /// <summary>
        /// 全局配置信息
        /// </summary>
        internal static readonly SeleniumConfiguration Current;

        static SeleniumConfiguration()
        {
            Current = Configuration.GetSection("codeArt.selenium", () => new SeleniumConfiguration()) as SeleniumConfiguration;
            if (Current == null)
                Current = new SeleniumConfiguration();
        }

        #endregion

    }
}
