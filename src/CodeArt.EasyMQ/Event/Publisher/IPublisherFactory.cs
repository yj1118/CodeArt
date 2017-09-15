using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;


namespace CodeArt.EasyMQ.Event
{
    /// <summary>
    /// 事件发布者
    /// </summary>
    public interface IPublisherFactory
    {
        IPublisher Create();
    }
}
