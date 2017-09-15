using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    public class DeleteRole : Command
    {
        private Guid _id;

        public DeleteRole(Guid id)
        {
            _id = id;
        }

        protected override void ExecuteProcedure()
        {
            RoleService.DeleteGeneral(_id);
        }
    }
}
