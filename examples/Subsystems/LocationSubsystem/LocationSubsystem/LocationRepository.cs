using System;
using System.Collections.Generic;
using System.Text;

using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

namespace LocationSubsystem
{
    public interface ILocationRepository : IRepository<Location>
    {
        Location FindBy(string markedCode, QueryLevel level);

        /// <summary>
        /// 获取所有父亲集合（包括父级、祖父级等）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<Location> FindParents(long id);

        /// <summary>
        /// 获取子类集合(仅获取下一级的子类)
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        IEnumerable<Location> FindChilds(long parentId, QueryLevel level);

    }

    [SafeAccess]
    [DataMapper(typeof(LocationMapper))]
    public class SqlLocationRepository : SqlRepository<Location>, ILocationRepository
    {
        private SqlLocationRepository() { }

        public static readonly ILocationRepository Instance = new SqlLocationRepository();

        

        public IEnumerable<Location> FindChilds(long parentId, QueryLevel level)
        {
            return this.Query<Location>("parent.Id=@parentId[order by sortNumber desc]", (data) =>
            {
                data.Add("parentId", parentId);
            }, level);
        }

        public Location FindBy(string markedCode, QueryLevel level)
        {
            return this.QuerySingle<Location>("markedCode=@markedCode", (data) =>
            {
                data.Add("markedCode", markedCode);
            }, level);
        }

        public IEnumerable<Location> FindParents(long id)
        {
            return this.Query<Location>("key findParents", (data) =>
            {
                data.Add("id", id);
            }, QueryLevel.None);
        }

    }

    [SafeAccess]
    internal class LocationMapper : TreeDataMapper
    {
        protected override DomainProperty IdProperty => Location.IdProperty;

        protected override DomainProperty ParentProperty => Location.ParentProperty;
    }

}
