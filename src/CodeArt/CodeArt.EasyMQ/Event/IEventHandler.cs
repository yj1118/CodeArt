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
        void Handle(string eventName, DTObject arg);
    }
}