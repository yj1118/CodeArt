using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Configuration;

using CodeArt.Runtime;
using CodeArt.AppSetting;
using CodeArt.Util;

namespace CodeArt.TestPlatform
{
    public class VariableConfig
    {
        public VariableConfig()
        {
        }

        /// <summary>
        /// 服务提供者工厂的实现
        /// </summary>
        public InterfaceImplementer ProviderFactoryImplementer { get; set; }

        internal IVariableProviderFactory GetProviderFactory()
        {
            return this.ProviderFactoryImplementer?.GetInstance<IVariableProviderFactory>();
        }

        internal void LoadFrom(XmlNode root)
        {
            LoadVariable(root);
        }

        private void LoadVariable(XmlNode root)
        {
            var section = root.SelectSingleNode("variable");
            if (section == null) return;

            var providerFactory = InterfaceImplementer.Create(section.SelectSingleNode("providerFactory"));
            if (providerFactory != null)
                this.ProviderFactoryImplementer = providerFactory;
        }

    }
}
