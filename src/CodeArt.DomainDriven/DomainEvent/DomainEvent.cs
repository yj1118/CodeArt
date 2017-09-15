using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.EasyMQ.Event;

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
            return DTObject.Serialize(this, false);
        }

        internal void SetArgs(DTObject args)
        {
            args.Deserialize(this);
        }

        #region 前置事件

        private IEnumerable<string> _preEvents;

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

        protected virtual IEnumerable<string> GetPreEvents()
        {
            return Array.Empty<string>();
        }

        /// <summary>
        /// 获取前置事件的参数
        /// </summary>
        /// <param name="preEventName"></param>
        public DTObject GetArgs(string preEventName)
        {
            var args = DTObject.CreateReusable();
            FillArgs(preEventName, args);
            return args;
        }


        /// <summary>
        /// 填充前置事件的参数
        /// </summary>
        /// <param name="preEventName"></param>
        /// <param name="args"></param>
        protected virtual void FillArgs(string preEventName, DTObject args)
        {

        }

        /// <summary>
        /// 当前置事件执行完毕之后触发该回调
        /// </summary>
        /// <param name="preEventName"></param>
        /// <param name="result"></param>
        public virtual void Callback(string preEventName, DTObject result)
        {

        }


        #endregion


        public abstract void Raise();

        public abstract void Reverse();

    }
}
