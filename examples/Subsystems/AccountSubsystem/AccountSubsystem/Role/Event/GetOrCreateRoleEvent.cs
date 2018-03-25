using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.DTO;

namespace AccountSubsystem
{
    [Event("GetOrCreateRole")]
    public class GetOrCreateRoleEvent : DomainEvent
    {
        [EventArg()]
        public string Name
        {
            get;
            set;
        }

        [EventArg()]
        public string MarkedCode
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
        public Guid Id
        {
            get;
            private set;
        }

        public GetOrCreateRoleEvent()
        {

        }

        protected override void RaiseImplement()
        {
            var repository = Repository.Create<IRoleRepository>();
            var role = repository.FindByMarkedCode(this.MarkedCode, QueryLevel.None);
            if (!role.IsEmpty())
            {
                FillArgs(role);
                return;
            }

            role = repository.FindByMarkedCode(this.MarkedCode, QueryLevel.HoldSingle);
            if (!role.IsEmpty())
            {
                FillArgs(role);
                return;
            }

            var cmd = new CreateRole(this.Name, this.PermissionIds, this.IsSystem)
            {
                MarkedCode = this.MarkedCode ?? string.Empty
            };
            role = cmd.Execute();
            FillArgs(role);
        }

        private void FillArgs(Role role)
        {
            this.Id = role.Id;
            this.IsSystem = role.IsSystem;
            this.MarkedCode = role.MarkedCode;
            this.Name = role.Name;
            this.PermissionIds = role.Permissions.Select((p) => p.Id).ToArray();
        }

        protected override void ReverseImplement()
        {
            //角色无论是否创建成功，都不需要再回逆的时候删除，因为是GetOrCreate 
        }
    }
}
