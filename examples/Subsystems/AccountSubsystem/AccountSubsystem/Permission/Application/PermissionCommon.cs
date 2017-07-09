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

        public static Permission FindByMarkedCode(string markedCode, QueryLevel level)
        {
            var repository = Repository.Create<IPermissionRepository>();
            return repository.FindByMarkedCode(markedCode, level);
        }

        /// <summary>
        /// 以无锁的形式根据编号查找多个权限对象
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static IEnumerable<Permission> FindsBy(IEnumerable<Guid> ids)
        {
            if (ids == null || ids.Count() == 0) return Array.Empty<Permission>();
            var repository = Repository.Create<IPermissionRepository>();
            return repository.FindsBy(ids);
        }

        public static Page<Permission> FindPage(string name, int pageIndex, int pageSize)
        {
            var repository = Repository.Create<IPermissionRepository>();
            return repository.FindPageBy(name, pageIndex, pageSize);
        }


    }
}
