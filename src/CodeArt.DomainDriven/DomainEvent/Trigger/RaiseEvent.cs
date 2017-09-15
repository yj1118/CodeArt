using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeArt.DTO;
using CodeArt.EasyMQ.Event;

namespace CodeArt.DomainDriven
{
    internal class RaiseEvent : EventBase
    {
        private DTObject _remotable;

        public RaiseEvent(string eventName, DTObject remotable)
            : base(eventName)
        {
            _remotable = remotable;
        }

        public override DTObject GetRemotable()
        {
            return _remotable;
        }
    }
}