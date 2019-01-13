using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using CodeArt.AppSetting;
using CodeArt.Runtime;

namespace CodeArt.DomainDriven
{
    public class DomainDrivenConfiguration : IConfigurationSectionHandler
    {
        public RepositoryConfig RepositoryConfig { get; private set; }

        public RemotableConfig RemotableConfig { get; private set; }

        public BufferConfig BufferConfig { get; private set; }
        public MultiTenancyConfig MultiTenancyConfig { get; private set; }

        internal DomainDrivenConfiguration()
        {
            this.RepositoryConfig = new RepositoryConfig();
            this.RemotableConfig = new RemotableConfig();
            this.BufferConfig = new BufferConfig();
            this.MultiTenancyConfig = new MultiTenancyConfig();
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            DomainDrivenConfiguration config = new DomainDrivenConfiguration();
            config.RepositoryConfig = DeserializeRepositoryConfig(section);
            config.RemotableConfig = DeserializeRemotableConfig(section);
            config.BufferConfig = DeserializeBufferConfig(section);
            config.MultiTenancyConfig = DeserializeMultiTenancyConfig(section);
            return config;
        }

        private RepositoryConfig DeserializeRepositoryConfig(XmlNode root)
        {
            var config = new RepositoryConfig();

            var section = root.SelectSingleNode("repository");
            if (section == null) return config;

            config.LoadFrom(section);
            return config;
        }

        private RemotableConfig DeserializeRemotableConfig(XmlNode root)
        {
            RemotableConfig config = new RemotableConfig();

            var section = root.SelectSingleNode("remotable");
            if (section == null) return config;

            config.LoadFrom(section);
            return config;
        }

        private BufferConfig DeserializeBufferConfig(XmlNode root)
        {
            BufferConfig config = new BufferConfig();

            var section = root.SelectSingleNode("buffer");
            if (section == null) return config;

            config.LoadFrom(section);
            return config;
        }

        private MultiTenancyConfig DeserializeMultiTenancyConfig(XmlNode root)
        {
            MultiTenancyConfig config = new MultiTenancyConfig();

            var section = root.SelectSingleNode("multiTenancy");
            if (section == null) return config;

            config.LoadFrom(section);
            return config;
        }

        #region 全局配置

        /// <summary>
        /// 全局配置信息
        /// </summary>
        internal static readonly DomainDrivenConfiguration Current;

        static DomainDrivenConfiguration()
        {
            Current = ConfigurationManager.GetSection("codeArt.domainDriven") as DomainDrivenConfiguration;
            if (Current == null) Current = new DomainDrivenConfiguration();
        }

        #endregion

    }
}
