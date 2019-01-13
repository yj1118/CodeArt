using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace CodeArt.RabbitMQ
{
    /// <summary>
    /// 使用rabbitMQ时用到的策略
    /// </summary>
    public sealed class Policy
    {
        /// <summary>
        /// 策略名称
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// 该策略用到的RabbitMQ服务相关的信息
        /// </summary>
        public Server Server
        {
            get;
            private set;
        }


        public User User
        {
            get;
            private set;
        }

        /// <summary>
        ///  消费者在开启acknowledge的情况下，对接收到的消息可以根据业务的需要异步对消息进行确认。
        ///  然而在实际使用过程中，由于消费者自身处理能力有限，从rabbitMQ获取一定数量的消息后，
        ///  希望rabbitmq不再将队列中的消息推送过来，当对消息处理完后（即对消息进行了ack，并且有能力处理更多的消息）
        ///  再接收来自队列的消息。在这种场景下，我们可以通过设置PrefetchCount来达到这种效果。
        ///  如果PrefetchCount设置为50，表示最多同时处理50个消息，多余的消息RabbitMQ会堆积在服务器或者给其他的消费者处理
        /// </summary>
        public ushort PrefetchCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 发送方确认模式，一旦信道进入该模式，所有在信道上发布的消息都会被指派以个唯一的ID号（从1开始）。
        /// 一旦消息被成功投递给所匹配的队列后，信道会发送一个确认给生产者（包含消息的唯一ID）。这使得生产者
        /// 知晓消息已经安全达到目的队列了。如果消息和队列是可持久化的，那么确认消息只会在队列将消息写入磁盘后才会发出。
        /// 如果RabbitMQ服务器发生了宕机或者内部错误导致了消息的丢失，Rabbit会发送一条nack消息给生产者，表明消息已经丢失。
        /// 该模式性能优秀可以取代性能低下的发送消息事务机制。
        /// 在需要严谨的、消息必须送达的情况下需要开启该模式。
        /// </summary>
        public bool PublisherConfirms
        {
            get;
            private set;
        }

        /// <summary>
        /// 是否持久化消息，如果为true，消息会被写入到日志，只有当消费者确认消费了该消息后，才会从日志中清除
        /// 持久化的消息是可以恢复的，就算服务器宕机了也可以保证消息依然能正确的送达
        /// 所以在需要严谨的、消息必须送达的情况下需要开启该模式。
        /// </summary>
        public bool PersistentMessages
        {
            get;
            private set;
        }


        public string ConnectionString
        {
            get;
            private set;
        }

        public Policy(string name, Server server, User user, ushort prefetchCount, bool publisherConfirms, bool persistentMessages)
        {
            this.Name = name;
            this.Server = server;
            this.User = user;
            this.PrefetchCount = prefetchCount;
            this.PublisherConfirms = publisherConfirms;
            this.PersistentMessages = persistentMessages;
            this.ConnectionString = GetConnectionString();
        }

        private string GetConnectionString()
        {
            StringBuilder code = new StringBuilder();
            code.AppendFormat("host={0};", this.Server.Host);
            code.AppendFormat("virtualHost={0};", this.Server.VirtualHost);
            code.AppendFormat("username={0};", this.User.Name);
            code.AppendFormat("password={0};", this.User.Password);
            code.AppendFormat("prefetchcount={0};", this.PrefetchCount);
            code.AppendFormat("publisherConfirms={0};", this.PublisherConfirms.ToString().ToLower());
            code.AppendFormat("persistentMessages={0};", this.PersistentMessages.ToString().ToLower());
            return code.ToString();
        }

        public override int GetHashCode()
        {
            return this.ConnectionString.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var target = obj as Policy;
            if (target == null) return false;
            return target.ConnectionString.Equals(this.ConnectionString);
        }


        ///// <summary>
        ///// 根据策略配置消息
        ///// </summary>
        ///// <param name="msg"></param>
        //public void SetMessage(MessageProperties msg)
        //{
        //    if(this.PersistentMessages)
        //    {
        //        msg.DeliveryMode = 2; //必须设置为2才能持久化
        //    }


        //}


        internal void Init(ConnectionFactory factory)
        {
            factory.HostName = Server.Host;
            factory.VirtualHost = Server.VirtualHost;
            factory.UserName = User.Name;
            factory.Password = User.Password;
        }

    }
}