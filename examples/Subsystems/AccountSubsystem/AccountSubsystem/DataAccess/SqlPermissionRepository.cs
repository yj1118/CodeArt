using System;
using System.Collections.Generic;
using CodeArt.DomainDriven;

using CodeArt.Concurrent;
using CodeArt.DomainDriven.DataAccess;

namespace AccountSubsystem
{
    [SafeAccess]
    public class SqlPermissionRepository : SqlRepository<Permission>, IPermissionRepository
    {
        public Permission FindByName(string name, QueryLevel level)
        {
            return DataContext.Current.QuerySingle<Permission>("name=@name", (arg) =>
             {
                 arg.Add("name", name);
             }, level);
        }

        public Permission FindByMarkedCode(string markedCode, QueryLevel level)
        {
            return DataContext.Current.QuerySingle<Permission>("markedCode=@markedCode", (arg) =>
            {
                arg.Add("markedCode", markedCode);
            }, level);
        }

        public Page<Permission> FindPageBy(string name, int pageIndex, int pageSize)
        {
            return DataContext.Current.Query<Permission>("name like %@name%", pageIndex, pageSize, (arg) =>
            {
                arg.Add("name", name);
            });
        }

        public IEnumerable<Permission> FindsBy(IEnumerable<Guid> ids, QueryLevel level)
        {
            return DataContext.Current.Query<Permission>("id in @ids", (arg) =>
            {
                arg.Add("ids", ids);
            }, level);
        }

        public static readonly SqlPermissionRepository Instance = new SqlPermissionRepository();

    }
}
