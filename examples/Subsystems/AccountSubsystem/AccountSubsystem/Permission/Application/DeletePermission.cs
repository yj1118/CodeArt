using System;
using System.Linq;

using CodeArt;
using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    public sealed class DeletePermission : Command
    {
        private Guid _id;

        public DeletePermission(Guid id)
        {
            _id = id;
        }

        protected override void ExecuteProcedure()
        {
            var repository = Repository.Create<IPermissionRepository>();
            Permission permission = repository.Find(_id, QueryLevel.Single);
            repository.Delete(permission);
        }
    }
}
