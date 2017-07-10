using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

using CodeArt;
using CodeArt.Util;
using CodeArt.DTO;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 领域事件
    /// </summary>
    public abstract class DomainEvent : IDomainEvent
    {
        /// <summary>
        /// 领域事件参数必须提供远程能力的版本
        /// </summary>
        /// <returns></returns>
        public abstract DTObject GetRemotable();

    }
}