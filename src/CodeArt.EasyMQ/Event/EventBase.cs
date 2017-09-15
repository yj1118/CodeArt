using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

using CodeArt;
using CodeArt.Util;
using CodeArt.DTO;

namespace CodeArt.EasyMQ.Event
{
    /// <summary>
    /// 事件基类，注意，事件类型的名称就代表着事件的名称
    /// </summary>
    public abstract class EventBase : IEvent
    {
        public string Name
        {
            get;
            private set;
        }

        public EventBase(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// 事件必须提供远程能力的版本
        /// </summary>
        /// <returns></returns>
        public abstract DTObject GetRemotable();

    }
}