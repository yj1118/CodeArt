using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace SerialNumberSubsystem
{
    public static class SNGeneratorCommon
    {
        public static SNGenerator FindByMarkedCode(string markedCode, QueryLevel level)
        {
            var repository = Repository.Create<ISNGeneratorRepository>();
            return repository.FindByMarkedCode(markedCode, level);
        }

    }
}
