using System;
using System.Linq;
using System.Xml.Serialization;
using System.Configuration;
using System.Xml;

using CodeArt.Runtime;
using CodeArt.AppSetting;
using CodeArt.Util;

namespace CodeArt.DomainDriven
{
    public class MultiTenancyConfig
    {
        public bool IsEnabled
        {
            get;
            private set;
        }

        internal void LoadFrom(XmlNode section)
        {
            this.IsEnabled = section.GetAttributeValue("isEnabled", "false").ToLower() == "true";
        }
    }
}
