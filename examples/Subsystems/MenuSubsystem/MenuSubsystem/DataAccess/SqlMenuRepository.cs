using System;
using System.Collections.Generic;
using CodeArt.DomainDriven;

using CodeArt.Concurrent;
using CodeArt.DomainDriven.DataAccess;

namespace MenuSubsystem
{
    [SafeAccess]
    public class SqlMenuRepository : SqlRepository<Menu>, IMenuRepository
    {
        public Menu FindBy(string markedCode, QueryLevel level)
        {
            return DataContext.Current.QuerySingle<Menu>("markedCode=@markedCode", (arg) =>
            {
                arg.Add("markedCode", markedCode);
            }, level);
        }


        /// <summary>
        /// 获取菜单的所有父亲（包括父级、祖父级等）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<Menu> FindParents(Guid id)
        {
            return DataContext.Current.Query<Menu>("[key MenuFindParents]", (arg) =>
            {
                arg.Add("id", id);
            }, QueryLevel.None);
        }


        /// <summary>
        /// 获取直系子菜单
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public IEnumerable<Menu> FindChilds(Guid parentId)
        {
            return DataContext.Current.Query<Menu>("parent.id=@parentId", (arg) =>
            {
                arg.Add("parentId", parentId);
            }, QueryLevel.None);
        }

        public static readonly SqlMenuRepository Instance = new SqlMenuRepository();

    }
}
