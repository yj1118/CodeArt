using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Configuration;
using System.Xml;

using CodeArt.Runtime;
using CodeArt.AppSetting;
using CodeArt.Util;

namespace CodeArt.Selenium
{
    /// <summary>
    /// 策略组配置
    /// </summary>
    public class DriverConfig
    {
        public DriverConfig()
        {
        }

        public string Type
        {
            get;
            private set;
        }

        public string Directory
        {
            get;
            private set;
        }


        internal void LoadFrom(XmlNode root)
        {
            var driverNode = root.SelectSingleNode("driver");
            if (driverNode == null) return;
           
            this.Type = driverNode.GetAttributeValue("type");
            this.Directory = driverNode.GetAttributeValue("directory");
        }
    }
}
