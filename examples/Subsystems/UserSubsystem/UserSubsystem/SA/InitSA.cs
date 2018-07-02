using System;
using System.Linq;
using System.Collections.Generic;

using CodeArt;
using CodeArt.DomainDriven;
using AccountSubsystem;
using FileSubsystem;

namespace UserSubsystem
{
    public sealed class InitSA : CreateUser
    {
        public InitSA(string accountName, string password)
            : base(accountName, password, 100 * VirtualDisk.Size1G)
        {
            this.NickName = Strings.SystemAdministrator;
        }

        protected override User ExecuteProcedure()
        {
            var role = GetOrCreateSARole();
            if (HasBeenInitialized(role)) throw new RepeatedInitSAException();
            this.RoleIds = new Guid[] { role.Id };
            return base.ExecuteProcedure();
        }

        private Role GetOrCreateSARole()
        {
            var role = RoleCommon.FindByMarkedCode(SARole.MarkedCode, QueryLevel.HoldSingle);
            if (role.IsEmpty())
            {
                var cmd = new CreateRole(SARole.Name, Array.Empty<Guid>(), true)
                {
                    MarkedCode = SARole.MarkedCode
                };
                return cmd.Execute();
            }
            return role;
        }


        private bool HasBeenInitialized(Role role)
        {
            var sas = AccountCommon.FindsByRole(role.Id, QueryLevel.HoldSingle);
            return sas.Count() > 0;
        }
    }
}
