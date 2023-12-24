using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Configuration;
using System.Xml;

using CodeArt.Runtime;
using CodeArt.AppSetting;
using CodeArt.Util;

namespace CodeArt.RabbitMQ
{
    /// <summary>
    /// 策略组配置
    /// </summary>
    public class PolicyGroupConfig
    {
        public PolicyGroupConfig()
        {
            this.Policies = new List<Policy>();
        }

        public List<Policy> Policies { get; set; }

        internal void LoadFrom(XmlNode root)
        {
            var policyNodes = root.SelectNodes("policy");
            foreach (XmlNode policyNode in policyNodes)
            {
                var policy = GetPolicy(policyNode);
                this.Policies.Add(policy);
            }
        }

        private Policy GetPolicy(XmlNode node)
        {
            //name="event" host="127.0.0.1" virtualHost="event" userName="yj1118" password="Q!1"
            string name = node.GetAttributeValue("name");
            string host = node.GetAttributeValue("host");
            string virtualHost = node.GetAttributeValue("virtualHost");
            Server server = new Server(host, virtualHost);
            string userName = node.GetAttributeValue("userName");
            string password = node.GetAttributeValue("password");
            User user = new User(userName, password);

            string prefetchCountString = node.GetAttributeValue("prefetchCount", string.Empty);
            ushort prefetchCount = string.IsNullOrEmpty(prefetchCountString) ? (ushort)0 : ushort.Parse(prefetchCountString);

            string publisherConfirmsString = node.GetAttributeValue("publisherConfirms", string.Empty);
            bool publisherConfirms = publisherConfirmsString == "true";

            string persistentMessagesString = node.GetAttributeValue("persistentMessages", string.Empty);
            bool persistentMessages = persistentMessagesString == "true";

            return new Policy(name, server, user, prefetchCount, publisherConfirms, persistentMessages);
        }

        public Policy GetPolicy(string policyName)
        {
            var policy = this.Policies.FirstOrDefault((p) =>
            {
                return p.Name.EqualsIgnoreCase(policyName);
            });

            if(policy == null)
            {
                throw new RabbitMQException(string.Format(Strings.NotFoundPolicy, policyName));
            }

            return policy;
        }

    }
}
