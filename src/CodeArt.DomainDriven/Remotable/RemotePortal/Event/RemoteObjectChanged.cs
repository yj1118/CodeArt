using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeArt.DTO;
using CodeArt.EasyMQ.Event;

namespace CodeArt.DomainDriven
{
    public class RemoteObjectChanged : EventBase
    {
        public RemoteType RemoteType
        {
            get;
            private set;
        }

        public object Id
        {
            get;
            private set;
        }

        public RemoteObjectChanged(string eventName, RemoteType remoteType, object id)
            : base(eventName)
        {
            this.RemoteType = remoteType;
            this.Id = id;
        }

        public override DTObject GetRemotable()
        {
            var arg = DTObject.CreateReusable();
            arg["typeName"] = this.RemoteType.FullName;
            arg["id"] = this.Id;
            return arg;
        }

    }
}