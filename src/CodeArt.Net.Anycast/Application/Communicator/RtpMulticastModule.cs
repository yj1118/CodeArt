using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace CodeArt.Net.Anycast
{
    public abstract class RtpMulticastModule : RtpModule
    {
        public string MulticastAddress
        {
            get;
            private set;
        }

        public RtpMulticastModule(string multicastAddress, Participant participant)
            :base(participant)
        {
            this.MulticastAddress = multicastAddress;
        }

        protected override RtpChannel CreateChannel(RtpCommunicator communicator)
        {
            return communicator.Context.CreateChannel(this.MulticastAddress, this.Participant);
        }

        protected override void RemoveChannel(RtpCommunicator communicator)
        {
            communicator.Context.RemoveChannel(this.MulticastAddress);
        }

        #region 成员

        public Participant[] GetParticipants()
        {
            return this.Communicator.Context.GetParticipants(this.MulticastAddress);
        }

        protected override Participant UpdateParticipantImpl(Action<Participant> action)
        {
            return this.Communicator.Context.UpdateParticipant(this.MulticastAddress, action);
        }

        #endregion

    }
}
