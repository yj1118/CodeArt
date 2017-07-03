using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Configuration;
using System.Xml;

using CodeArt.Util;
using CodeArt.AppSetting;


namespace CodeArt.ServiceModel
{
    public class ServerConfig
    {
        /// <summary>
        /// 服务提供者工厂的实现
        /// </summary>
        public InterfaceImplementer ProviderFactoryImplementer { get; set; }

        internal IServiceProviderFactory GetProviderFactory()
        {
            return this.ProviderFactoryImplementer?.GetInstance<IServiceProviderFactory>();
        }

        internal void LoadFrom(XmlNode root)
        {
            LoadHost(root);
        }

        private void LoadHost(XmlNode root)
        {
            var section = root.SelectSingleNode("host");
            if (section == null) return;
            var providerFactory = InterfaceImplementer.Create(section.SelectSingleNode("providerFactory"));
            if (providerFactory != null)
                this.ProviderFactoryImplementer = providerFactory;
        }
        
    }
}
