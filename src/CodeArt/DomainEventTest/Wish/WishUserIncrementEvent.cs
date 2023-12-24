using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;
using CodeArt.DTO;

namespace DomainEventTest
{
    /// <summary>
    /// 消费一次赠送碎片的机会
    /// </summary>
    [Event("WishUserIncrementEvent")]
    public class WishUserIncrementEvent : DomainEvent
    {

        [EventArg()]
        public long UserId
        {
            get;
            set;
        }

        [EventArg()]
        public int Pieces
        {
            get;
            set;
        }

        [EventArg()]
        public bool Success
        {
            get;
            set;
        }

        public WishUserIncrementEvent()
        {
        }


        protected override void RaiseImplement()
        {
            User.Single(this.UserId,(user)=>
            {
                user.Pieces += this.Pieces;
            });
            this.Success = true;
        }

        protected override void ReverseImplement()
        {
            if(this.Success)
            {

    
            }
        }
    }
}
