using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    public static class PermissionCommon
    {
        public static Permission FindById(Guid id, QueryLevel level)
        {
            var repository = Repository.Create<IPermissionRepository>();
            return repository.Find(id, level);
        }


        public static Permission FindByName(string name, QueryLevel level)
        {
            var repository = Repository.Create<IPermissionRepository>();
            return repository.FindByName(name, level);
        }

        public static Permission FindByMarkedCode(string markedCode, QueryLevel level)
        {
            var repository = Repository.Create<IPermissionRepository>();
            return repository.FindByMarkedCode(markedCode, level);
        }

        public static IEnumerable<Permission> FindsBy(IEnumerable<Guid> ids, QueryLevel level)
        {
            if (ids == null || ids.Count() == 0) return Array.Empty<Permission>();
            var repository = Repository.Create<IPermissionRepository>();
            return repository.FindsBy(ids, level);
        }

        public static Page<Permission> FindPage(string name, int pageIndex, int pageSize)
        {
            var repository = Repository.Create<IPermissionRepository>();
            return repository.FindPageBy(name, pageIndex, pageSize);
        }


    }
}
