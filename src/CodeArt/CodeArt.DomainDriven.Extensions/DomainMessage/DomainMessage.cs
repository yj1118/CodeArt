using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodeArt.AppSetting;
using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using CodeArt.DTO;
using CodeArt.EasyMQ;
using CodeArt.EasyMQ.Event;
using CodeArt.Log;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 消息
    /// </summary>
    [ObjectRepository(typeof(IDomainMessageRepository), CloseMultiTenancy = true)]
    [ObjectValidator(typeof(DomainMessageSpecification))]
    public class DomainMessage : AggregateRoot<DomainMessage, Guid, DomainMessage.DomainMessageEmpty>
    {
        [PropertyRepository()]
        [StringLength(0, 200)]
        [ASCIIString]
        private static readonly DomainProperty EventNameProperty = DomainProperty.Register<string, DomainMessage>("EventName");

        /// <summary>
        /// 事件名称
        /// </summary>
        public string EventName
        {
            get
            {
                return GetValue<string>(EventNameProperty);
            }
            private set
            {
                SetValue(EventNameProperty, value);
            }
        }


        [PropertyRepository()]
        private static readonly DomainProperty ArgCodeProperty = DomainProperty.Register<string, DomainMessage>("ArgCode");

        /// <summary>
        /// 参数代码
        /// </summary>
        public string ArgCode
        {
            get
            {
                return GetValue<string>(ArgCodeProperty);
            }
            private set
            {
                SetValue(ArgCodeProperty, value);
            }
        }

        [PropertyRepository]
        private static readonly DomainProperty CreateTimeProperty = DomainProperty.Register<DateTime, DomainMessage>("CreateTime");

        /// <summary>
        /// 消息发送的时间
        /// </summary>
        public DateTime CreateTime
        {
            get
            {
                return GetValue<DateTime>(CreateTimeProperty);
            }
            private set
            {
                SetValue(CreateTimeProperty, value);
            }
        }

        [PropertyRepository]
        private static readonly DomainProperty StatusProperty = DomainProperty.Register<DomainMessageStatus, DomainMessage>("Status");

        /// <summary>
        /// 消息的状态
        /// </summary>
        public DomainMessageStatus Status
        {
            get
            {
                return GetValue<DomainMessageStatus>(StatusProperty);
            }
            private set
            {
                SetValue(StatusProperty, value);
            }
        }


        [PropertyRepository]
        private static readonly DomainProperty SyncProperty = DomainProperty.Register<bool, DomainMessage>("Sync");

        /// <summary>
        /// 是否同步发送消息，大多数情况是不需要同步发送消息的，极少情况下
        /// 可以用同步发送消息来满足特点场景的需要，比如刚发送消息，就需要在UI刷新呈现消息内容
        /// 这就需要及时性高，需要在发送消息时同步等待
        /// </summary>
        public bool Sync
        {
            get
            {
                return GetValue<bool>(SyncProperty);
            }
            private set
            {
                SetValue(SyncProperty, value);
            }
        }


        public override void OnAddCommitted()
        {
            base.OnAddCommitted();

            //当提交成功后，自动发布消息
            this.Send(this.Sync);

            //由于事务已经提交，但是还处于当前数据上下文中，所以以下代码新开数据上下文
            DataContext.NewScope(() =>
            {
                //发布成功后，紧接着修改消息的状态，消息如果删除失败了，会导致消息被重发，这就需要消息的接收方具有幂等性
                this.Status = DomainMessageStatus.Sent;
                var repository = Repository.Create<IDomainMessageRepository>();
                repository.Update(this);
            });
        }

        internal static string GetRaiseResultEventName(Guid msgId)
        {
            return string.Format("DomainMessageResult-{0}", msgId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sync">true:同步发送消息，发送方需要等待对方响应,false:异步发送消息</param>
        private void Send(bool sync)
        {
            var arg = DTObject.Create(this.ArgCode);
            arg["DomainMessageId"] = this.Id; //传递领域消息的编号，接收方可以通过该编号识别是否重复处理
            arg["Sync"] = sync;
            if(sync)
            {
                using (var handler = new DomainMessageSyncHandler())
                {
                    var raiseResultEventName = GetRaiseResultEventName(this.Id);

                    try
                    {
                        SubscribeMessageResult(raiseResultEventName, handler);
                        EventPortal.Publish(this.EventName, arg);

                        var error = handler.Wait();

                        if (!string.IsNullOrEmpty(error))
                        {
                            throw new BusinessException(error);
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        CleanupMessageResult(raiseResultEventName);
                    }
                }
            }
            else
            {
                //异步发送
                EventPortal.Publish(this.EventName, arg);
            }
        }


        #region 同步发送消息

        private void SubscribeMessageResult(string eventName, DomainMessageSyncHandler handle)
        {
            EventPortal.Subscribe(eventName, handle, true);
        }

        private static void CleanupMessageResult(string eventName)
        {
            EventPortal.Cleanup(eventName);
        }

        [SafeAccess]
        private class DomainMessageSyncHandler : IEventHandler,IDisposable
        {
            public EventPriority Priority => EventPriority.Medium;

            private AutoResetEvent _signal;
            private string _error = null;
            private bool _timeOut = true;

            public DomainMessageSyncHandler()
            {
                _signal = new AutoResetEvent(false);
            }

            public void Handle(string eventName, TransferData data)
            {
                _timeOut = false;
                var arg = data.Info;
                _error = data.Info.GetValue<string>("error", string.Empty);
                _signal.Set();  //收到消息服务器的返回
            }

            public string Wait()
            {
                _signal.WaitOne(30000); //最多等待30秒

                if (_timeOut)
                    return "等待结果超时";

                return _error;
            }

            public void Dispose()
            {
                _signal.Dispose();
            }
        }

        #endregion

        internal DomainMessage(Guid id, string eventName, string argCode, bool sync)
           : base(id)
        {
            this.EventName = eventName;
            this.ArgCode = argCode;
            this.CreateTime = DateTime.Now;
            this.Status = DomainMessageStatus.Idle;
            this.Sync = sync;

            this.OnConstructed();
        }

        [ConstructorRepository]
        internal DomainMessage(Guid id, string eventName, string argCode, DateTime createTime, DomainMessageStatus status, bool sync)
           : base(id)
        {
            this.EventName = eventName;
            this.ArgCode = argCode;
            this.CreateTime = createTime;
            this.Status = status;
            this.Sync = sync;

            this.OnConstructed();
        }

        public class DomainMessageEmpty : DomainMessage
        {
            public DomainMessageEmpty()
                : base(Guid.Empty, string.Empty, string.Empty, false)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }


        /// <summary>
        /// 通知消息，请与具体业务对象共享一个事务，确保业务成功后，消息才发送
        /// </summary>
        /// <param name="sounderId"></param>
        /// <param name="soundId"></param>
        public static void Notice(string eventName, DTObject arg, bool sync = false)
        {
            //以下代码，当msg真实被提交后，就会触发对应的事件，发布消息
            var msg = new DomainMessage(Guid.NewGuid(), eventName, arg.GetCode(false, false), sync);
            var repository = Repository.Create<IDomainMessageRepository>();
            repository.Add(msg);
        }

        /// <summary>
        /// 获得已中断的消息，就是由于断电、故障等原因，消息没有发送出去或者虽然发送出去了，但是消息没有成功改变状态
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Guid> Top10Idles()
        {
            var repository = Repository.Create<IDomainMessageRepository>();
            return repository.Top10Idles();
        }

        #region 继续发送（用于机器故障，断电等意外情况）

        private static void Send(Guid msgId)
        {
            DataContext.Using(() =>
            {
                var repository = Repository.Create<IDomainMessageRepository>();
                var msg = repository.Find(msgId, QueryLevel.Single);
                msg.Send(false); //恢复不需要同步,也就是说，恢复的时候发送领域消息，是不需要等待对方发送响应结果的
                msg.Status = DomainMessageStatus.Sent;
                repository.Update(msg);
            });
        }


        /// <summary>
        /// 后台继续发送消息（用于机器故障，断电等意外情况）
        /// </summary>
        internal static void Continue()
        {
            Task.Run(() =>
            {
                try
                {
                    AppSession.Using(() =>
                    {
                        while (true)
                        {
                            IEnumerable<Guid> msgIds = null;
                            DataContext.Using(() =>
                            {
                                msgIds = Top10Idles();
                            });
                            if (msgIds.Count() == 0) break;
                            Parallel.ForEach(msgIds, (msgId) =>
                            {
                                Send(msgId);
                            });
                        }
                    }, true);
                }
                catch (Exception ex)
                {
                    Logger.Fatal(ex);
                }
            });
        }

        internal static void DeleteExpired()
        {
            try
            {
                AppSession.Using(() =>
                {
                    DataContext.Using(() =>
                    {
                        var repository = Repository.Create<IDomainMessageRepository>();
                        repository.DeleteExpired(30); //删除30天之前的数据，也就是说只保留最近30天的数据
                    });
                }, true);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }
        }

        #endregion

    }

    public enum DomainMessageStatus
    {
        /// <summary>
        /// 闲置，还未发送
        /// </summary>
        Idle = 1,
        /// <summary>
        /// 已发送
        /// </summary>
        Sent = 2
    }

}
