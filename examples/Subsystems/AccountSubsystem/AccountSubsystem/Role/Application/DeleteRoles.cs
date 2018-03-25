using System;
using System.Collections.Generic;
using System.Linq;

using CodeArt;
using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    public sealed class DeleteRoles : Command
    {
        private IEnumerable<Guid> _ids;

        public DeleteRoles(IEnumerable<Guid> ids)
        {
            _ids = ids;
        }

        protected override void ExecuteProcedure()
        {
            var repository = Repository.Create<IRoleRepository>();
            var roles = repository.FindRoles(_ids);
            foreach(var role in roles)
            {
                repository.Delete(role);
            }
        }
    }
}
