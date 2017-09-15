using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using CodeArt.Concurrent;
using CodeArt.DTO;
using CodeArt.EasyMQ.Event;
using CodeArt.Runtime;

namespace CodeArt.DomainDriven
{
    [SafeAccess]
    public abstract class DomainEventHandler : IEventHandler
    {
        public abstract void Handle(DTObject @event);
    }
}