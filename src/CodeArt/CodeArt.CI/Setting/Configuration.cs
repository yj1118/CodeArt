using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using CodeArt.AppSetting;
using CodeArt.Util;
using CodeArt.DTO;
using CodeArt.Log;
using CodeArt.Security;

using CodeArt.CI.Setting;

namespace CodeArt.CI
{
    public class Configuration : IConfigurationSectionHandler
    {
        /// <summary>
        /// 额外配置存放的路径
        /// </summary>
        public string Extra
        {
            get;
            private set;
        }

        public Workspace Workspace
        {
            get;
            private set;
        }

        public Jenkins Jenkins
        {
            get;
            private set;
        }

        internal Configuration()
        {
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            Configuration config = new Configuration();
            config.Extra = section.GetAttributeValue("extra", string.Empty);
            config.Workspace = DeserializeWorkspace(section);
            config.Jenkins = DeserializeJenkins(section);
            return config;
        }

        private Workspace DeserializeWorkspace(XmlNode root)
        {
            var section = root.SelectSingleNode("workspace");
            if (section == null) return new Workspace();

            var workspace = new Workspace();
            workspace.LoadFrom(section);
            return workspace;
        }


        private Jenkins DeserializeJenkins(XmlNode root)
        {
            var section = root.SelectSingleNode("jenkins");
            if (section == null) return new Jenkins();

            var jenkins = new Jenkins();
            jenkins.LoadFrom(section);
            return jenkins;
        }

        #region 全局配置

        /// <summary>
        /// 全局配置信息
        /// </summary>
        public static readonly Configuration Current;

        static Configuration()
        {
            Current = ConfigurationManager.GetSection("codeArt.ci") as Configuration;
            if (Current == null) Current = new Configuration();
        }

        #endregion
    }
}
