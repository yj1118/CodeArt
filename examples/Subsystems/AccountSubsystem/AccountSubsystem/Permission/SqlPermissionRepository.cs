using System;
using System.Collections.Generic;
using CodeArt.DomainDriven;

using CodeArt.Concurrent;
using CodeArt.DomainDriven.DataAccess;

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
            return DataContext.Current.Query<Permission>("@name<name like %@name%>[order by createTime desc]", pageIndex, pageSize, (arg) =>
            {
                arg.TryAdd("name", name);
            });
        }

        public IEnumerable<Permission> FindsBy(IEnumerable<Guid> ids)
        {
            return DataContext.Current.Query<Permission>("id in @ids", (arg) =>
            {
                arg.Add("ids", ids);
            }, QueryLevel.None);
        }

        public static readonly SqlPermissionRepository Instance = new SqlPermissionRepository();

    }
}
