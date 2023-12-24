using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using CodeArt.AppSetting;
using CodeArt.Runtime;

namespace CodeArt.RabbitMQ
{
    public class RabbitMQConfiguration : IConfigurationSectionHandler
    {
        public PolicyGroupConfig PolicyGroupConfig { get; private set; }

        internal RabbitMQConfiguration()
        {
            this.PolicyGroupConfig = new PolicyGroupConfig();
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            RabbitMQConfiguration config = new RabbitMQConfiguration();
            config.PolicyGroupConfig = DeserializePolicyGroupConfig(section);
            return config;
        }

        private PolicyGroupConfig DeserializePolicyGroupConfig(XmlNode root)
        {
            var config = new PolicyGroupConfig();

            var section = root.SelectSingleNode("policyGroup");
            if (section == null) return config;

            config.LoadFrom(section);
            return config;
        }

        #region 全局配置

        /// <summary>
        /// 全局配置信息
        /// </summary>
        internal static readonly RabbitMQConfiguration Current;

        static RabbitMQConfiguration()
        {
            Current = Configuration.GetSection("codeArt.rabbitMQ", () => new RabbitMQConfiguration()) as RabbitMQConfiguration;
            if (Current == null)
                Current = new RabbitMQConfiguration();
        }

        #endregion

    }
}
