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
        public bool Record
        {
            get;
            private set;
        }

        public ServerConfig()
        {
            this.Record = false; //默认是不录制
        }

        /// <summary>
        /// 服务提供者工厂的实现
        /// </summary>
        public InterfaceImplementer ProviderFactoryImplementer { get; set; }

        internal IServiceProviderFactory GetProviderFactory()
        {
            return this.ProviderFactoryImplementer?.GetInstance<IServiceProviderFactory>();
        }


        public InterfaceImplementer RecorderFactoryImplementer { get; set; }

        internal IServiceRecorderFactory GetRecorderFactory()
        {
            return this.RecorderFactoryImplementer?.GetInstance<IServiceRecorderFactory>();
        }

        internal void LoadFrom(XmlNode root)
        {
            LoadHost(root);
        }

        private void LoadHost(XmlNode root)
        {
            var section = root.SelectSingleNode("host");
            if (section == null) return;

            this.Record = section.GetAttributeValue("record", "false").ToLower() == "true";

            var providerFactory = InterfaceImplementer.Create(section.SelectSingleNode("providerFactory"));
            if (providerFactory != null)
                this.ProviderFactoryImplementer = providerFactory;

            var recorderFactory = InterfaceImplementer.Create(section.SelectSingleNode("recorderFactory"));
            if (recorderFactory != null)
                this.RecorderFactoryImplementer = recorderFactory;
        }
        
    }
}
