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
    internal class DateCodeFactory : IRegionRuleFactory
    {
        public DateCodeFactory() { }

        public RegionRule Create(int ruleId , DTObject dto)
        {
            var dateCode = new DateCode(ruleId);
            return dateCode;
        }
    }
}
