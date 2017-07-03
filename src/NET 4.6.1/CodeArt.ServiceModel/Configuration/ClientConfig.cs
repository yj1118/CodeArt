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
    public class ClientConfig
	{
        /// <summary>
        /// 默认的服务命名空间
        /// </summary>
        public string DefaultServiceNamespace
        {
            get;
            internal set;
        }

        /// <summary>
        /// 服务代理的实现
        /// </summary>
        public InterfaceImplementer ProxyImplementer { get; internal set; }

        /// <summary>
        /// 身份提供者的实现
        /// </summary>
        public InterfaceImplementer IdentityProviderImplementer { get; internal set; }

        internal IIdentityProvider GetIdentityProvider()
        {
            return this.IdentityProviderImplementer != null ? this.IdentityProviderImplementer.GetInstance<IIdentityProvider>() : DefaultIdentityProvider.Instance;
        }

        internal IServiceProxy GetProxy()
        {
            return this.ProxyImplementer != null ? this.ProxyImplementer.GetInstance<IServiceProxy>() : null;
        }


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
            LoadServices(root);
            LoadProxy(root);
            LoadSecurity(root);
            LoadRouter(root);
            LoadDirect(root);
        }

        private void LoadServices(XmlNode root)
        {
            var section = root.SelectSingleNode("services");
            if (section == null) return;
            this.DefaultServiceNamespace = section.GetAttributeValue("defaultNamespace", string.Empty);
        }

        private void LoadProxy(XmlNode root)
        {
            var proxyImplement = InterfaceImplementer.Create(root.SelectSingleNode("proxy"));
            if (proxyImplement != null)
                this.ProxyImplementer = proxyImplement;
        }

        private void LoadSecurity(XmlNode root)
        {
            var section = root.SelectSingleNode("security");
            if (section == null) return;

            var identityProviderImplement = InterfaceImplementer.Create(section.SelectSingleNode("identityProvider"));
            if (identityProviderImplement != null)
                this.IdentityProviderImplementer = identityProviderImplement;
        }


        private void LoadRouter(XmlNode root)
        {
            var nodes = root.SelectNodes("router/add");
            foreach (XmlNode node in nodes)
            {
                var address = node.GetAttributeValue("address");
                if (!string.IsNullOrEmpty(address))
                {
                    string ns = node.GetAttributeValue("namespace", this.DefaultServiceNamespace);
                    this.RouterAddresses.Add(ns, address);
                }
            }
        }

        private void LoadDirect(XmlNode section)
        {
            var nodes = section.SelectNodes("direct/add");
            List<string> addresses = new List<string>(nodes.Count);
            foreach (XmlNode node in nodes)
            {
                var name = node.GetAttributeValue("name");
                var address = node.GetAttributeValue("address");
                if (name.IndexOf(":") == -1 && !string.IsNullOrEmpty(this.DefaultServiceNamespace))
                    name = string.Format("{0}:{1}", this.DefaultServiceNamespace, name);
                this.Direct.Add(name, address);
            }
        }



        public ClientConfig()
        {
            this.DefaultServiceNamespace = string.Empty;
            this.Direct = new DirectTable();
            this.RouterAddresses = new MultiDictionary<string, string>(false);
        }

    }
}
