using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Linq;

using CodeArt.Util;
using CodeArt.AppSetting;

namespace CodeArt.TestPlatform
{
    public class TestPlatformConfiguration : IConfigurationSectionHandler
    {
        public TestPlatformConfiguration()
        {
            this.Variable = new VariableConfig();
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            this.Variable = DeserializeVariable(section);
            return this;
        }


        #region 服务器端

        public VariableConfig Variable { get; private set; }

        private VariableConfig DeserializeVariable(XmlNode root)
        {
            VariableConfig config = new VariableConfig();
            config.LoadFrom(root);
            return config;
        }

        #endregion


        #region 当前配置

        public static TestPlatformConfiguration Current;
        static TestPlatformConfiguration()
        {
            Current = ConfigurationManager.GetSection("codeArt.testPlatform") as TestPlatformConfiguration;
            if (Current == null) Current = new TestPlatformConfiguration();
        }

        #endregion
    }
}
