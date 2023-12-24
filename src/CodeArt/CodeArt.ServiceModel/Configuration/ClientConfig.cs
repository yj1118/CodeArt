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


        internal IServiceProxy GetProxy()
        {
            return this.ProxyImplementer != null ? this.ProxyImplementer.GetInstance<IServiceProxy>() : null;
        }

        /// <summary>
        /// 启用老版本模式，该设置主要用于兼容老的服务提供者，老版本不支持服务的命名空间
        /// </summary>
        public bool EnabledOldVersion
        {
            get;
            private set;
        }



        public void LoadFrom(XmlNode root)
        {
            LoadServices(root);
            LoadProxy(root);

            this.EnabledOldVersion = root.GetAttributeValue("EnabledOldVersion", null) != null;
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


        public ClientConfig()
        {
            this.DefaultServiceNamespace = string.Empty;
        }

    }
}
