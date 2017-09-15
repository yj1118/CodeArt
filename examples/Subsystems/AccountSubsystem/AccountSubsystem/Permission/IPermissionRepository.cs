using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using CodeArt.DomainDriven;


namespace AccountSubsystem
{
    public interface IPermissionRepository : IRepository<Permission>
    {
        /// <summary>
        /// 根据唯一标示精确查找权限对象
        /// </summary>
        /// <param name="markedCode"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        Permission FindByMarkedCode(string markedCode, QueryLevel level);

        /// <summary>
        /// 根据名称模糊查找权限翻页集合
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        Page<Permission> FindPageBy(string name, int pageIndex, int pageSize);

        /// <summary>
        /// 根据编号得到多个权限对象
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        IEnumerable<Permission> FindsBy(IEnumerable<Guid> ids);
    }
}