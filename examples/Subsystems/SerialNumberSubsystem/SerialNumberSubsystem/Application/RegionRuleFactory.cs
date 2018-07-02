using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.DTO;
using CodeArt.Util;

namespace SerialNumberSubsystem
{
    internal static class RegionRuleFactory
    {
        public static IEnumerable<RegionRule> GetRules(IEnumerable<DTObject> dtos)
        {
            if (dtos == null) return Array.Empty<RegionRule>();

            List<RegionRule> rules = new List<RegionRule>();
            int index = 0;
            foreach (var dto in dtos)
            {
                index++;
                var rule = Create(index, dto);
                rules.Add(rule);
            }
            return rules;
        }

        private static RegionRule Create(int id, DTObject dto)
        {
            var ruleType = dto.GetValue<string>("ruleType");  //这句话的意思是，我们约定每一个JSON定义里，一定会定义了规则的类型
            var factory = _getFactory(ruleType);
            return factory.Create(id, dto);
        }

        private static Func<string, IRegionRuleFactory> _getFactory = LazyIndexer.Init<string, IRegionRuleFactory>((ruleType)=>
        {
            var factoryType = Type.GetType(string.Format("SerialNumberSubsystem.{0}Factory,SerialNumberSubsystem", ruleType), true, true);
            var factory = SafeAccessAttribute.CreateInstance<IRegionRuleFactory>(factoryType);
            return factory;
        });

    }
}
