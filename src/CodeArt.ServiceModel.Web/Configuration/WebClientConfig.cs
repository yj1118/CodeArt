using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Configuration;

using CodeArt.ServiceModel;
using CodeArt.Runtime;
using CodeArt.AppSetting;
using CodeArt.Util;

namespace CodeArt.ServiceModel
{
    public class WebClientConfig
	{
        /// <summary>
        /// 用于远程服务路由的地址,命名空间->路由地址
        /// 可以根据不同的命名空间，定义路由地址
        /// </summary>
        private MultiDictionary<string, string> RouterAddresses
        {
            get;
            set;
        }

        public IList<string> GetAddresses(string serviceNamespace)
        {
            return this.RouterAddresses.GetValues(serviceNamespace);
        }

        internal DirectTable Direct
        {
            get;
            private set;
        }


        public void LoadFrom(XmlNode root)
        {
            LoadRouter(root);
            LoadDirect(root);
        }


        private void LoadRouter(XmlNode root)
        {
            var defaultNamespace = ServiceModelConfiguration.Current.Client.DefaultServiceNamespace;
            var nodes = root.SelectNodes("router/add");
            foreach (XmlNode node in nodes)
            {
                var address = node.GetAttributeValue("address");
                if (!string.IsNullOrEmpty(address))
                {
                    string ns = node.GetAttributeValue("namespace", defaultNamespace);
                    this.RouterAddresses.Add(ns, address);
                }
            }
        }

        private void LoadDirect(XmlNode section)
        {
            var defaultNamespace = ServiceModelConfiguration.Current.Client.DefaultServiceNamespace;
            var nodes = section.SelectNodes("direct/add");
            List<string> addresses = new List<string>(nodes.Count);
            foreach (XmlNode node in nodes)
            {
                var name = node.GetAttributeValue("name");
                var address = node.GetAttributeValue("address");
                if (name.IndexOf(":") == -1 && !string.IsNullOrEmpty(defaultNamespace))
                    name = string.Format("{0}:{1}", defaultNamespace, name);
                this.Direct.Add(name, address);
            }
        }



        public WebClientConfig()
        {
            this.Direct = new DirectTable();
            this.RouterAddresses = new MultiDictionary<string, string>(false);
        }

    }
}
