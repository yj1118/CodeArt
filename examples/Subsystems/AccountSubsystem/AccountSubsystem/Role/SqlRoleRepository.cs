using System;
using System.Collections.Generic;
using CodeArt.DomainDriven;

using CodeArt.Concurrent;
using CodeArt.DomainDriven.DataAccess;

namespace AccountSubsystem
{
    public interface IRoleRepository : IRepository<Role>
    {
        Role FindByMarkedCode(string markedCode, QueryLevel level);

        Page<Role> FindPage(Guid organizationId, string name, int pageSize, int pageIndex);

        IEnumerable<Role> FindRoles(IEnumerable<Guid> ids);
    }

    [SafeAccess]
    public class SqlRoleRepository : SqlRepository<Role>, IRoleRepository
    {
        private SqlRoleRepository() { }

        public Role FindByMarkedCode(string markedCode, QueryLevel level)
        {
            return this.QuerySingle<Role>("markedCode=@markedCode", (arg) =>
             {
                 arg.Add("markedCode", markedCode);
             }, level);
        }

        public Page<Role> FindPage(Guid organizationId, string name, int pageIndex, int pageSize)
        {
            return this.Query<Role>("@organizationId<organization.id=@organizationId> and @name<name like %@name%>[order by createTime desc]",
                                                    pageIndex, pageSize,
                                                    (arg) =>
                                                    {
                                                        arg.TryAdd("organizationId", organizationId);
                                                        arg.TryAdd("name", name);
                                                    });
        }

        public IEnumerable<Role> FindRoles(IEnumerable<Guid> ids)
        {
            return DataContext.Current.Query<Role>("id in @ids", (arg) =>
            {
                arg.Add("ids", ids);
            }, QueryLevel.None);
        }

        public static readonly SqlRoleRepository Instance = new SqlRoleRepository();

    }
}
