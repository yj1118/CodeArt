using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using CodeArt.DTO;

namespace SerialNumberSubsystem
{
    internal interface IRegionRuleFactory
    {
        RegionRule Create(int ruleId, DTObject dto);
    }
}