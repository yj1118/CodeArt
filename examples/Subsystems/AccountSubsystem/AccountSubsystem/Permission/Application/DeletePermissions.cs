using System;
using System.Collections.Generic;
using System.Linq;

using CodeArt;
using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    public sealed class DeletePermissions : Command
    {
        private IEnumerable<Guid> _ids;

        public DeletePermissions(IEnumerable<Guid> ids)
        {
            _ids = ids;
        }

        protected override void ExecuteProcedure()
        {
            var repository = Repository.Create<IPermissionRepository>();
            var permissions = repository.FindsBy(_ids);
            foreach(var permission in permissions)
            {
                repository.Delete(permission);
            }
        }
    }
}
