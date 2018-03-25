using CodeArt.DTO;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Net.Anycast
{
    /// <summary>
    /// 组播对象
    /// </summary>
    public class Multicast
    {
        public AnycastClient Client
        {
            get;
            private set;
        }

        /// <summary>
        /// 组播地址
        /// </summary>
        public string Address
        {
            get;
            private set;
        }

        /// <summary>
        /// 客户端在该组播中的身份
        /// </summary>
        public Participant Host
        {
            get;
            private set;
        }

        public Multicast(AnycastClient session, string address, Participant host)
        {
            this.Client = session;
            this.Address = address;
            this.Host = host;
        }

        /// <summary>
        /// 加入组播
        /// </summary>
        internal void Join()
        {
            var msg = CreateJoinMessage(this.Address);
            this.Client.Send(msg);
            AddOrUpdateParticipant(this.Host);
        }

        private Message CreateJoinMessage(string multicastAddress)
        {
            DTObject header = DTObject.Create();
            header.SetValue(MessageField.MessageType, (byte)MessageType.Join);
            header.SetValue(MessageField.MulticastAddress, multicastAddress);
            return new Message(header, this.Host.Data);
        }

        internal void UpdateHost(Participant host)
        {
            var msg = CreateUpdateParticipant(this.Address);
            this.Client.Send(msg);
            AddOrUpdateParticipant(host);
        }

        private Message CreateUpdateParticipant(string multicastAddress)
        {
            DTObject header = DTObject.Create();
            header.SetValue(MessageField.MessageType, (byte)MessageType.ParticipantUpdated);
            header.SetValue(MessageField.MulticastAddress, multicastAddress);
            header.SetValue(MessageField.Destination, multicastAddress);
            return new Message(header, this.Host.Data);
        }


        internal void Leave()
        {
            var msg = CreateLeaveMessage(this.Address);
            this.Client.Send(msg);
            RemoveParticipant(this.Host);
        }

        private Message CreateLeaveMessage(string multicastAddress)
        {
            DTObject header = DTObject.Create();
            header.SetValue(MessageField.MessageType, (byte)MessageType.Leave);
            header.SetValue(MessageField.MulticastAddress, multicastAddress);
            return new Message(header, this.Host.Data);
        }

        /// <summary>
        /// 移除过期的成员
        /// </summary>
        /// <param name="interval"></param>
        //internal void RemoveOverdues(TimeSpan interval)
        //{
        //    var current = DateTime.Now;
        //    lock (_participants)
        //    {
        //        var overdues = _participants.Values.Where((participant) =>
        //        {
        //            return current > (participant.LastActiveTime + interval);
        //        }).ToArray();

        //        foreach (var item in overdues)
        //        {
        //            if (item == this.Client.Participant) continue; //如果是本地参与人，不移除
        //            RemoveParticipant(item);
        //        }
        //    }
        //}


        #region 参与者

        private Dictionary<string, Participant> _participants = new Dictionary<string, Participant>();

        private Dictionary<string, Participant> _participantsForOrigin = new Dictionary<string, Participant>();


        public Participant[] Participants
        {
            get
            {
                Participant[] participants;

                lock (_participants)
                {
                    participants = _participants.Values.ToArray();
                }
                return participants;
            }
        }

        public Participant GetParticipant(string participantId)
        {
            lock (_participants)
            {
                Participant participant = null;
                if (_participants.TryGetValue(participantId, out participant))
                    return participant;
            }
            return null;
        }

        private bool IsLocal(Participant participant)
        {
            return participant == this.Host;
        }

        internal void AddOrUpdateParticipant(Participant participant)
        {
            lock (_participants)
            {
                if (!_participants.ContainsKey(participant.Id))
                {
                    _participants.Add(participant.Id, participant);
                    ClientEvents.AsyncRaiseParticipantAdded(this.Client, this, participant, IsLocal(participant));
                }
                else
                {
                    _participants[participant.Id] = participant;
                    ClientEvents.AsyncRaiseParticipantUpdated(this.Client, this, participant, IsLocal(participant));
                }

                if(!string.IsNullOrEmpty(participant.Orgin))
                {
                    if (!_participantsForOrigin.ContainsKey(participant.Orgin))
                    {
                        _participantsForOrigin.Add(participant.Orgin, participant);
                    }
                    else
                    {
                        _participantsForOrigin[participant.Orgin] = participant;
                    }
                }


                //exist.LastActiveTime = DateTime.Now; //更新最后一次活跃时间
            }
        }


        internal void RemoveParticipant(Participant participant)
        {
            lock (_participants)
            {
                if (_participants.Remove(participant.Id))
                {
                    ClientEvents.AsyncRaiseParticipantRemoved(this.Client, this, participant, IsLocal(participant));
                }

                if(!string.IsNullOrEmpty(participant.Orgin))
                {
                    _participantsForOrigin.Remove(participant.Orgin);
                }
            }
        }

        internal void RemoveParticipant(string orgin)
        {
            if (_participantsForOrigin.TryGetValue(orgin, out var participant))
            {
                RemoveParticipant(participant);
            }
        }

        public override bool Equals(object obj)
        {
            var target = obj as Multicast;
            if (target == null) return false;
            return this.Address == target.Address && this.Client == target.Client;
        }

        public override int GetHashCode()
        {
            return this.Address.GetHashCode() ^ this.Client.GetHashCode();
        }

        #endregion

        ///// <summary>
        ///// 创建该组播的发送器，用于向组播发送数据
        ///// </summary>
        ///// <param name="extensions"></param>
        ///// <returns></returns>
        //internal MulticastRtpSender CreateSender(NameValueCollection header)
        //{
        //    return CreateSender(header, string.Empty);
        //}

        ///// <summary>
        ///// 创建该组播的发送器，用于向组播发送数据
        ///// </summary>
        ///// <param name="extensions"></param>
        ///// <returns></returns>
        //internal MulticastRtpSender CreateSender(NameValueCollection header, string eventSource)
        //{
        //    return this.Client.CreateMulticastRtpSender(this, header, eventSource);
        //}

        ///// <summary>
        ///// 创建该组播的发送器，用于向组播发送数据
        ///// </summary>
        ///// <returns></returns>
        //internal MulticastRtpSender CreateSender()
        //{
        //    return CreateSender(new NameValueCollection(), string.Empty);
        //}

    }
}
