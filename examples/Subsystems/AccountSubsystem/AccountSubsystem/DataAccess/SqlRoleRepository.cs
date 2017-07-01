using System;
using System.Collections.Generic;
using CodeArt.DomainDriven;

using CodeArt.Concurrent;
using CodeArt.DomainDriven.DataAccess;

namespace AccountSubsystem
{
    [SafeAccess]
    public class SqlRoleRepository : SqlRepository<Role>, IRoleRepository
    {
        public Role FindByMarkedCode(string markedCode, QueryLevel level)
        {
            return this.QuerySingle<Role>("markedCode=@markedCode", (arg) =>
             {
                 arg.Add("markedCode", markedCode);
             }, level);
        }

        public Page<Role> FindPage(Guid organizationId, string name, int pageIndex, int pageSize)
        {
            return this.Query<Role>("organization.id=@organizationId and name like %@name%",
                                                    pageIndex, pageSize,
                                                    (arg) =>
                                                    {
                                                        arg.Add("organizationId", organizationId);
                                                        arg.Add("name", name);
                                                    });
        }

        public IEnumerable<Role> FindRoles(IEnumerable<Guid> ids, QueryLevel level)
        {
            return DataContext.Current.Query<Role>("id in @ids", (arg) =>
            {
                arg.Add("ids", ids);
            }, level);
        }

        public static readonly SqlRoleRepository Instance = new SqlRoleRepository();

    }
}
