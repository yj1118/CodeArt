using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Configuration;
using System.Xml;

using CodeArt.Util;
using CodeArt.AppSetting;

namespace CodeArt.DomainDriven
{
    public class BufferConfig
    {
        private InterfaceImplementer BufferImplementer { get; set; }

        public IDomainBuffer GetCache()
        {
            return this.BufferImplementer?.CreateInstance<IDomainBuffer>();
        }

        internal void LoadFrom(XmlNode root)
        {
            LoadBuffer(root);
        }

        private void LoadBuffer(XmlNode root)
        {
            var service = root.SelectSingleNode("implementer");
            if (service != null)
            {
                var imp = InterfaceImplementer.Create(service);
                if (imp != null) this.BufferImplementer = imp;
            }
        }
    }
}