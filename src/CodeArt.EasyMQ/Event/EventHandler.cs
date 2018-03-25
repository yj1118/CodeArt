using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

using CodeArt;
using CodeArt.Runtime;
using CodeArt.DTO;

namespace CodeArt.EasyMQ.Event
{
    public class EventHandler : IEventHandler
    {
        protected EventHandler() { }

        public virtual void Handle(string eventName, DTObject arg)
        {
            this.DynamicHandle(eventName, arg);
        }

        protected virtual void DynamicHandle(string eventName,  dynamic arg)
        {

        }
    }
}