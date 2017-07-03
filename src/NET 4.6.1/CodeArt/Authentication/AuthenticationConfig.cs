using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

using CodeArt.Util;

namespace CodeArt.Authentication
{
	public class AuthenticationConfig
    {
        public AuthenticationConfig()
        {
            this.Identity = Identity.Empty;
        }

        public Identity Identity
        {
            get;
            private set;
        }



        #region 加载云身份

        private void LoadIdentity(XmlNode section)
        {
            var identityNode = section.SelectSingleNode("identity");

            if (identityNode != null)
            {
                var name = identityNode.GetAttributeValue("name");
                var password = identityNode.GetAttributeValue("password");
                this.Identity = new Identity(name, password);
            }
            else
                this.Identity = Identity.Empty;
        }
        



        #endregion

        public void LoadFrom(XmlNode section)
        {
            LoadIdentity(section);
        }

        internal static readonly AuthenticationConfig Default = new AuthenticationConfig();

    }
}
