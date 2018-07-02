using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt;
using CodeArt.DTO;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;


namespace SerialNumberSubsystem
{
    public class GetNumber : Command<string>
    {
        public string MarkedCode { get; private set; }

        public GetNumber(string markedCode)
        {
            this.MarkedCode = markedCode;
        }

        protected override string ExecuteProcedure()
        {
            var repository = Repository.Create<ISNGeneratorRepository>();
            var generator = repository.FindByMarkedCode(this.MarkedCode, QueryLevel.Single);
            var number = generator.Generate();
            repository.Update(generator);
            return number;
        }
    }
}
