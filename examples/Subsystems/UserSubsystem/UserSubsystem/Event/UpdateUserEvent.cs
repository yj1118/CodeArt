using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.DTO;
using CodeArt.EasyMQ.Event;
using AccountSubsystem;

namespace UserSubsystem
{
    [Event("UpdateUser")]
    public class UpdateUserEvent : DomainEvent
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        [EventArg()]
        public Guid Id
        {
            get;
            set;
        }

        /// <summary>
        /// 用户名
        /// </summary>
        [EventArg()]
        public string Name
        {
            get;
            set;
        }

        [EventArg()]
        public byte? Sex
        {
            get;
            set;
        }

        [EventArg()]
        public string MobileNumber
        {
            get;
            set;
        }


        [EventArg()]
        public string AccountName
        {
            get;
            set;
        }

        [EventArg()]
        public string Password
        {
            get;
            set;
        }


        public UpdateUserEvent()
        {

        }

        protected override void RaiseImplement()
        {
            {
                var cmd = new UpdateUser(this.Id)
                {
                    Name = this.Name,
                };
                if(this.Sex != null)
                {
                    cmd.Sex = (Sex)this.Sex.Value;
                }
                cmd.Execute();
            }

            {
                var cmd = new UpdateAccount(this.Id)
                {
                    Name = this.AccountName,
                    MobileNumber = this.MobileNumber,
                    Password = this.Password
                };
                cmd.Execute();
            }
        }

        protected override void ReverseImplement()
        {
            //修改没什么可回逆的，就算修改不成功，这里生效或者不生效也无所谓
        }
    }
}
