using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    public static class OrganizationCommon
    {
        public static Organization FindBy(Guid orgId, QueryLevel level)
        {
            var repository = Repository.Create<IOrganizationRepository>();
            return repository.Find(orgId, level);
        }

    }
}
