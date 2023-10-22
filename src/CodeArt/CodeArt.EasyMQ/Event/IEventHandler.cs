using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

using CodeArt;
using CodeArt.DTO;

namespace CodeArt.EasyMQ.Event
{
    /// <summary>
    /// 远程事件处理器
    /// </summary>
    public interface IEventHandler
    {
        /// <summary>
        /// 处理事件
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="arg"></param>
        void Handle(string eventName, TransferData data);

        EventPriority Priority
        {
            get;
        }

    }


    public enum EventPriority : byte
    {
        /// <summary>
        /// 低优先级
        /// </summary>
        Low = 1,
        /// <summary>
        /// 中等优先级
        /// </summary>
        Medium = 2,
        /// <summary>
        /// 高优先级
        /// </summary>
        High =3
    }

}