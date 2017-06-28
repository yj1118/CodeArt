using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

using CodeArt.Util;

namespace CodeArt.AppSetting
{
	public class AppSettingConfig
    {
        internal InterfaceImplementer AppSessionImplementer
        {
            get;
            private set;
        }

        public AppSettingConfig()
        {
        }

        public void LoadFrom(XmlNode section)
        {
            var sessionNode = section.SelectSingleNode("session");
            this.AppSessionImplementer = InterfaceImplementer.Create(sessionNode);
        }

        internal static readonly AppSettingConfig Default = new AppSettingConfig();

    }
}
