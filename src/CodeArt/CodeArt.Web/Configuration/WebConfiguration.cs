using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using CodeArt.AppSetting;
using CodeArt.Util;
using System.Globalization;

namespace CodeArt.Web
{
    public class WebConfiguration : IConfigurationSectionHandler
    {
        public SessionConfig SessionConfig { get; private set; }


        internal WebConfiguration()
        {
            this.SessionConfig = SessionConfig.Default;
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            WebConfiguration config = new WebConfiguration();
            config.SessionConfig = DeserializeSessionConfig(section);
            return config;
        }

        private SessionConfig DeserializeSessionConfig(XmlNode root)
        {
            var section = root.SelectSingleNode("session");
            if (section == null) return SessionConfig.Default;

            XmlSerializer ser = new XmlSerializer(typeof(SessionConfig));
            var config = ser.Deserialize(new XmlNodeReader(section)) as SessionConfig;
            if (config == null) throw new ConfigurationErrorsException("获取SessionConfig失败");

            config.LoadFrom(section);
            return config;
        }

        #region 全局配置

        /// <summary>
        /// 全局配置信息
        /// </summary>
        public static readonly WebConfiguration Current;

        static WebConfiguration()
        {
            Current = ConfigurationManager.GetSection("codeArt.web") as WebConfiguration;
            if (Current == null) Current = new WebConfiguration();
        }

        #endregion
    }
}
