using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;

namespace CodeArt.Net.Anycast
{
    internal class CallIdentity
    {
        public Guid RequestId
        {
            get;
            private set;
        }

        public Future<bool> Future
        {
            get;
            private set;
        }

        public Action<RtpData,bool> Process
        {
            get;
            set;
        }

        public CallIdentity()
        {
            this.RequestId = Guid.NewGuid();
            this.Future = new Future<bool>();
        }

        public void Reset()
        {
            this.RequestId = Guid.NewGuid();
            this.Future.Reset();
            this.Process = null;
        }

    }
}
