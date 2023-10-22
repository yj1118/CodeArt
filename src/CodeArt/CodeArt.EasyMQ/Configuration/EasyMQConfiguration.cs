using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using CodeArt.AppSetting;
using CodeArt.Runtime;

using CodeArt.EasyMQ.Event;

namespace CodeArt.EasyMQ
{
    public class EasyMQConfiguration : IConfigurationSectionHandler
    {
        public EventConfig EventConfig { get; private set; }

        public RPCConfig RPCConfig { get; private set; }

        internal EasyMQConfiguration()
        {
            this.EventConfig = new EventConfig();
            this.RPCConfig = new RPCConfig();
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            EasyMQConfiguration config = new EasyMQConfiguration();
            config.EventConfig = DeserializeEventConfig(section);
            config.RPCConfig = DeserializeRPCConfig(section);
            return config;
        }

        private EventConfig DeserializeEventConfig(XmlNode root)
        {
            var config = new EventConfig();

            var section = root.SelectSingleNode("event");
            if (section == null) return config;

            config.LoadFrom(section);
            return config;
        }

        private RPCConfig DeserializeRPCConfig(XmlNode root)
        {
            var config = new RPCConfig();

            var section = root.SelectSingleNode("rpc");
            if (section == null) return config;

            config.LoadFrom(section);
            return config;
        }


        #region 全局配置

        /// <summary>
        /// 全局配置信息
        /// </summary>
        public static readonly EasyMQConfiguration Current;

        static EasyMQConfiguration()
        {
            Current = Configuration.GetSection("codeArt.easyMQ", () => new EasyMQConfiguration()) as EasyMQConfiguration;
            if (Current == null) Current = new EasyMQConfiguration();
        }

        #endregion

    }
}
