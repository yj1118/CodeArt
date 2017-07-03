using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Linq;

using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.AppSetting;

namespace CodeArt.ServiceModel.Mock
{
    public class ServiceConfiguration : IConfigurationSectionHandler
    {
        public string[] ContractEventsAssemblyNames
        {
            get;
            private set;
        }

        /// <summary>
        /// 契约包工厂的实现
        /// </summary>
        public InterfaceImplementer ContractPackageFactoryImplementer { get; set; }


        /// <summary>
        /// 本地mock数据所在的目录路径
        /// </summary>
        public string LocalPath
        {
            get;
            private set;
        }

        internal IContractPackage GetContractPackage()
        {
            var factory = this.ContractPackageFactoryImplementer != null ? this.ContractPackageFactoryImplementer.GetInstance<IContractPackageFactory>() : LocalContractPackageFactory.Instance;
            return factory.Create();
        }

        public ServiceConfiguration()
        {
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            var contractNode = section.SelectSingleNode("contract");
            var packageFactoryImplement = InterfaceImplementer.Create(contractNode.SelectSingleNode("packageFactory"));
            if (packageFactoryImplement != null)
                this.ContractPackageFactoryImplementer = packageFactoryImplement;

            this.LocalPath = string.Empty;
            var localNode = contractNode.SelectSingleNode("local");
            if (localNode != null)
            {
                this.LocalPath = localNode.GetAttributeValue("path");
            }

            this.ContractEventsAssemblyNames = Array.Empty<string>();
            var eventNode = section.SelectSingleNode("event");
            if (eventNode != null)
            {
                var assemblies = eventNode.GetAttributeValue("assemblies").Trim(';');
                this.ContractEventsAssemblyNames = assemblies.Split(';');
            }

            return this;
        }

        #region 当前配置

        public static ServiceConfiguration Current;
        static ServiceConfiguration()
        {
            Current = (ServiceConfiguration)ConfigurationManager.GetSection("codeArt.serviceModel.mock");
            if (Current == null)
                throw new ServiceException("没有配置codeArt.serviceModel.mock节点，无法使用ServiceConfiguration");
        }

        #endregion
    }
}
