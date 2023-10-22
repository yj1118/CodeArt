using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeArt.DTO;
using CodeArt.Concurrent;
using CodeArt.EasyMQ.Event;

namespace CodeArt.DomainDriven
{
    internal static class RemoteObjectUpdated
    {
        public static string GetEventName(RemoteType remoteType)
        {
            return RemoteActionName.ObjectUpdated(remoteType);
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
            protected override void Handle(DTObject arg)
            {
                UseDefines(arg, (define, id) =>
                {
                    RemotePortal.UpdateObject(define, id);
                });
            }
        }

        private static readonly RemoteObjectUpdatedHandler Handler = new RemoteObjectUpdatedHandler();
    }
}