using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeArt.DTO;
using CodeArt.EasyMQ.Event;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven
{
    public sealed class RemoteObjectDeleted : RemoteObjectChanged
    {
        public RemoteObjectDeleted(RemoteType remoteType, object id)
            : base(GetEventName(remoteType), remoteType, id)
        {
        }

        private static string GetEventName(RemoteType remoteType)
        {
            return string.Format("{0}Deleted", remoteType.FullName);
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
        internal class RemoteObjectDeletedHandler : RemoteObjectHandler
        {
            public override void Handle(DTObject @event)
            {
                var arg = @event;

                UseDefines(arg, (define, id) =>
                {
                    RemotePortal.DeleteObject(define, id);
                });
            }
        }

        private static readonly RemoteObjectDeletedHandler Handler = new RemoteObjectDeletedHandler();
    }
}