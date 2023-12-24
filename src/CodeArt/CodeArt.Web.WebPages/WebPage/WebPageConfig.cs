using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

using CodeArt.AppSetting;

namespace CodeArt.Web.WebPages
{
    [XmlRoot("codeArt.web.pages")]
	public class WebPageConfig
    {
        #region GET

        [XmlIgnore]
        public InterfaceImplementer CodePreprocessor;

        [XmlIgnore]
        public InterfaceImplementer CompressorFactory;

        //[XmlIgnore]
        //public InterfaceImplement SecurityFactory;

        #endregion


        #region 缓存

        [XmlIgnore]
        public InterfaceImplementer ClientCacheFactory;

        [XmlIgnore]
        public InterfaceImplementer ServerCacheFactory;


        #endregion

        //#region 性能
    
        ///// <summary>
        ///// 开启性能追踪
        ///// </summary>
        //public bool OpenPerformanceTracking
        //{
        //    get;
        //    set;
        //}

        //#endregion

        public WebPageConfig()
        {
            
        }

        #region pageLocators

        private Dictionary<string, InterfaceImplementer> _locators = new Dictionary<string, InterfaceImplementer>();

        private void LoadPageLocators(XmlNode section)
        {
            var node = section.SelectSingleNode("pageLocator");
            if (node != null)
            {
                var items = node.SelectNodes("add");
                foreach (XmlNode item in items)
                {
                    var extension = item.Attributes["extension"].Value;
                    if (!extension.StartsWith(".")) extension = string.Format(".{0}", extension);
                    _locators[extension] = AppSettingUtil.ParseInterfaceImplement(item);
                }
            }
        }

        public IWebPageLocator GetPageLocator(string extension)
        {
            InterfaceImplementer imp = null;
            if (_locators.TryGetValue(extension, out imp)) return imp.GetInstance<IWebPageLocator>();
            return null;
        }

        #endregion


        public void LoadFrom(XmlNode section)
        {
            LoadPageLocators(section);

            var node = section.SelectSingleNode("codePreprocessor");
            if (node != null) this.CodePreprocessor = AppSettingUtil.ParseInterfaceImplement(node);

            node = section.SelectSingleNode("compressorFactory");
            if (node != null) this.CompressorFactory = AppSettingUtil.ParseInterfaceImplement(node);

            node = section.SelectSingleNode("clientCacheFactory");
            if (node != null) this.ClientCacheFactory = AppSettingUtil.ParseInterfaceImplement(node);

            node = section.SelectSingleNode("serverCacheFactory");
            if (node != null) this.ServerCacheFactory = AppSettingUtil.ParseInterfaceImplement(node);

            //node = section.SelectSingleNode("securityFactory");
            //if (node != null) this.SecurityFactory = AppSettingUtil.ParseInterfaceImplement(node);

            //node = section.SelectSingleNode("performance");
            //if (node != null)
            //{
            //    var attr = node.Attributes["tracking"];
            //    this.OpenPerformanceTracking = attr == null ? false : (attr.Value == "true");
            //}
        }



        public static readonly WebPageConfig Default = new WebPageConfig();

	}
}
