using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

using CodeArt.Util;

namespace CodeArt.Caching.Redis
{
	public class CacheConfig
    {
        internal Dictionary<string, string> ConnectionStrings
        {
            get;
            set;
        }

        public CacheConfig()
        {
            this.ConnectionStrings = new Dictionary<string, string>();
        }

        public string GetConnectionString(string name)
        {
            if (this.ConnectionStrings.TryGetValue(name, out var value))
                return value;
            return string.Empty;
        }


        public void LoadFrom(XmlNode section)
        {
            var connectionStringNodes = section.SelectNodes("connectionStrings/add");
            foreach(XmlNode node in connectionStringNodes)
            {
                var name = node.GetAttributeValue("name", string.Empty);
                var connectionString = node.GetAttributeValue("connectionString", string.Empty);
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(connectionString)) continue;
                this.ConnectionStrings.Add(name, connectionString);
            }
        }

        internal static readonly CacheConfig Default = new CacheConfig();

    }
}
