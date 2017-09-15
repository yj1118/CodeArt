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
    /// 可以供远程和本地订阅和发布的事件
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// 每一类事件都有一个唯一的名称
        /// </summary>
        string Name { get; }


        /// <summary>
        /// 事件必须提供远程能力的版本
        /// </summary>
        /// <returns></returns>
        DTObject GetRemotable();
    }
}