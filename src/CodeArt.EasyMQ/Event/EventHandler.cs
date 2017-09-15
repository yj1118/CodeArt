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

        public virtual void Handle(DTObject @event)
        {
            this.DynamicHandle(@event);
        }

        protected virtual void DynamicHandle(dynamic @event)
        {

        }
    }
}