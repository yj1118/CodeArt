using System;
using System.Linq;
using System.Xml.Serialization;
using System.Configuration;
using System.Xml;

using CodeArt.Runtime;
using CodeArt.AppSetting;

namespace CodeArt.DomainDriven
{
    public class RepositoryConfig
    {
        #region 仓储

        /// <summary>
        /// 仓储的直接实现
        /// </summary>
        public InterfaceMapper RepositoryMapper { get; set; }

        /// <summary>
        /// 仓储的事务管理实现
        /// </summary>
        public InterfaceImplementer TransactionManagerFactoryImplementer { get; set; }

        internal ITransactionManagerFactory GetTransactionManagerFactory()
        {
            return this.TransactionManagerFactoryImplementer?.GetInstance<ITransactionManagerFactory>();
        }

        internal void LoadFrom(XmlNode root)
        {
            var mapper = root.SelectSingleNode("mapper");
            if (mapper != null)
            {
                var map = InterfaceMapper.Create(mapper);
                if (map != null) this.RepositoryMapper = map;
            }

            var transactionManagerFactory = root.SelectSingleNode("transactionManagerFactory");
            if (transactionManagerFactory != null)
            {
                var imp = InterfaceImplementer.Create(transactionManagerFactory);
                if (imp != null) this.TransactionManagerFactoryImplementer = imp;
            }
        }

        #endregion
	}
}
