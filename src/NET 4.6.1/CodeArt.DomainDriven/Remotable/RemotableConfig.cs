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
    public class RemotableConfig
    {
        /// <summary>
        /// 远程服务的实现
        /// </summary>
        public InterfaceImplementer RemoteServiceImplementer { get; private set; }

        public IEnumerable<Membership> Memberships { get; private set; }

        /// <summary>
        /// 所有会籍的地址
        /// </summary>
        public IEnumerable<string> MembershipAddresses { get; private set; }

        public bool Authentication { get; private set; }


        internal IRemoteService GetRemoteService()
        {
            return this.RemoteServiceImplementer != null
                     ? this.RemoteServiceImplementer.GetInstance<IRemoteService>()
                    : null;
        }

        internal void LoadFrom(XmlNode root)
        {
            this.Authentication = root.GetAttributeValue("authentication", "true") == "true";
            LoadRemoteService(root);
            LoadMemberships(root);
        }

        private void LoadRemoteService(XmlNode root)
        {
            var service = root.SelectSingleNode("remoteService");
            if (service != null)
            {
                var imp = InterfaceImplementer.Create(service);
                if (imp != null) this.RemoteServiceImplementer = imp;
            }
        }


        private void LoadMemberships(XmlNode root)
        {
            List<string> membershipAddresses = new List<string>();

            var nodes = root.SelectNodes("membership");
            var memberships = new List<Membership>(nodes.Count);
            foreach (XmlNode node in nodes)
            {
                var name = node.GetAttributeValue("name");
                if (!string.IsNullOrEmpty(name))
                {
                    List<string> addresses = new List<string>();

                    var address = node.GetAttributeValue("address", string.Empty); //可以在节点上直接指定一个地址
                    if (!string.IsNullOrEmpty(address)) addresses.Add(address);

                    //也可以以子节点的形式指定多个地址
                    var addressNodes = node.SelectNodes("address");
                    foreach (XmlNode addressNode in addressNodes)
                    {
                        address = addressNode.GetAttributeValue("value", string.Empty); //可以在节点上直接指定一个地址
                        if (!string.IsNullOrEmpty(address)
                            && !addresses.Contains(address)) addresses.Add(address);
                    }
                    var membership = new Membership(name, addresses);
                    memberships.Add(membership);

                    membershipAddresses.AddRange(addresses);
                }
            }
            this.Memberships = memberships;
            this.MembershipAddresses = membershipAddresses.Distinct();
        }


        public class Membership
        {
            /// <summary>
            /// 会籍名
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// 会籍地址
            /// </summary>
            public IEnumerable<string> Addresses { get; private set; }


            public Membership(string name, IEnumerable<string> addresses)
            {
                this.Name = name;
                this.Addresses = addresses;
            }
        }
    }
}
