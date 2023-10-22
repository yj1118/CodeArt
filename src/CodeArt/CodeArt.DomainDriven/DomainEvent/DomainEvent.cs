using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.EasyMQ.Event;
using CodeArt.Util;
using CodeArt.Runtime;

namespace CodeArt.DomainDriven
{
    [DTOClass()]
    public abstract class DomainEvent : IDomainEvent
    {
        public DomainEvent()
        {
        }

        public string EventName
        {
            get
            {
                return this.Tip.Name;
            }
        }

        public EventAttribute Tip
        {
            get
            {
                return EventAttribute.GetTip(this.GetType(), true);
            }
        }

        internal DTObject GetArgs()
        {
            return DTObject.Serialize(this);
        }

        internal void SetArgs(DTObject args)
        {
            args.Deserialize(this);
        }

        #region 事件

        private IEnumerable<string> _preEvents;

        /// <summary>
        /// 前置事件
        /// </summary>
        public IEnumerable<string> PreEvents
        {
            get
            {
                if (_preEvents == null)
                {
                    _preEvents = GetPreEvents();
                }
                return _preEvents;
            }
        }

        /// <summary>
        /// 获得前置事件
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<string> GetPreEvents()
        {
            return Array.Empty<string>();
        }


        private IEnumerable<string> _proEvents;

        /// <summary>
        /// 后置事件
        /// </summary>
        public IEnumerable<string> ProEvents
        {
            get
            {
                if (_proEvents == null)
                {
                    _proEvents = GetProEvents();
                }
                return _proEvents;
            }
        }

        /// <summary>
        /// 获得后置事件
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<string> GetProEvents()
        {
            return Array.Empty<string>();
        }

        /// <summary>
        /// 获取事件的参数
        /// </summary>
        /// <param name="preEventName"></param>
        internal DTObject GetArgs(string eventName)
        {
            if(eventName.EqualsIgnoreCase(this.EventName))
            {
                //获取事件自身的数据
                return this.GetArgs();
            }

            var args = DTObject.Create();
            FillArgs(eventName, args);

            UpdateCode();

            return args;
        }

        /// <summary>
        /// 更新条目码
        /// </summary>
        private void UpdateCode()
        {
            if (this.Entry != null)
            {
                this.Entry.ArgsCode = this.GetArgs().GetCode();
            }
        }

        /// <summary>
        /// 当前事件对应的条目
        /// </summary>
        internal EventEntry Entry
        {
            get;
            set;
        }


        /// <summary>
        /// 当前事件对应的条目
        /// </summary>
        //internal EventEntrySlim Entry
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// 接受一个事件调用完成后的结果
        /// </summary>
        /// <returns></returns>
        internal void ApplyResult(EventEntry entry, DTObject result)
        {
            var eventName = entry.EventName;
            if (eventName.EqualsIgnoreCase(this.EventName))
            {
                //接受自身事件触发的结果
                this.SetArgs(result);
            }
            else
            {
                entry.ArgsCode = result.GetCode();
            }
            this.EventCompleted(eventName, result);
            UpdateCode();
        }


        /// <summary>
        /// 填充事件的参数
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="args"></param>
        protected virtual void FillArgs(string eventName, DTObject args)
        {

        }

        /// <summary>
        /// 事件执行完毕之后触发该回调
        /// </summary>
        /// <param name="preEventName"></param>
        /// <param name="result"></param>
        protected virtual void EventCompleted(string eventName, DTObject result)
        {

        }


        #endregion


        public void Raise()
        {
            RaiseImplement();
        }

        /// <summary>
        /// 实现执行事件的方法
        /// </summary>
        /// <returns>如果领域事件没有返回值，那么返回null</returns>
        protected abstract void RaiseImplement();


        public void Reverse()
        {
            ReverseImplement();
        }

        /// <summary>
        /// 实现回逆事件的方法
        /// </summary>
        protected abstract void ReverseImplement();


        #region 全局事件

        /// <summary>
        /// 领域事件被成功执行完毕的事件
        /// </summary>
        internal static event Action<Guid, DomainEvent> Succeeded;


        public static void OnSucceeded(Guid queueId, DomainEvent @event)
        {
            if (Succeeded != null)
                Succeeded(queueId, @event);
        }

        /// <summary>
        /// 表示领域事件执行失败，但是成功恢复状态（还原到执行领域事件之前的状态）
        /// </summary>
        internal static event Action<Guid, EventFailedException> Failed;


        public static void OnFailed(Guid queueId, EventFailedException reason)
        {
            if (Failed != null)
                Failed(queueId, reason);
        }

        /// <summary>
        /// 表示领域事件执行失败，并且没有成功恢复的事件
        /// </summary>
        internal static event Action<Guid, EventErrorException> Error;


        public static void OnError(Guid queueId, EventErrorException ex)
        {
            if (Error != null)
                Error(queueId, ex);
        }

        public static Exception OnErrorNoQueue(Guid eventId)
        {
            var ex = EventErrorException.CreateNoQueue(eventId);
            OnError(Guid.Empty, ex);
            return ex;
        }


        #endregion

        internal static void Initialize()
        {
            EventLockRepository.Instance.Initialize();
            EventLogEntryRepository.Instance.Initialize();
            EventLogRepository.Instance.Initialize();
            EventMonitorRepository.Instance.Initialize();
            EventQueueRepository.Instance.Initialize();
        }


        #region 测试支持

        /// <summary>
        /// 指定事件条目，在debug模式下会自动测试
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<EventEntry> TestGetEventEntries()
        {
            return null;
        }

        #endregion

    }
}
