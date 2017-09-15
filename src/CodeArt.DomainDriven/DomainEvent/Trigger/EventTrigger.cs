using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.EasyMQ.Event;
using CodeArt.Runtime;
using CodeArt.DTO;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 事件触发器
    /// </summary>
    internal static class EventTrigger
    {
        public static Future<DomainEvent> Start(DomainEvent source)
        {
            return Start(Guid.NewGuid(), source);
        }

        /// <summary>
        /// 开始一个新的事件
        /// </summary>
        /// <param name="event"></param>
        internal static Future<DomainEvent> Start(Guid queueId, DomainEvent source)
        {
            Future<DomainEvent> future = null;
            try
            {
                queueId = CreateQueue(queueId, source);
                future = Raise(queueId);
            }
            catch (Exception ex)
            {
                EventRestorer.Restore(queueId, source.EventName, queueId, ex.Message); //此处队列编号就是事件编号
                throw ex;
            }
            return future;
        }

        private static Guid CreateQueue(Guid queueId, DomainEvent source)
        {
            DataContext.NewScope(() =>
            {
                var queue = EventQueue.Create(queueId, source);
                queueId = queue.Id;
                EventRestorer.Start(queue);
            }, true);
            return queueId;
        }

        private static Future<DomainEvent> Raise(Guid queueId)
        {
            Future<DomainEvent> future = new Future<DomainEvent>();
            bool isSucceeded = false;
            bool isRunning = false;

            while (!isRunning)
            {
                //触发队列事件
                DataContext.NewScope(() =>
                {
                    var queue = EventQueue.Find(queueId);
                    var entry = queue.GetPreRaise();
                    if (!entry.IsEmpty())
                    {
                        EventRestorer.Raise(queue, entry);
                        var local = EventFactory.GetLocalEvent(entry, false);
                        if (local != null)
                            RaiseLocalEvent(entry, local);
                        else
                            RaiseRemoteEvent(entry);
                    }
                    EventQueue.Update(queue);
                    isRunning = queue.IsRunning;
                    isSucceeded = queue.IsSucceeded;
                }, true);
            }

            if (isSucceeded)
            {
                DomainEvent source = null;
                DataContext.NewScope(() =>
                {
                    var queue = EventQueue.Find(queueId);
                    var entry = queue.Source;
                    PublishRaiseSucceeded(queueId, source.EventName, entry.EventId, entry.ArgsCode);
                    source = EventFactory.GetLocalEvent(entry, true);
                }, true);
                EventRestorer.End(queueId); //指示恢复管理器事件队列的操作已经全部完成

                future.SetResult(source); //设置返回结果
            }
            return future;
        }


        private static void RaiseLocalEvent(EventEntry entry, DomainEvent local)
        {
            entry.Status = EventStatus.Raising;
            local.Raise();
            //当本地事件执行完毕后，回调entry
            var result = local.GetArgs();
            entry.Callback(result);

            entry.Status = EventStatus.Raised;
        }

        private static void RaiseRemoteEvent(EventEntry entry)
        {
            entry.Status = EventStatus.Raising;
            var raiseResultEventName = EventUtil.GetRaiseResult(entry.EventName, entry.EventId);
            //先订阅触发事件的返回结果的事件
            EventPortal.Subscribe(raiseResultEventName, RaiseResultEventHandler.Instance);
            //再发布“触发事件”的事件
            var raiseEventName = EventUtil.GetRaise(entry.EventName);
            EventPortal.Publish(new RaiseEvent(raiseEventName, entry.GetRemotable())); //触发远程事件就是发布一个“触发事件”的事件 ，订阅者会收到消息后会执行触发操作
        }

        /// <summary>
        /// 接入事件
        /// </summary>
        /// <param name="event"></param>
        public static void Accept(DTObject @event)
        {
            var eventId = @event.GetValue<Guid>(EventEntry.EventIdProperty.Name);
            var eventName = @event.GetValue<string>(EventEntry.EventNameProperty.Name);
            var args = @event.GetObject("args");
            var source = EventFactory.GetLocalEvent(eventName, args, true);
            Start(eventId, source); //我们把调用方指定的事件编号作为本地的事件队列编号
        }


        public static void Continue(DTObject @event)
        {
            var queueId = @event.GetValue<Guid>(EventEntry.QueueId);
            var eventId = @event.GetValue<Guid>(EventEntry.EventIdProperty.Name);
            var eventName = @event.GetValue<string>(EventEntry.EventNameProperty.Name);
            var success = @event.GetValue<bool>("success");
            var message = @event.GetValue<string>("message");
            var args = @event.GetObject("args");

            if (success)
            {
                try
                {
                    if (CompleteRemoteEvent(queueId, eventName, eventId, args))
                    {
                        Raise(queueId);
                    }
                }
                catch (Exception ex)
                {
                    EventRestorer.Restore(queueId, eventName, eventId, ex.Message);
                }
            }
            else
            {
                EventRestorer.Restore(queueId, eventName, eventId, message);
            }
        }

        /// <summary>
        /// 完成远程事件
        /// </summary>
        /// <param name="event"></param>
        private static bool CompleteRemoteEvent(Guid queueId, string eventName, Guid eventId, DTObject args)
        {
            //为了保证幂等性，以下逻辑有的返回false是为了保证幂等性，只对致命错误抛出异常
            bool result = true;
            try
            {
                DataContext.NewScope(() =>
                {
                    var queue = EventQueue.Find(queueId);
                    if (queue.IsEmpty())
                    {
                        //有可能由于回逆或者事件全部完成而导致队列已经被删除了
                        result = false;
                        return;
                    }

                    var entry = queue.GetEntry(eventId);
                    if (entry.IsEmpty() ||
                        entry.Status != EventStatus.Raising)
                    {
                        //有可能由于回逆或者事件全部完成而导致entry已经被删除了或entry的状态不是Raising
                        result = false;
                        return;
                    }

                    if (entry.IsLocal) //本地事件是不能继续触发的，这明显是个致命错误
                        throw new DomainEventException(string.Format(Strings.ContinueNotWithLocal, entry.EventName));

                    //远程事件执行完毕后，回调entry
                    entry.Callback(args);
                    entry.Status = EventStatus.Raised;

                    EventQueue.Update(queue);
                }, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //释放资源，删除为了收取信息而建立的临时队列
                var raiseResultEventName = EventUtil.GetRaiseResult(eventName, eventId);
                EventPortal.Subscribe(raiseResultEventName, RaiseResultEventHandler.Instance);
            }
            return result;
        }


        #region 发布调用结果

        /// <summary>
        /// 发布事件调用失败的结果
        /// </summary>
        internal static void PublishRaiseFailed(Guid queueId, string eventName, Guid eventId, string message)
        {
            var @event = new RaiseResultEvent(queueId, eventName, eventId, false, message, string.Empty);
            EventPortal.Publish(@event);
        }

        /// <summary>
        /// 发布事件调用成功的结果
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="eventId"></param>
        /// <param name="message"></param>
        internal static void PublishRaiseSucceeded(Guid queueId, string eventName, Guid eventId,string argsCode)
        {
            //返回事件成功被执行的结果
            var @event = new RaiseResultEvent(queueId, eventName, eventId, true, string.Empty, argsCode);
            EventPortal.Publish(@event);
        }

        #endregion

    }
}
