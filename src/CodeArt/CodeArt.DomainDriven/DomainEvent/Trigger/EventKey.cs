using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven
{
    [DebuggerDisplay("EventName:{EventName},EventId:{EventId}")]
    internal class EventKey
    {

        public Guid EventId
        {
            get;
            private set;
        }

        public string EventName
        {
            get;
            private set;
        }

        public EventKey(Guid eventId, string eventName)
        {
            this.EventId = eventId;
            this.EventName = eventName;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(this.EventName);
        }

        public static readonly EventKey Empty = new EventKey(Guid.Empty, string.Empty);

    }
}