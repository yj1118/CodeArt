using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    public static class RoleCommon
    {
        public static Role FindBy(Guid roleId, QueryLevel level)
        {
            var repository = Repository.Create<IRoleRepository>();
            return repository.Find(roleId, level);
        }


        public static Role FindByMarkedCode(string markedCode, QueryLevel level)
        {
            var repository = Repository.Create<IRoleRepository>();
            return repository.FindByMarkedCode(markedCode, level);
        }

        public static IEnumerable<Role> FindsBy(IEnumerable<Guid> ids, QueryLevel level)
        {
            var repository = Repository.Create<IRoleRepository>();
            return repository.FindRoles(ids, level);
        }

        public static Page<Role> FindPage(Guid organizationId, string name, int pageIndex, int pageSize)
        {
            var repository = Repository.Create<IRoleRepository>();
            return repository.FindPage(organizationId, name, pageIndex, pageSize);
        }
    }
}