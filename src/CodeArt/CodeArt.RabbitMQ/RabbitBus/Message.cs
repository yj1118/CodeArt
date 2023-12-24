using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.EasyMQ;
using RabbitMQ.Client;

namespace CodeArt.RabbitMQ
{
    public class Message
    {
        public string Language
        {
            get
            {
                return this.Content.Language;
            }
        }

        /// <summary>
        /// 消息的内容
        /// </summary>
        public TransferData Content
        {
            get;
            private set;
        }

        /// <summary>
        /// 消息的属性
        /// </summary>
        public IBasicProperties Properties
        {
            get;
            private set;
        }

        private Action _ack;

        /// <summary>
        /// 回复消息队列，提示消息已成功处理
        /// </summary>
        public void Success()
        {
            _ack();
        }


        private Action<bool> _reject;

        /// <summary>
        /// 回复消息队列，提示消息处理失败
        /// </summary>
        /// <param name="requeue">true:提示RabbitMQ服务器重发消息给下一个订阅者，false:提示RabbitMQ服务器把消息从队列中移除</param>
        public void Failed(bool requeue)
        {
            _reject(requeue);
        }


        internal Message(TransferData content, IBasicProperties properties, Action ack, Action<bool> reject)
        {
            this.Content = content;
            this.Properties = properties;
            _ack = ack;
            _reject = reject;
        }


    }
}
