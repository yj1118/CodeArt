using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;


namespace CodeArt.EasyMQ.Event
{
    /// <summary>
    /// 事件的订阅者
    /// </summary>
    public interface ISubscriber
    {
        /// <summary>
        /// 接收事件
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="branch"></param>
        void Accept();

        /// <summary>
        /// 停止接收事件
        /// </summary>
        void Stop();

        /// <summary>
        /// 为订阅者添加处理器
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="branch"></param>
        /// <param name="handler"></param>
        void AddHandler(IEventHandler handler);

        /// <summary>
        /// 释放事件资源
        /// </summary>
        void Cleanup();

    }
}
