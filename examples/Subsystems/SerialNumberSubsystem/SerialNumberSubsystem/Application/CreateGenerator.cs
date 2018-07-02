using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

namespace SerialNumberSubsystem
{
    public class CreateGenerator : Command<SNGenerator>
    {
        private string _name;

        public string MarkedCode
        {
            get;
            set;
        }

        private IEnumerable<DTObject> _rules;

        public CreateGenerator(string name, IEnumerable<DTObject> rules)
        {
            _name = name;
            _rules = rules;
        }

        protected override SNGenerator ExecuteProcedure()
        {
            var g = new SNGenerator()
            {
                Name = _name,
                MarkedCode = this.MarkedCode ?? string.Empty,
                Rules = GetRules()
            };

            var repository = Repository.Create<ISNGeneratorRepository>(); //获取仓储
            repository.Add(g); //向仓储中追加对象

            return g;
        }

        private IEnumerable<RegionRule> GetRules()
        {
            return RegionRuleFactory.GetRules(_rules);
        }


    }
}
