using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Event;

namespace CodeArt.Net.Anycast
{
    public sealed class ParticipantChangedEventArgs : ValidEventArgs
    {
        public RtpCommunicator Communicator
        {
            get;
            private set;
        }

        public Multicast Multicast
        {
            get;
            private set;
        }

        public Participant Participant
        {
            get;
            private set;
        }

        /// <summary>
        /// 是否加入
        /// </summary>
        public ParticipantChangedType Type
        {
            get;
            private set;
        }


        public ParticipantChangedEventArgs(RtpCommunicator communicator, Multicast multicast, Participant participant, ParticipantChangedType type)
        {
            this.Communicator = communicator;
            this.Multicast = multicast;
            this.Participant = participant;
            this.Type = type;
        }

        //对于成员离开或更改，是不具有历史性意义的，不需要为新挂载的事件执行
        //但是成员的加入需要
        public override bool IsHistoric => this.Type != ParticipantChangedType.Removed;


        public override void Apply(IList<ValidEventArgs> args, ValidEventArgs newArg)
        {
            var @new = newArg as ParticipantChangedEventArgs;
            if (@new == null) return;
            var same = this.Communicator == @new.Communicator
                    && this.Multicast == @new.Multicast
                    && this.Participant.Equals(@new.Participant);
            if (!same) return; //不是同一个成员的参数，返回

            if (@new.Type == ParticipantChangedType.Removed)
            {
                args.Remove(this); //当removed时，把之前的操作全部移除
            }
            else if(@new.Type == ParticipantChangedType.Updated)
            {
                if(this.Type == ParticipantChangedType.Added)
                {
                    this.Participant = @new.Participant;  //当之前有新增操作时，同步更新participant
                }
                else if (this.Type == ParticipantChangedType.Updated)
                {
                    args.Remove(this); //仅保留最新的修改操作的事件
                }
            }
        }


    }
}
