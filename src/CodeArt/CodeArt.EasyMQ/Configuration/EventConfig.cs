using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Configuration;
using System.Xml;

using CodeArt.Runtime;
using CodeArt.AppSetting;
using CodeArt.Util;

namespace CodeArt.EasyMQ.Event
{
    public class EventConfig
    {
        public EventConfig()
        {
            this.SubscriberGroup = "default";
        }

        /// <summary>
        /// 事件发布者工厂的实现
        /// </summary>
        public InterfaceImplementer PublisherFactoryImplementer { get; set; }

        /// <summary>
        /// 事件订阅者工厂的实现
        /// </summary>
        public InterfaceImplementer SubscriberFactoryImplementer { get; set; }

        /// <summary>
        /// 订阅者分组，每个分组里的订阅者会均衡的处理收到的事件
        /// </summary>
        public string SubscriberGroup { get; set; }


        internal void LoadFrom(XmlNode root)
        {
            LoadPublisher(root);
            LoadSubscriber(root);
        }

        private void LoadPublisher(XmlNode root)
        {
            var node = root.SelectSingleNode("publisher");
            if (node == null) return;

            var factoryNode = node.SelectSingleNode("factory");
            if (factoryNode != null) this.PublisherFactoryImplementer = InterfaceImplementer.Create(factoryNode);
        }

        private void LoadSubscriber(XmlNode root)
        {
            var node = root.SelectSingleNode("subscriber");
            if (node == null) return;

            this.SubscriberGroup = node.GetAttributeValue("group", string.Empty);

            var factoryNode = node.SelectSingleNode("factory");
            if (factoryNode != null) this.SubscriberFactoryImplementer = InterfaceImplementer.Create(factoryNode);
        }


    }
}
