using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using CodeArt.AppSetting;

namespace CodeArt.Web.WebPages
{
    public class WebPagesConfiguration : IConfigurationSectionHandler
    {
        public WebPageConfig PageConfig { get; private set; }

        internal WebPagesConfiguration()
        {
            this.PageConfig = WebPageConfig.Default;
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            WebPagesConfiguration config = new WebPagesConfiguration();
            config.PageConfig = DeserializePageConfig(section);
            return config;
        }

        private WebPageConfig DeserializePageConfig(XmlNode root)
        {
            XmlSerializer ser = new XmlSerializer(typeof(WebPageConfig));
            var config = ser.Deserialize(new XmlNodeReader(root)) as WebPageConfig;
            if (config == null)
                throw new ConfigurationErrorsException(Strings.FailedGetWebPageConfig);

            config.LoadFrom(root);
            return config;
        }

        #region 全局配置

        /// <summary>
        /// 全局配置信息
        /// </summary>
        public static readonly WebPagesConfiguration Global;

        static WebPagesConfiguration()
        {
            Global = ConfigurationManager.GetSection("codeArt.web.pages") as WebPagesConfiguration;
            if (Global == null) Global = new WebPagesConfiguration();
        }

        #endregion
    }
}
