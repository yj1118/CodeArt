using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.AppSetting;
using CodeArt.DTO;
using CodeArt.EasyMQ.Event;
using CodeArt.Log;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 事件保护器，可以确保事件要么完整的执行，要么回滚到初始状态，也就是确保事件的原子性
    /// </summary>
    internal static class EventProtector
    {
        /// <summary>
        /// 使用一个新队列
        /// </summary>
        /// <param name="queueId"></param>
        /// <param name="action"></param>
        /// <param name="error"></param>
        public static void UseNewQueue(Guid queueId, Action<EventCallback> action, Action<Exception> error = null)
        {
            UseQueue(queueId, true, action, error);
        }

        /// <summary>
        /// 使用已存在的队列
        /// </summary>
        /// <param name="queueId"></param>
        /// <param name="action"></param>
        /// <param name="error"></param>
        public static void UseExistedQueue(Guid queueId, Action<EventCallback> action, Action<Exception> error = null)
        {
            UseQueue(queueId, false, action, error);
        }

        /// <summary>
        /// 使用队列
        /// </summary>
        /// <param name="queueId">队列编号</param>
        /// <param name="newQueue">是否新建队列</param>
        /// <param name="action">使用队列完成的任务</param>
        /// <param name="error">发生错误时的回调</param>
        private static void UseQueue(Guid queueId,bool newQueue, Action<EventCallback> action, Action<Exception> error)
        {
            EventCallback callBack = new EventCallback();
            Exception exception = null;
            try
            {
                DataContext.NewScope(() =>
                {
                    var @lock = newQueue ? EventLock.GetOrCreate(queueId)
                                      : EventLock.Find(queueId, QueryLevel.Single);//这段代码将入口锁住，所以监视器和队列也间接被锁住了

                    if (@lock.IsEmpty()) return; //无相关的信息，直接返回（保证幂等性，有可能队列被多次重复处理）

                    var monitor = newQueue ? EventMonitor.GetOrCreate(queueId)
                                         : EventMonitor.Find(queueId);

                    if (monitor.IsEmpty()) return;
                    if (monitor.Interrupted) return; //监视器被中断，那么需要等待恢复，不必执行队列

                    monitor.Interrupted = true;
                    EventMonitor.FlushUpdate(monitor); //主动将监视器设置为中断的，这样后续的操作中如果出错或者中途断电，监视器就是中断的了，可以被恢复

                    action(callBack);

                    if (callBack.HasAction)
                    {
                        //执行注册的回调方法
                        callBack.Execute();
                    }

                    monitor.Interrupted = false;
                    EventMonitor.Update(monitor); //修改监视器为不中断的，但是此处不提交，而是跟整体操作一起提交才生效
                });

            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                try
                {
                    Logger.Fatal(exception);
                    //自定义错误处理
                    if (error != null)
                        error(exception);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    //恢复
                    TryRestore(queueId, exception, true);
                }
            }
        }

        #region 恢复


        private static void Restore(Guid queueId, Exception reason)
        {
            try
            {
                Guid logId = queueId;
                //找到日志
                var log = EventLog.Find(logId);
                if (log.IsEmpty() || log.Status == EventLogStatus.Reversed) return; //表示已经回逆了

                AppSession.Identity = log.GetIdentity(); //初始化会话身份

                //找到日志下所有的条目
                var repository = EventLogEntryRepository.Instance;
                var entries = repository.FindByReverseOrder(logId);

                //根据每项条目恢复
                foreach (var entry in entries)
                {
                    if (entry.IsReversed) continue;
                    //为日志条目的回逆创建独立的事务
                    DataContext.NewScope(() =>
                    {
                        Rollback(log, entry, reason);
                        entry.IsReversed = true;
                        EventLogEntry.Update(entry);
                    });
                }

                log.Status = EventLogStatus.Reversed;
                EventLog.FlushUpdate(log);
            }
            catch (Exception ex)
            {
                //恢复期间发生了错误
                var e = new EventRestoreException(string.Format(Strings.RecoveryEventFailed, queueId), ex);
                //写入日志
                Logger.Fatal(e);
                throw e;
            }
        }

        private static void Rollback(EventLog log, EventLogEntry logEntry, Exception reason)
        {
            var queueId = log.Id;
            if (logEntry.Operation == EventOperation.Raise)
            {
                var content = DTObject.Create(logEntry.ContentCode);
                var entryId = content.GetValue<Guid>("entryId");

                var queue = EventQueue.Find(queueId);
                if (queue.IsEmpty()) return;
                var entry = queue.GetEntry(entryId);
                if (!entry.IsEmpty())
                {
                    Reverse(queue, entry);
                }
                EventQueue.Update(queue);

            }
            else if (logEntry.Operation == EventOperation.Start)
            {
                EventQueue.Delete(queueId);
            }
        }

        private static void Reverse(EventQueue queue, EventEntry entry)
        {
            if (entry.IsEmpty() ||
                entry.Status == EventStatus.Reversed)//已回逆的不用回逆，保证幂等性 
                return;

            var args = entry.GetArgs();
            if (entry.IsLocal)
            {
                ReverseLocalEvent(entry, args);
            }
            else
            {
                var identity = queue.GetIdentity();
                ReverseRemoteEvent(entry, identity, args);
            }
        }

        private static void ReverseLocalEvent(EventEntry entry, DTObject args)
        {
            var local = EventFactory.GetLocalEvent(entry, args, true);
            entry.Status = EventStatus.Reversing;
            local.Reverse();
            entry.Status = EventStatus.Reversed;
        }

        private static void ReverseRemoteEvent(EventEntry entry, dynamic identity, DTObject args)
        {
            entry.Status = EventStatus.Reversing;

            if (entry.IsMockRemote)
            {
                //模拟远程回逆
                Task.Run(() =>
                {
                    var local = EventFactory.GetLocalEvent(entry, args, true);
                    local.Reverse();
                });
            }
            else
            {
                //调用远程事件时会创建一个接收结果的临时队列，有可能该临时队列没有被删除，所以需要在回逆的时候处理一次
                EventTrigger.CleanupRemoteEventResult(entry.EventName, entry.EventId);

                //发布“回逆事件”的事件
                var reverseEventName = EventUtil.GetReverse(entry.EventName);
                EventPortal.Publish(reverseEventName, entry.GetRemotable(identity, args));
            }

            //注意，与“触发事件”不同，我们不需要等待回逆的结果，只用传递回逆的消息即可
            entry.Status = EventStatus.Reversed;
        }


        #endregion

        #region 后台恢复（用于机器故障，断电等意外情况）

        /// <summary>
        /// 后台恢复（用于机器故障，断电等意外情况）
        /// </summary>
        public static void RestoreAsync()
        {
            Task.Run(()=>
            {
                try
                {
                    AppSession.Using(() =>
                    {
                        while (true)
                        {
                            bool need = false;
                            IEnumerable<Guid> monitorIds = null;
                            DataContext.Using(()=>
                            {
                                monitorIds = EventMonitor.Top10Interrupteds();
                            });
                            if (monitorIds.Count() == 0) break;
                            Parallel.ForEach(monitorIds, (monitorId) =>
                            {
                                var reason = new HardwareFailureException(Strings.HardwareFailureTip);
                                var success = TryRestore(monitorId, reason, false); //监视器的编号就是队列编号，后台只能读取中断的队列，但是中断有两种情况：1.机器故障导致服务器重启，2.监视器正在运行，所以，这里不会强制处理正在运行的监视器
                                if (success) need = true; //只要有一个需要恢复，那么就需要继续找新一轮的监视器进行恢复
                            });

                            if (!need) break;//没有需要恢复的，那么返回
                        }
                    }, true);
                }
                catch (Exception ex)
                {
                    Logger.Fatal(ex);
                }
            });
        }

        /// <summary>
        /// 有两种情况会恢复：
        /// 1.调用方主动要求恢复（这一般是因为调用方出了错误，要求被调用的领域事件恢复）,这时候本地的事件队列就算是成功执行完毕的（非中断），也需要恢复
        /// 2.本地机器故障或业务错误，要求恢复
        /// </summary>
        /// <param name="queueId"></param>
        /// <param name="reason"></param>
        /// <param name="forceRestore">即使监视器不会中断的，也要回逆，这一般是因为调用方发布的回逆要求</param>
        /// <returns></returns>
        public static bool TryRestore(Guid queueId, Exception reason, bool forceRestore)
        {
            bool success = false; //是否成功恢复
            try
            {
                DataContext.NewScope(() =>
                {
                    var @lock = EventLock.Find(queueId, QueryLevel.Single);//这段代码将入口锁住，所以监视器和队列也间接被锁住了
                    if (@lock.IsEmpty()) return; //没有锁，那么意味着队列及相关的数据已被恢复了

                    var monitor = EventMonitor.Find(queueId);

                    var queue = EventQueue.Find(queueId);
                    if (queue.IsEmpty())
                    {
                        //队列不存在，要么是队列已被恢复了，要么就是队列还没初始化之前，就被终止了
                        //删除监视器
                        if (!monitor.IsEmpty())
                            EventMonitor.Delete(monitor);
                        //删除锁
                        EventLock.Delete(@lock);
                        success = true;
                        return;
                    }


                    if (!forceRestore && !monitor.Interrupted)
                    {
                        //由于中断的监视器有可能是正在运行中的(正在执行的队列我们会把监视器设置为中断，当队列执行完毕后恢复为非中断)，所以这类监视器运行完毕后，就不再中断了
                        //所以就不需要恢复
                        success = false;
                        return;
                    }

                    //防止恢复过程中也断电或故障，这里主动设置为中断的
                    monitor.Interrupted = true;
                    EventMonitor.FlushUpdate(monitor);

                    Restore(queueId, reason);

                    //恢复完毕后，删除监视器
                    EventMonitor.Delete(monitor);
                    //删除锁
                    EventLock.Delete(@lock);
                });

                DomainEvent.OnFailed(queueId, new EventFailedException(reason));
                success = true;
            }
            catch (EventRestoreException)
            {
                //Restore方法已写入日志，就不再写入
                DomainEvent.OnError(queueId, new EventErrorException(reason));
                throw;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
                DomainEvent.OnError(queueId, new EventErrorException(reason));
            }
            return success;
        }


        #endregion

    }
}
