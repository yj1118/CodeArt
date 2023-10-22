using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using CodeArt.Concurrent;
using CodeArt.DTO;
using CodeArt.EasyMQ.Event;
using CodeArt.Runtime;
using CodeArt.Util;
using CodeArt.Log;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 
    /// </summary>
    [SafeAccess]
    public class ReverseEventHandler : DomainEventHandler
    {
        private ReverseEventHandler()
        {
        }

        protected override void Handle(DTObject arg)
        {
            try
            {
                EventListener.AskedToReverse(arg);
            }
            catch (EventRestoreException)
            {
                //如果抛出的恢复事件发生的异常，那么意味着发生了很严重的错误
                //此时我们要抛出异常，告诉消息队列不要回复ack,让管理员去处理
                throw;
            }
            catch (Exception ex)
            {
                //其他类型的错误写入日志，不抛出异常
                Logger.Fatal(ex);
            }
        }


        public static readonly ReverseEventHandler Instance = new ReverseEventHandler();

    }
}