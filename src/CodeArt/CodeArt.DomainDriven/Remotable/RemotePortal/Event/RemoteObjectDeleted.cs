using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeArt.DTO;
using CodeArt.EasyMQ.Event;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven
{
    public static class RemoteObjectDeleted
    {
        public static string GetEventName(RemoteType remoteType)
        {
            return RemoteActionName.ObjectDeleted(remoteType);
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
            protected override void Handle(DTObject arg)
            {
                UseDefines(arg, (define, id) =>
                {
                    RemotePortal.DeleteObject(define, id);
                });
            }
        }

        private static readonly RemoteObjectDeletedHandler Handler = new RemoteObjectDeletedHandler();
    }
}