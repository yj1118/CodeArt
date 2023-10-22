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
    [Event("ConsumeFreeGiftEvent")]
    public class ConsumeFreeGiftEvent : DomainEvent
    {
        public static int PiecesValue = 10;


        [EventArg()]
        public long UserId
        {
            get;
            set;
        }

        [EventArg()]
        public decimal Pieces
        {
            get;
            set;
        }

        public ConsumeFreeGiftEvent()
        {
        }


        protected override void RaiseImplement()
        {
            this.Pieces = PiecesValue;
        }

        protected override void ReverseImplement()
        {
            if(this.Pieces > 0)
            {
                
            }
        }
    }
}
