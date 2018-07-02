using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using CodeArt.Concurrent;
using CodeArt.Util;
using CodeArt.DTO;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CodeArt.RabbitMQ
{
    public interface IMessageHandler
    {
        /// <summary>
        /// 请在处理完消息后调用应答方法
        /// </summary>
        /// <param name="msg"></param>
        void Handle(Message msg);
    }
}