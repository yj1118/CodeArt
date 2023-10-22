using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CodeArt.EasyMQ.RPC;
using CodeArt.Concurrent;
using CodeArt.DTO;
using CodeArt.Util;
using CodeArt.Log;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using CodeArt.AppSetting;
using CodeArt.EasyMQ;

namespace CodeArt.RabbitMQ
{
    /// <summary>
    /// 为事件提供广播服务的广播器
    /// </summary>
    public class RPCServer : IServer, IDisposable, IMessageHandler
    {
        private IPoolItem<RabbitBus> _hostItem;
        private IRPCHandler _handler;
        private string _queue;

        public string Name
        {
            get;
            private set;
        }

        public RPCServer(string method)
        {
            this.Name = method;
            _hostItem = RabbitBus.Borrow(RPC.Policy);
            _queue = RPC.GetServerQueue(method);
        }

        public void Initialize(IRPCHandler handler)
        {
            _handler = handler;
        }

        public void Open()
        {
            var host = _hostItem.Item;
            host.QueueDeclare(_queue);
            host.Consume(_queue, this);
        }


        void IMessageHandler.Handle(Message message)
        {
            var bus = _hostItem.Item;

            AppSession.Using(() =>
            {
                try
                {
                    AppSession.Language = message.Language;

                    var content = message.Content;
                    var info = content.Info;
                    var method = info.GetValue<string>("method");
                    var arg = info.GetObject("arg");

                    var result = Process(method, arg);

                    var routingKey = message.Properties.ReplyTo; //将客户端的临时队列名称作为路由key
                    bus.Publish(string.Empty, routingKey, result, (replyProps) =>
                    {
                        replyProps.CorrelationId = message.Properties.CorrelationId;
                    });
                }
                catch (Exception ex)
                {
                    Logger.Fatal(ex);

                    var arg = new RPCEvents.ServerErrorArgs(ex);
                    RPCEvents.RaiseServerError(this, arg);
                }
                finally
                {
                    message.Success();
                }
            }, true);
        }

        private TransferData Process(string method, DTObject arg)
        {
            TransferData result;
            DTObject info = DTObject.Create();
            try
            {
                result = _handler.Process(method, arg);

                info["status"] = "success";
                info["returnValue"] = result.Info;

                result.Info = info;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
                info["status"] = "fail";
                info["message"] = ex.Message;
                result = new TransferData(AppSession.Language, info);
            }
            return result;
        }

        public void Close()
        {
            this.Dispose();
        }


        public void Dispose()
        {
            _hostItem.Dispose();
        }
    }
}
