using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeArt.DTO;
using CodeArt.Concurrent;
using CodeArt.EasyMQ.Event;

namespace CodeArt.DomainDriven
{
    internal sealed class RemoteObjectUpdated : RemoteObjectChanged
    {
        public RemoteObjectUpdated(RemoteType remoteType, object id)
            : base(GetEventName(remoteType), remoteType, id)
        {
        }

        private static string GetEventName(RemoteType remoteType)
        {
            return string.Format("{0}Updated", remoteType.FullName);
        }

        public static void Subscribe(RemoteType remoteType)
        {
            var eventName = GetEventName(remoteType);
            EventPortal.Subscribe(eventName, Handler);
        }

        public static void Cancel(RemoteType remoteType)
        {
            var eventName = GetEventName(remoteType);
            EventPortal.Cancel(eventName);
        }

        [SafeAccess]
        private class RemoteObjectUpdatedHandler : RemoteObjectHandler
        {
            public override void Handle(DTObject @event)
            {
                var arg = @event;

                UseDefines(arg, (define, id) =>
                {
                    RemotePortal.UpdateObject(define, id);
                });
            }
        }

        private static readonly RemoteObjectUpdatedHandler Handler = new RemoteObjectUpdatedHandler();
    }
}