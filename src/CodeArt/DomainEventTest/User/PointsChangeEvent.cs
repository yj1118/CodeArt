using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.DTO;
using CodeArt.EasyMQ.Event;

namespace DomainEventTest
{
    public abstract class PointsChangeEvent : DomainEvent
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        [EventArg()]
        public long UserId
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [EventArg()]
        public decimal Value
        {
            get;
            set;
        }


        /// <summary>
        /// 操作是否成功
        /// </summary>
        [EventArg()]
        public bool Success
        {
            get;
            set;
        }


        public PointsChangeEvent()
        {

        }

        protected override void RaiseImplement()
        {
            System.Threading.Thread.Sleep(1000);//模拟远程调用时间
            User.Single(this.UserId,(user)=>
            {
                ExecuteRaise(user);
            });
            this.Success = true;
        }

        protected abstract void ExecuteRaise(User user);


        protected override void ReverseImplement()
        {
            if (this.Success)
            {
                User.Single(this.UserId, (user) =>
                {
                    ExecuteReverse(user);
                });
                this.Success = false;
            }
        }

        protected abstract void ExecuteReverse(User user);

    }
}
