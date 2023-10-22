using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Configuration;
using System.Xml;

using CodeArt.Runtime;
using CodeArt.AppSetting;
using CodeArt.Util;

namespace CodeArt.EasyMQ.Event
{
    public class RPCConfig
    {
        public RPCConfig()
        {
            this.ClientTimeout = 20;
        }

        public InterfaceImplementer ClientFactoryImplementer { get; set; }

        public InterfaceImplementer ServerFactoryImplementer { get; set; }

        /// <summary>
        /// 客户端请求超时的时间
        /// </summary>
        public int ClientTimeout { get; set; }

        internal void LoadFrom(XmlNode root)
        {
            LoadClient(root);
            LoadServer(root);
        }

        private void LoadClient(XmlNode root)
        {
            var node = root.SelectSingleNode("client");
            if (node == null) return;

            var factoryNode = node.SelectSingleNode("factory");
            if (factoryNode != null) this.ClientFactoryImplementer = InterfaceImplementer.Create(factoryNode);

            int clientTimeout = int.Parse(node.GetAttributeValue("timeout", "20")); //默认20秒超时
            this.ClientTimeout = clientTimeout;
        }

        private void LoadServer(XmlNode root)
        {
            var node = root.SelectSingleNode("server");
            if (node == null) return;

            var factoryNode = node.SelectSingleNode("factory");
            if (factoryNode != null) this.ServerFactoryImplementer = InterfaceImplementer.Create(factoryNode);
        }
    }
}
