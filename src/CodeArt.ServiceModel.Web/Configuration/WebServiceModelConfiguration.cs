using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Linq;

using CodeArt.Util;
using CodeArt.AppSetting;

namespace CodeArt.ServiceModel
{
    public class WebServiceModelConfiguration : IConfigurationSectionHandler
    {
        public WebServiceModelConfiguration()
        {
            this.Client = new WebClientConfig();
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            this.Client = DeserializeClient(section);
            return this;
        }

        #region 客户端

        public WebClientConfig Client { get; private set; }

        private WebClientConfig DeserializeClient(XmlNode root)
        {
            var config = new WebClientConfig();
            var clientNode = root.SelectSingleNode("client");
            if (clientNode == null) return config;

            config.LoadFrom(clientNode);
            return config;
        }

        

        #endregion


        #region 当前配置

        public static WebServiceModelConfiguration Current;
        static WebServiceModelConfiguration()
        {
            Current = ConfigurationManager.GetSection("codeArt.serviceModel.web") as WebServiceModelConfiguration;
            if (Current == null) Current = new WebServiceModelConfiguration();
        }

        #endregion
    }
}
