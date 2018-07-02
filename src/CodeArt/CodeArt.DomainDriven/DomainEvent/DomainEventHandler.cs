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

namespace CodeArt.DomainDriven
{
    [SafeAccess]
    public abstract class DomainEventHandler : IEventHandler
    {
        public void Handle(string eventName, DTObject arg)
        {
            InitIdentity(arg);
            Handle(arg);
        }

        protected abstract void Handle(DTObject arg);


        private void InitIdentity(DTObject arg)
        {
            var identity = arg.GetObject("identity");
            AppSession.Identity = identity;
        }
    }
}