using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using CodeArt.DTO;

namespace SerialNumberSubsystem
{
    [SafeAccess]
    internal class IncrementByMonthFactory : IRegionRuleFactory
    {
        public IncrementByMonthFactory() { }

        public RegionRule Create(int ruleId , DTObject dto)
        {
            var width = dto.GetValue<int>("width");
            var incrementByMonth = new IncrementByMonth(ruleId,width);
            return incrementByMonth;
        }
    }
}
