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
    public class ServiceModelConfiguration : IConfigurationSectionHandler
    {
        public ServiceModelConfiguration()
        {
            this.Client = new ClientConfig();
            this.Server = new ServerConfig();
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            this.Client = DeserializeClient(section);
            this.Server = DeserializeServer(section);
            return this;
        }

        #region 客户端

        public ClientConfig Client { get; private set; }

        private ClientConfig DeserializeClient(XmlNode root)
        {
            var config = new ClientConfig();
            var clientNode = root.SelectSingleNode("client");
            if (clientNode == null) return config;

            config.LoadFrom(clientNode);
            return config;
        }

        

        #endregion

        #region 服务器端

        public ServerConfig Server { get; private set; }

        private ServerConfig DeserializeServer(XmlNode root)
        {
            ServerConfig config = new ServerConfig();
            var serverNode = root.SelectSingleNode("server");
            if (serverNode == null) return config;

            config.LoadFrom(serverNode);
            return config;
        }

       

        #endregion

        #region 当前配置

        public static ServiceModelConfiguration Current;
        static ServiceModelConfiguration()
        {
            Current = Configuration.GetSection("codeArt.serviceModel", () => new ServiceModelConfiguration()) as ServiceModelConfiguration;
            if (Current == null) Current = new ServiceModelConfiguration();
        }

        #endregion
    }
}
