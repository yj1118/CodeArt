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
        /// 处理远程格式的事件
        /// </summary>
        /// <param name="event"></param>
        void Handle(DTObject @event);
    }
}