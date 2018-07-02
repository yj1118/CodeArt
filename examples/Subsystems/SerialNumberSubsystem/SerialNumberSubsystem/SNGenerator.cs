using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace SerialNumberSubsystem
{
    [ObjectRepository(typeof(ISNGeneratorRepository))]
    [ObjectValidator(typeof(SNGeneratorSpecification))]
    public class SNGenerator : AggregateRoot<SNGenerator, Guid>
    {
        private static readonly DomainProperty NameProperty = DomainProperty.Register<string, SNGenerator>("Name");

        /// <summary>
        /// 生成器的名称
        /// </summary>
        [PropertyRepository()]
        [NotEmpty]
        [StringLength(1, 50)]
        public string Name
        {
            get
            {
                return GetValue<string>(NameProperty);
            }
            set
            {
                SetValue(NameProperty, value);
            }
        }


        internal static readonly DomainProperty MarkedCodeProperty = DomainProperty.Register<string, SNGenerator>("MarkedCode");

        /// <summary>
        /// 生成器的唯一标识符
        /// </summary>
        [PropertyRepository()]
        [StringLength(0, 50)]
        public string MarkedCode
        {
            get
            {
                return GetValue<string>(MarkedCodeProperty);
            }
            set
            {
                SetValue(MarkedCodeProperty, value);
            }
        }


        #region 区域规则

        /// <summary>
        /// 每个流水号生成器由多个区域规则组成
        /// </summary>
        [PropertyRepository]
        [List(Max = 8)]
        private static readonly DomainProperty RulesProperty = DomainProperty.RegisterCollection<RegionRule, SNGenerator>("Rules");

        /// <summary>
        /// 每个流水号生成器由多个区域规则组成
        /// </summary>
        public IEnumerable<RegionRule> Rules
        {
            get
            {
                return RulesImpl;
            }
            set
            {
                RulesImpl = new DomainCollection<RegionRule>(RulesProperty, value);
            }
        }

        private DomainCollection<RegionRule> RulesImpl
        {
            get
            {
                return GetValue<DomainCollection<RegionRule>>(RulesProperty);
            }
            set
            {
                SetValue(RulesProperty, value);
            }
        }

        #endregion

        public SNGenerator()
            : base(Guid.NewGuid())
        {
            this.OnConstructed();
        }

        [ConstructorRepository]
        internal SNGenerator(Guid id)
            : base(id)
        {
            this.OnConstructed();
        }


        /// <summary>
        /// 生成流水号
        /// </summary>
        /// <returns></returns>
        public string Generate()
        {
            StringBuilder code = new StringBuilder();
            foreach (var rule in this.Rules)
            {
                var item = rule.GetCode();
                code.Append(item);
            }
            return code.ToString();
        }

        #region 空对象

        private class SNGeneratorEmpty : SNGenerator
        {
            public SNGeneratorEmpty()
                : base(Guid.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly SNGenerator Empty = new SNGeneratorEmpty();

        #endregion

    }
}
