using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.DTO;

namespace DomainEventTest
{
    public class AddWishPieceFree : EventCommand<AddWishPieceFreeEvent, (decimal Pieces,decimal Points)>
    {
        private long _helperId;
        private long _wishUserId;

        public AddWishPieceFree(long helperId, long wishUserId)
        {
            _helperId = helperId;
            _wishUserId = wishUserId;
        }

        protected override AddWishPieceFreeEvent CreateEvent()
        {
            var @event = new AddWishPieceFreeEvent()
            {
                HelperId = _helperId,
                UserId = _wishUserId,
            };

            return @event;
        }

        protected override (decimal Pieces, decimal Points) GetResult(AddWishPieceFreeEvent @event)
        {
            if (@event.Points == 0)
            {
                Util.Error("获得的Points为0");
            }

            return (@event.Count, @event.Points);
        }
    }

    /// <summary>
    /// 赠送方式
    /// </summary>
    public enum GiftMethod : byte
    {
        /// <summary>
        /// 免费赠送
        /// </summary>
        Free = 1,
        /// <summary>
        /// 账户余额赠送
        /// </summary>
        Balance = 2
    }


}
