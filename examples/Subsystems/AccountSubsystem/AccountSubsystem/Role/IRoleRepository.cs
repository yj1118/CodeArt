using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    public interface IRoleRepository : IRepository<Role>
    {
        Role FindByMarkedCode(string markedCode, QueryLevel level);

        Page<Role> FindPage(Guid organizationId, string name, int pageSize, int pageIndex);

        IEnumerable<Role> FindRoles(IEnumerable<Guid> ids);
    }
}
