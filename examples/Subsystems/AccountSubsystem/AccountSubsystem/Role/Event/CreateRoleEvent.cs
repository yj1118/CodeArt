using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.DTO;

namespace AccountSubsystem
{
    [Event("CreateRole")]
    public class CreateRoleEvent : DomainEvent
    {
        [EventArg()]
        public string Name
        {
            get;
            set;
        }

        [EventArg()]
        public Guid[] PermissionIds
        {
            get;
            set;
        }


        [EventArg()]
        public bool IsSystem
        {
            get;
            set;
        }

        [EventArg()]
        public Guid RoleId
        {
            get;
            private set;
        }

        public CreateRoleEvent()
        {

        }

        public override void Raise()
        {
            var cmd = new CreateRole(this.Name, this.PermissionIds, this.IsSystem);
            var role = cmd.Execute();
            this.RoleId = role.Id;
        }

        public override void Reverse()
        {
            if (this.RoleId != Guid.Empty)
            {
                var cmd = new DeleteRolePro(this.RoleId);
                cmd.Execute();
            }
        }
    }
}
