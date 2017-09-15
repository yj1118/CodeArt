using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using CodeArt.AppSetting;
using CodeArt.Runtime;

namespace CodeArt.DomainDriven.DataAccess
{
    public class DataAccessConfiguration : IConfigurationSectionHandler
    {
        private InterfaceImplementer DbAgentImplementer { get; set; }

        public IDatabaseAgent GetDatabaseAgent()
        {
            return this.DbAgentImplementer?.GetInstance<IDatabaseAgent>();
        }

        internal DataAccessConfiguration()
        {
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            DataAccessConfiguration config = new DataAccessConfiguration();
            config.LoadFrom(section);
            return config;
        }

        private void LoadFrom(XmlNode root)
        {
            var agent = root.SelectSingleNode("agent");
            if (agent != null)
            {
                var imp = InterfaceImplementer.Create(agent);
                if (imp != null) this.DbAgentImplementer = imp;
            }
        }

        #region 全局配置

        /// <summary>
        /// 全局配置信息
        /// </summary>
        internal static readonly DataAccessConfiguration Current;

        static DataAccessConfiguration()
        {
            Current = ConfigurationManager.GetSection("codeArt.domainDriven.dataAccess") as DataAccessConfiguration;
            if (Current == null) Current = new DataAccessConfiguration();
        }

        #endregion

    }
}
