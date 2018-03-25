using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    public class DeleteAccount : Command
    {
        private Guid _id;

        public DeleteAccount(Guid id)
        {
            _id = id;
        }

        protected override void ExecuteProcedure()
        {
            var repository = Repository.Create<IAccountRepository>();
            var account = repository.Find(_id, QueryLevel.Mirroring);
            repository.Delete(account);
        }


    }
}
