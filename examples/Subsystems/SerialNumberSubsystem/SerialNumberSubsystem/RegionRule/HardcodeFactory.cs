using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using CodeArt.DTO;
          
namespace SerialNumberSubsystem
{
    /// <summary>
    /// 硬编码区域规则的工厂
    /// </summary>
    [SafeAccess]
    internal class HardcodeFactory : IRegionRuleFactory
    {
        public HardcodeFactory() { }

        public RegionRule Create(int ruleId, DTObject dto)
        {
            var content = dto.GetValue<string>("content");
            var hardcode = new Hardcode(ruleId, content);
            return hardcode;
        }
    }
}