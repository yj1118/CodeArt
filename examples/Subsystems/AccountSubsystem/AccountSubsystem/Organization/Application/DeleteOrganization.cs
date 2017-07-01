using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    public class DeleteOrganization : Command
    {
        private Guid _id;

        public DeleteOrganization(Guid id)
        {
            _id = id;
        }

        protected override void ExecuteProcedure()
        {
            var repository = Repository.Create<IOrganizationRepository>();
            var org = repository.Find(_id, QueryLevel.Single);
            repository.Delete(org);
        }
    }
}
