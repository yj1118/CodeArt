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
using CodeArt.AppSetting;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CodeArt.RabbitMQ
{
    /// <summary>
    /// <para>由于异步工作中，channel.Dispose非常慢，所以我们取消了异步发布者确认的工作</para>
    /// </summary>
    internal class RabbitBus : IDisposable
    {
        public Policy Policy
        {
            get;
            private set;
        }

        private IConnection _connection;
        public IConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = CreateConnection();
                }
                return _connection;
            }
        }

        private IConnection CreateConnection()
        {
            var factory = new ConnectionFactory();
            factory.HostName = this.Policy.Server.Host;
            factory.VirtualHost = this.Policy.Server.VirtualHost;
            factory.UserName = this.Policy.User.Name;
            factory.Password = this.Policy.User.Password;

            return factory.CreateConnection();
        }

        private IModel _channel;
        public IModel Channel
        {
            get
            {
                if (_channel == null)
                {
                    _channel = CreateChannel();
                }
                return _channel;
            }
        }

        private IModel CreateChannel()
        {
            var channel = this.Connection.CreateModel();
            if (this.Policy.PersistentMessages)
            {
                channel.ConfirmSelect();
            }
            channel.BasicQos(0, this.Policy.PrefetchCount, false);
            return channel;
        }


        public RabbitBus(Policy policy)
        {
            this.Policy = policy;
        }


        /// <summary>
        /// 声明交换机
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="type"></param>
        public void ExchangeDeclare(string exchange, string type)
        {
            if (this.Policy.PersistentMessages)
            {
                this.Channel.ExchangeDeclare(exchange, type, true, false);
            }
            else
            {
                this.Channel.ExchangeDeclare(exchange, type, false, true);
            }
        }

        /// <summary>
        /// 声明一个队列<paramref name="queue"/>并将其用<paramref name="routingKey"/>绑定到指定的交换机<paramref name="exchange"/>
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        public void QueueDeclare(string queue, string exchange, string routingKey)
        {
            QueueDeclare(queue);
            this.Channel.QueueBind(queue, exchange, routingKey);
        }


        public void QueueDeclare(string queue)
        {
            if (this.Policy.PersistentMessages)
            {
                this.Channel.QueueDeclare(queue, true, false, false);
            }
            else
            {
                this.Channel.QueueDeclare(queue, false, false, true);
            }
        }

        /// <summary>
        /// 声明临时队列
        /// </summary>
        /// <returns>返回队列名称</returns>
        public string TempQueueDeclare()
        {
            //临时队列是由rabbit分配名称、只有自己可以看见、用后就删除的队列
            return this.Channel.QueueDeclare(string.Empty, false, true, true).QueueName;
        }

        /// <summary>
        /// 删除队列
        /// </summary>
        /// <param name="queue"></param>
        public void QueueDelete(string queue)
        {
            if (ChannelIsClosed()) //如果连接已关闭，证明该队列已经被删除，不必重复删除
                return;
            this.Channel.QueueDelete(queue);
        }

        private bool ChannelIsClosed()
        {
            return _channel != null && _channel.IsClosed;
        }


        #region 发布消息

        /// <summary>
        /// 将dto消息发布到指定的交换机
        /// </summary>
        /// <param name="message"></param>
        public void Publish(string exchange, string routingKey, DTObject message, Action<IBasicProperties> setProperties = null)
        {
            var body = message.ToData();
            var properties = this.Channel.CreateBasicProperties();
            properties.ContentEncoding = "utf-8";
            properties.ContentType = "text/plain";

            if (setProperties != null)
            {
                setProperties(properties);
            }

            if (this.Policy.PersistentMessages)
            {
                properties.Persistent = true;
                if (!ConfirmPublish(exchange, routingKey, properties, body))
                {
                    throw new RabbitMQException(string.Format(Strings.PublishMessageFailed, message.GetCode()));
                }
            }
            else
            {
                properties.Persistent = false;
                this.Channel.BasicPublish(exchange, routingKey, properties, body);
            }
        }

        /// <summary>
        /// 发送消息，如果发送失败会尝试重发
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <param name="properties"></param>
        /// <param name="body"></param>
        private bool ConfirmPublish(string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            var nack = false;
            const int maxResendTimes = 3;
            int sendTimes = 0;

            do
            {
                if (sendTimes > 0)
                {
                    Thread.Sleep(1000); //1秒后重发
                }
                this.Channel.BasicPublish(exchange, routingKey, properties, body);
                nack = !this.Channel.WaitForConfirms();
                sendTimes++;
            }
            while (nack && sendTimes <= maxResendTimes);
            return !nack;
        }

        #endregion

        private IMessageHandler _messageHandler = null;

        public void Consume(string queue, IMessageHandler handler)
        {
            _messageHandler = handler;
            Accept(queue, false);
        }

        private void Accept(string queue, bool noAck)
        {
            var consumer = new EventingBasicConsumer(this.Channel);
            consumer.Received += MessageReceived;
            this.Channel.BasicConsume(queue, noAck, consumer);
        }


        private void MessageReceived(object sender, BasicDeliverEventArgs e)
        {
            IBasicProperties properties = e.BasicProperties;

            DTObject content = DTObject.CreateReusable(e.Body);
            var message = new Message(content, properties, () =>
              {
                  this.Channel.BasicAck(e.DeliveryTag, false);
              }, (requeue) =>
              {
                  this.Channel.BasicReject(e.DeliveryTag, requeue);
              });
            _messageHandler.Handle(message);
        }

        /// <summary>
        /// 回收资源，这样会导致取消订阅等操作，回收后再次使用bus,会重新建立连接
        /// RabbitMQ建议客户端线程之间不要共用Channel，
        /// 至少要保证共用Channel的线程发送消息必须是串行的，但是建议尽量共用Connection。
        /// </summary>
        internal void Clear()
        {
            if (_channel != null)
            {
                _channel.Dispose();
                _channel = null;
            }

            //暂时不共享连接，所以此处释放连接
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }


        public void Dispose()
        {
            Clear();
        }

        public static IPoolItem<RabbitBus> Borrow(Policy policy)
        {
            var pool = _getPool(policy);
            return pool.Borrow();
        }

        private static Func<Policy, Pool<RabbitBus>> _getPool = LazyIndexer.Init<Policy, Pool<RabbitBus>>((policy) =>
        {
            return new Pool<RabbitBus>(() =>
            {
                return new RabbitBus(policy);
            }, (bus, phase) =>
            {
                if (phase == PoolItemPhase.Returning)
                {
                    bus.Clear();
                }
                return true;
            }, new PoolConfig()
            {
                MaxRemainTime = 60 //闲置时间60秒
            });
        });
    }
}