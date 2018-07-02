using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeArt.DomainDriven;

namespace LocationSubsystem
{
    public class DeleteLocation : Command
    {
        private long _id;

        public DeleteLocation(long id)
        {
            _id = id;
        }

        protected override void ExecuteProcedure()
        {
            var repository = Repository.Create<ILocationRepository>();
            var location = repository.Find(_id, QueryLevel.Mirroring);
            repository.Delete(location);
        }

    }
}
