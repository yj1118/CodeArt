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
    /// <summary>
    /// 乐币充值
    /// </summary>
    [Event("PointsRechargeEvent",IsMockRemote = true)]
    public class PointsRechargeEvent : PointsChangeEvent
    {
        public PointsRechargeEvent()
        {

        }

        protected override void ExecuteRaise(User user)
        {
            user.Points += this.Value;
        }

        protected override void ExecuteReverse(User user)
        {
        }
    }
}