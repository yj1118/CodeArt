using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    public class DeleteRolePro : Command
    {
        private Guid _id;

        public DeleteRolePro(Guid id)
        {
            _id = id;
        }

        protected override void ExecuteProcedure()
        {
            RoleService.DeleteSystem(_id);
        }
    }
}
