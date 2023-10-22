using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;


namespace CodeArt.EasyMQ.Event
{
    /// <summary>
    /// 事件订阅器工厂
    /// </summary>
    public interface ISubscriberFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventName">订阅者订阅的事件</param>
        /// <param name="group">订阅者所属的分组</param>
        /// <returns></returns>
        ISubscriber Create(string eventName, string group);

        /// <summary>
        /// 移除订阅器
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="group"></param>
        ISubscriber Remove(string eventName, string group);

        IEnumerable<ISubscriber> GetAll();
    }
}
