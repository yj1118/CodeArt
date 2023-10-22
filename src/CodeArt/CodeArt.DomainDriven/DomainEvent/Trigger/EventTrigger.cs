using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.EasyMQ.Event;
using CodeArt.Runtime;
using CodeArt.DTO;
using CodeArt.Concurrent;
using CodeArt.AppSetting;
using System.Timers;
using CodeArt.Util;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 事件触发器
    /// </summary>
    internal static class EventTrigger
    {
        /// <summary>
        /// 开始一个全新的事件
        /// </summary>
        /// <param name="source"></param>
        public static void Start(Guid queueId, DomainEvent source,EventCallback callback)
        {
            Start(queueId, source, false, callback);
        }

        /// <summary>
        /// 开始一个新的事件
        /// </summary>
        /// <param name="event"></param>
        public static void Start(Guid queueId, DomainEvent source, bool isSubqueue, EventCallback callback)
        {
            var queue = CreateQueue(queueId, source, isSubqueue);
            Raise(queue, callback);
        }

        private static EventQueue CreateQueue(Guid queueId, DomainEvent source, bool isSubqueue)
        {
            EventQueue queue = null;
            DataContext.NewScope(() =>
            {
                queue = EventQueue.Create(queueId, source, isSubqueue, AppContext.Identity);
                EventLog.Create(queue);
            });
            return queue;
        }


        /// <summary>
        /// 触发队列事件
        /// </summary>
        /// <param name="queue"></param>
        private static void Raise(EventQueue queue, EventCallback callback)
        {
            bool isSucceeded = false;
            bool isRunning = false;

            while (!isRunning && !isSucceeded)
            {
                DataContext.NewScope(() =>
                {
                    //触发队列事件
                    var entry = queue.GetPreRaise();
                    if (!entry.IsEmpty())
                    {
                        var args = entry.GetArgs(); //获取条目的事件参数
                        EventLog.FlushRaise(queue, entry); //一定要确保日志先被正确的写入，否则会有BUG
                        if (entry.IsLocal)
                        {
                            var source = entry.GetSourceEvent();
                            var local = EventFactory.GetLocalEvent(entry, args, true);
                            RaiseLocalEvent(entry, local, source);
                        }
                        else
                        {
                            var identity = queue.GetIdentity();
                            RaiseRemoteEvent(entry, identity, args);
                        }
                    }

                    EventQueue.Update(queue); //这里队列的修改和业务的处理共享一个事务，因为业务的处理会影响队列的状态改变，两者必须原子性，要么一起成功，要么一起失败
                    isRunning = queue.IsRunning;
                    isSucceeded = queue.IsSucceeded;
                });
            }

            bool completed = false;
            DomainEvent @event = null;
            if (isSucceeded)
            {
                if (queue.IsSubqueue)
                {
                    var entry = queue.Source;
                    var argsCode = entry.ArgsCode;

                    var identity = queue.GetIdentity();
                    var eventName = entry.EventName;
                    var eventId = entry.EventId;
                    callback.Mount(() => //挂载回调事件，这样所有操作执行完毕后，会发布事件被完成的消息
                    {
                        //发布事件被完成的消息
                        PublishRaiseSucceeded(identity, eventName, eventId, argsCode);
                    });
                }
                else
                {
                    //不是被外界调用的，所以整个事件场景已完成
                    completed = true;
                    @event = queue.Source.GetSourceEvent();
                }

                EventLog.FlushEnd(queue.Id); //指示恢复管理器事件队列的操作已经全部完成
            }

            if (completed)
            {
                callback.Mount(() => //挂载回调事件
                {
                    DomainEvent.OnSucceeded(queue.Id, @event);
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="local">local是entry对应的本地事件</param>
        /// <param name="source">source是entry所属的事件源</param>
        /// <param name=""></param>
        private static void RaiseLocalEvent(EventEntry entry, DomainEvent local, DomainEvent source)
        {
            entry.Status = EventStatus.Raising;
            local.Raise();

            //当本地事件执行完毕后，执行它所在的源事件的PreEventCompleted方法
            var result = local.GetArgs();
            source.ApplyResult(entry, result); //无论是本地事件还是远程事件，调用后的结果都是由源事件存储

            entry.Status = EventStatus.Raised;
        }

        private static void RaiseRemoteEvent(EventEntry entry, dynamic identity, DTObject args)
        {
            entry.Status = EventStatus.Raising;
            var eventKey = new EventKey(entry.EventId, entry.EventName);
            if (entry.IsMockRemote)
            {
                //模拟远程
                DTObject @event = null;
                Task.Run(()=>
                {
                    try
                    {
                        DataContext.NewScope(() =>
                        {
                            var local = EventFactory.GetLocalEvent(entry, args, true);
                            local.Raise();
                            var result = local.GetArgs();
                            var argsCode = result.GetCode();
                            @event = CreatePublishRaiseResultArg(identity, eventKey, true, string.Empty, false, argsCode);
                        });
                    }
                    catch (Exception ex)
                    {
                        @event = CreatePublishRaiseResultArgError(identity, eventKey, ex);
                    }
                    finally
                    {
                        EventListener.Receive(@event);
                    }
                });
            }
            else
            {
                //先订阅触发事件的返回结果的事件
                SubscribeRemoteEventResult(entry.EventName, entry.EventId);

                //再发布“触发事件”的事件
                var raiseEventName = EventUtil.GetRaise(entry.EventName);
                EventPortal.Publish(raiseEventName, entry.GetRemotable(identity, args)); //触发远程事件就是发布一个“触发事件”的事件 ，订阅者会收到消息后会执行触发操作
            }

            TimeoutManager.Start(eventKey);
        }

        /// <summary>
        /// 订阅远程事件的返回结果的事件
        /// </summary>
        internal static void SubscribeRemoteEventResult(string eventName, Guid eventId)
        {
            var raiseResultEventName = EventUtil.GetRaiseResult(eventName, eventId);
            EventPortal.Subscribe(raiseResultEventName, ReceiveResultEventHandler.Instance, true);
        }

        /// <summary>
        /// 删除由于接受调用结果而创建的临时队列
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="eventId"></param>
        internal static void CleanupRemoteEventResult(string eventName, Guid eventId)
        {
            var raiseResultEventName = EventUtil.GetRaiseResult(eventName, eventId);
            EventPortal.Cleanup(raiseResultEventName);
        }

        public static void Continue(Guid queueId, DTObject @event, EventKey key, EventCallback callback)
        {
            var queue = CompleteRemoteEvent(queueId, @event, key);
            if(queue != null) //为null表示超时了
                Raise(queue, callback);
        }

        /// <summary>
        /// 执行远程事件超时
        /// </summary>
        /// <param name="queueId"></param>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        public static void Timeout(Guid queueId, EventKey key, EventCallback callback)
        {
            if (!TimeoutManager.End(key)) return; //如果结束失败，证明已完成了

            //首先释放资源，删除为了收取信息而建立的临时队列
            CleanupRemoteEventResult(key.EventName, key.EventId);

            //记录异常状态
            var queue = EventQueue.Find(queueId);
            if (queue.IsEmpty()) return;

            var entry = queue.GetEntry(key.EventId);
            if (entry.IsEmpty() || entry.IsLocal) return;

            entry.Status = EventStatus.TimedOut;

            EventQueue.FlushUpdate(queue);

            //抛出执行失败的异常
            throw new RemoteEventFailedException(string.Format(Strings.ExecuteRemoteEventTimeout, key.EventName));
        }

        /// <summary>
        /// 完成远程事件
        /// </summary>
        /// <param name="event"></param>
        private static EventQueue CompleteRemoteEvent(Guid queueId, DTObject @event, EventKey key)
        {
            if (!TimeoutManager.End(key)) return null; //如果结束失败，证明已超时了

            //首先释放资源，删除为了收取信息而建立的临时队列
            CleanupRemoteEventResult(key.EventName, key.EventId);

            //再处理结果
            var success = @event.GetValue<bool>("success");
            var message = @event.GetValue<string>("message");

            if (!success)
            {
                var ui = @event.GetValue<bool>("ui"); //ui错误
                //如果没有执行成功，那么抛出异常
                if(ui) throw new RemoteBusinessFailedException(message);
                throw new RemoteEventFailedException(message);
            }

            var args = @event.GetObject("args");

            var queue = EventQueue.Find(queueId);
            if (queue.IsEmpty())
            {
                //短期内事件队列不存在只有一个原因，那就是由于回逆而被删除了
                //长期内事件不存在，那是因为为防止数据过多导致性能下降，已完成的过期队列会被删除，这种情况下不可能被继续调用，不会执行到这里来
                throw new DomainEventException(string.Format(Strings.QueueNotExistWithCallbackTip, queue.Id));
            }

            var entry = queue.GetEntry(key.EventId);
            if (entry.IsEmpty())
            {
                throw new DomainEventException(string.Format(Strings.EventEntryNotExistWithCallbackTip, queue.Id, entry.EventId));
            }

            if (entry.IsLocal) //本地事件是不能继续触发的，这明显是个致命错误
                throw new DomainEventException(string.Format(Strings.ContinueNotWithLocal, entry.EventName));

            if (entry.Status == EventStatus.TimedOut) //已经超时了，抛出异常
                throw new RemoteEventFailedException(string.Format(Strings.ExecuteRemoteEventTimeout, entry.EventName));

            //远程事件执行完毕后，用它所在的源事件接受结果
            var source = entry.GetSourceEvent();
            source.ApplyResult(entry, args);

            entry.Status = EventStatus.Raised;

            EventQueue.FlushUpdate(queue);

            return queue;
        }


        #region 发布调用结果

        /// <summary>
        /// 发布事件调用失败的结果
        /// </summary>
        internal static void PublishRaiseFailed(dynamic identity, EventKey key, Exception ex)
        {
            var en = EventUtil.GetRaiseResult(key.EventName, key.EventId);//消息队列的事件名称
            var arg = CreatePublishRaiseResultArgError(identity, key, ex);
            EventPortal.Publish(en, arg);
        }

        private static DTObject CreatePublishRaiseResultArgError(dynamic identity, EventKey key, Exception ex)
        {
            var message = ex.GetCompleteInfo();
            var isBusinessException = ex.IsUserUIException();

            return CreatePublishRaiseResultArg(identity, key, false, message, isBusinessException, string.Empty);
        }


        /// <summary>
        /// 发布事件调用成功的结果
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="eventId"></param>
        /// <param name="message"></param>
        internal static void PublishRaiseSucceeded(dynamic identity, string eventName, Guid eventId, string argsCode)
        {
            var en = EventUtil.GetRaiseResult(eventName, eventId); //消息队列的事件名称
            //返回事件成功被执行的结果
            var arg = CreatePublishRaiseResultArg(identity, new EventKey(eventId, eventName), true, string.Empty,false, argsCode);
            EventPortal.Publish(en, arg);
        }

        /// <summary>
        /// 得到发布事件调用结果的参数
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="eventName"></param>
        /// <param name="eventId"></param>
        /// <param name="success"></param>
        /// <param name="message"></param>
        /// <param name="argsCode"></param>
        /// <returns></returns>
        private static DTObject CreatePublishRaiseResultArg(DTObject identity, EventKey key,bool success, string message,bool isBusinessException, string argsCode)
        {
            var arg = DTObject.Create();
            arg["eventName"] = key.EventName;
            arg["eventId"] = key.EventId;
            arg["success"] = success;
            arg["message"] = message;
            arg["ui"] = isBusinessException;
            arg["args"] = DTObject.Create(argsCode);
            arg["identity"] = identity.Clone();
            return arg;
        }

        #endregion

    }
}
