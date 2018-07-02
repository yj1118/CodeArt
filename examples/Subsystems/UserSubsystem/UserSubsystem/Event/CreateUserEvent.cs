using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.DTO;
using CodeArt.EasyMQ.Event;
using FileSubsystem;

namespace UserSubsystem
{
    [Event("CreateUser")]
    public class CreateUserEvent : DomainEvent
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
        public Sex Sex
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


        [EventArg()]
        public Guid[] RoleIds
        {
            get;
            set;
        }

        public CreateUserEvent()
        {

        }

        protected override void RaiseImplement()
        {
            var cmd = new CreateUser(this.AccountName, this.Password, VirtualDisk.Size1G)
            {
                Name = this.Name,
                MobileNumber = this.MobileNumber,
                Sex = this.Sex,
                RoleIds = this.RoleIds
            };
            var user = cmd.Execute();
            this.Id = user.Id;
        }

        protected override void ReverseImplement()
        {
            if (this.Id == Guid.Empty) return;
            var cmd = new DeleteUser(this.Id);
            cmd.Execute();
        }
    }
}
