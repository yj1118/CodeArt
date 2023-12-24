using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.EasyMQ.Event;
using CodeArt.Concurrent;
using CodeArt.DTO;
using CodeArt.Util;

namespace CodeArt.RabbitMQ
{
    internal static class Event
    {
        public const string Exchange = "event-exchange";

        /// <summary>
        /// 事件采用的消息的策略
        /// </summary>
        public static readonly Policy Policy;

        static Event()
        {
            Policy = GetEventPolicy();
        }

        private static Policy GetEventPolicy()
        {
            var policy = RabbitMQConfiguration.Current.PolicyGroupConfig.GetPolicy("event");

            //由于事件需要高可靠性，所以我们需要发布者确认模式和持久化消息
            return new Policy(policy.Name, policy.Server, policy.User, policy.PrefetchCount, true, true);
        }
    }
}
