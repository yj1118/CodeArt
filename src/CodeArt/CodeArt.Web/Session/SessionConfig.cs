using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

using CodeArt.AppSetting;

namespace CodeArt.Web
{
    [XmlRoot("session")]
	public class SessionConfig
    {
        [XmlIgnore]
        public InterfaceImplementer SessionStorage;

        [XmlIgnore]
        public InterfaceImplementer KeyProvider;

        public SessionConfig()
        {

        }

        public void LoadFrom(XmlNode section)
        {
            var node = section.SelectSingleNode("storage");
            if (node != null) this.SessionStorage = AppSettingUtil.ParseInterfaceImplement(node);

            node = section.SelectSingleNode("keyProvider");
            if (node != null) this.KeyProvider = AppSettingUtil.ParseInterfaceImplement(node);
        }

        public static readonly SessionConfig Default = new SessionConfig();

	}
}
