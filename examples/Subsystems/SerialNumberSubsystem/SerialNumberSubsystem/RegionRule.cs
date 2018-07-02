using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace SerialNumberSubsystem
{
    /// <summary>
    /// 区域规则，每个流水号生成器由多个区域规则组成
    /// </summary>
    [ObjectRepository(typeof(ISNGeneratorRepository))]
    public abstract class RegionRule : EntityObject<RegionRule, int>, IRegionRule
    {

        public RegionRule(int id)
            : base(id)
        {
            this.OnConstructed();
        }

        public abstract string GetCode();


        #region 空对象

        private class RegionRuleEmpty : RegionRule
        {
            public RegionRuleEmpty()
                : base(0)
            {
                this.OnConstructed();
            }

            public override string GetCode()
            {
                return string.Empty;
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly RegionRule Empty = new RegionRuleEmpty();

        #endregion

    }
}
