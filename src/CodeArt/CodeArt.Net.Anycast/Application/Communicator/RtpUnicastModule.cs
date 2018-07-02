using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace CodeArt.Net.Anycast
{
    public abstract class RtpUnicastModule : RtpModule
    {
        public string UnicastAddress
        {
            get
            {
                return this.Communicator.Context.UnicastChannel.HostAddress;
            }
        }

        public RtpUnicastModule(Participant participant)
            :base(participant)
        {
        }

        protected override RtpChannel CreateChannel(RtpCommunicator communicator)
        {
            return communicator.Context.UnicastChannel;
        }

        protected override void RemoveChannel(RtpCommunicator communicator)
        {
           
        }

    }
}
