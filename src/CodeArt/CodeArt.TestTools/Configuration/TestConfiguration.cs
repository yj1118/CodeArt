using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using CodeArt.AppSetting;
using CodeArt.Runtime;

namespace CodeArt.TestTools
{
    public class TestConfiguration : IConfigurationSectionHandler
    {
        /// <summary>
        /// 日志工厂的实现
        /// </summary>
        public InterfaceImplementer LogFactoryImplementer { get; set; }

        internal ITestLogFactory GetLogFactory()
        {
            return this.LogFactoryImplementer?.GetInstance<ITestLogFactory>();
        }

        private void Load(XmlNode root)
        {
            var logSection = root.SelectSingleNode("log");
            if (logSection != null)
            {
                var logFactory = InterfaceImplementer.Create(logSection.SelectSingleNode("factory"));
                if (logFactory != null)
                    this.LogFactoryImplementer = logFactory;
            }
        }



        internal TestConfiguration()
        {
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            TestConfiguration config = new TestConfiguration();
            config.Load(section);
            return config;
        }

        #region 全局配置

        /// <summary>
        /// 全局配置信息
        /// </summary>
        internal static readonly TestConfiguration Current;

        static TestConfiguration()
        {
            Current = Configuration.GetSection("codeArt.test", () => new TestConfiguration()) as TestConfiguration;
            if (Current == null)
                Current = new TestConfiguration();
        }

        #endregion

    }
}
