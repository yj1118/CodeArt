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

namespace CodeArt.RabbitMQ
{
    /// <summary>
    /// 为事件提供广播服务的广播器
    /// </summary>
    public class RPCServer : IServer, IDisposable, IMessageHandler
    {
        private IPoolItem<RabbitBus> _busItem;
        private IRPCHandler _handler;
        private string _queue;

        public RPCServer(string method)
        {
            _busItem = RabbitBus.Borrow(RPC.Policy);
            _queue = RPC.GetServerQueue(method);
        }

        public void Open(IRPCHandler handler)
        {
            _handler = handler;

            var bus = _busItem.Item;
            bus.QueueDeclare(_queue);
            bus.Consume(_queue, this);
        }


        void IMessageHandler.Handle(Message  message)
        {
            var bus = _busItem.Item;

            AppSession.Using(() =>
            {
                try
                {
                    var content = message.Content;
                    var method = content.GetValue<string>("method");
                    var args = content.GetObject("args");

                    var result = DTObject.CreateReusable();
                    Process(method, args, result);

                    var routingKey = message.Properties.ReplyTo; //将客户端的临时队列名称作为路由key
                    bus.Publish(string.Empty, routingKey, result, (replyProps) =>
                    {
                        replyProps.CorrelationId = message.Properties.CorrelationId;
                    });
                }
                catch (Exception ex)
                {
                    LogWrapper.Default.Fatal(ex);

                    var arg = new RPCEvents.ServerErrorArgs(ex);
                    RPCEvents.RaiseServerError(this, arg);
                }
                finally
                {
                    message.Success();
                }
            }, true);
        }

        private void Process(string method, DTObject args, DTObject result)
        {
            try
            {
                result["status"] = "success";
                result["returnValue"] = _handler.Process(method, args);
            }
            catch (Exception ex)
            {
                LogWrapper.Default.Fatal(ex);
                result["status"] = "fail";
                result["message"] = ex.Message;
            }
        }

        public void Close()
        {
            this.Dispose();
        }


        public void Dispose()
        {
            _busItem.Dispose();
        }
    }
}
