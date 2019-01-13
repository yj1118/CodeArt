using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using CodeArt.AppSetting;
using CodeArt.Runtime;
using CodeArt.Util;




namespace CodeArt.Search
{
    public class SearchConfiguration : IConfigurationSectionHandler
    {
        private Dictionary<string, string> _servers;


        public string GetServer(string name)
        {
            return _servers[name];
        }

        public string GetDefatultServer()
        {
            return this.GetServer("default");
        }


        //public PolicyGroupConfig PolicyGroupConfig { get; private set; }

        internal SearchConfiguration()
        {
            //this.PolicyGroupConfig = new PolicyGroupConfig();
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            SearchConfiguration config = new SearchConfiguration();
            config._servers = DeserializeServers(section);
            return config;
        }

        private Dictionary<string, string> DeserializeServers(XmlNode root)
        {
            Dictionary<string, string> servers = new Dictionary<string, string>();

            var nodes = root.SelectNodes("servers/add");
            foreach(XmlNode node in nodes)
            {
                string name = node.GetAttributeValue("name");
                string value = node.GetAttributeValue("value");
                servers.Add(name, value);
            }

            return servers;
        }

        #region 全局配置

        /// <summary>
        /// 全局配置信息
        /// </summary>
        internal static readonly SearchConfiguration Current;

        static SearchConfiguration()
        {
            Current = ConfigurationManager.GetSection("codeArt.search") as SearchConfiguration;
            if (Current == null) Current = new SearchConfiguration();
        }

        #endregion

    }
}
