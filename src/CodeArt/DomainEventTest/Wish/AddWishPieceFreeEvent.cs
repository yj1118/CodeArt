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
    /// 模拟免费赠送碎片
    /// </summary>
    [Event("AddWishPieceFreeEvent")]
    public class AddWishPieceFreeEvent : DomainEvent
    {

        [EventArg()]
        public long HelperId
        {
            get;
            set;
        }

        [EventArg()]
        public decimal Count
        {
            get;
            set;
        }

        [EventArg()]
        public long UserId
        {
            get;
            set;
        }


        [EventArg()]
        public decimal Points
        {
            get;
            set;
        }


        public AddWishPieceFreeEvent()
        {
        }

        /// <summary>
        /// 免费赠送
        /// </summary>
        private static string[] _preEvents = new string[] { "ConsumeFreeGiftEvent", "PointsRechargeEvent", "WishUserIncrementEvent" };

        /// <summary>
        /// 获得前置事件
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<string> GetPreEvents()
        {
            return _preEvents;
        }

        protected override void FillArgs(string eventName, DTObject args)
        {
            switch (eventName)
            {
                case "ConsumeFreeGiftEvent":
                    {
                        args["userId"] = this.HelperId;
                    }
                    break;
                case "PointsRechargeEvent":
                    {
                        this.Points = this.Count * 2; //2倍返还

                        args["userId"] = this.HelperId;
                        args["value"] = this.Points;
                    }
                    break;
                case "WishUserIncrementEvent":
                    {
                        args["userId"] = this.HelperId;
                        args["pieces"] = this.Count;
                    }
                    break;
            }
        }

        protected override void EventCompleted(string eventName, DTObject result)
        {
            switch (eventName)
            {
                case "ConsumeFreeGiftEvent":
                    {
                        this.Count = result.Dynamic.Pieces;//可以赠送的碎片
                    }
                    break;
            }
        }

        protected override void RaiseImplement()
        {
            User.Single(this.UserId, (user) =>
            {
                user.Gift += this.Count;
            });
        }

        protected override void ReverseImplement()
        {
            if (this.UserId != 0)
            {

            }
        }



        public override IEnumerable<EventEntry> TestGetEventEntries()
        {
            return new EventEntry[]
            {
                EventEntry.CreateTest("ConsumeFreeGiftEvent"),
                EventEntry.CreateTest("PointsRechargeEvent"),
                EventEntry.CreateTest("WishUserIncrementEvent"),
                EventEntry.CreateTest("AddWishPieceFreeEvent"),
            };
        }


    }
}
