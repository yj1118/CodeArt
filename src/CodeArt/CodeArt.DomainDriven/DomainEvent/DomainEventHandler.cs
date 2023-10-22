using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using CodeArt.AppSetting;
using CodeArt.Concurrent;
using CodeArt.DTO;
using CodeArt.EasyMQ.Event;
using CodeArt.Runtime;
using CodeArt.EasyMQ;

namespace CodeArt.DomainDriven
{
    [SafeAccess]
    public abstract class DomainEventHandler : IEventHandler
    {
        public EventPriority Priority => EventPriority.Medium;

        public void Handle(string eventName, TransferData data)
        {
            InitIdentity(data.Info);
            Handle(data.Info);
        }

        protected abstract void Handle(DTObject arg);


        private void InitIdentity(DTObject arg)
        {
            var identity = arg.GetObject("identity");
            AppSession.Identity = identity;
        }
    }
}