using System;
using System.Collections.Generic;
using System.Text;

using Dapper;

using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

namespace FileSubsystem
{
    public interface IVirtualDirectoryRepository : IRepository<VirtualDirectory>
    {
        /// <summary>
        /// 查询磁盘的根目录
        /// </summary>
        /// <param name="diskId"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        VirtualDirectory FindRoot(Guid diskId, QueryLevel level);

        IEnumerable<VirtualDirectory> FindChilds(Guid parentId, QueryLevel level);

        VirtualDirectory FindByName(string name, Guid parentId, QueryLevel level);

        /// <summary>
        /// 获取目录的所有父亲（包括父级、祖父级等）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<VirtualDirectory> FindParents(Guid id);

        Page<VirtualDirectory> FindPageByParent(Guid parentId, int pageIndex, int pageSize);

        int GetChildCount(Guid parentId, QueryLevel level);

    }

    [SafeAccess]
    [DataMapper(typeof(VirtualDirectoryMapper))]
    public class SqlVirtualDirectoryRepository : SqlRepository<VirtualDirectory>, IVirtualDirectoryRepository
    {
        private SqlVirtualDirectoryRepository() { }

        public static readonly IVirtualDirectoryRepository Instance = new SqlVirtualDirectoryRepository();

        public VirtualDirectory FindRoot(Guid diskId, QueryLevel level)
        {
            return this.QuerySingle<VirtualDirectory>("disk.id=@diskId and parent.Id=@parentId", (data) =>
            {
                data.Add("diskId", diskId);
                data.Add("parentId", Guid.Empty); //根目录的父目录编号为空
            }, level);
        }

        public IEnumerable<VirtualDirectory> FindChilds(Guid parentId, QueryLevel level)
        {
            return this.Query<VirtualDirectory>("parent.Id=@parentId[order by createTime desc]", (data) =>
            {
                data.Add("parentId", parentId);
            }, level);
        }

        public VirtualDirectory FindByName(string name, Guid parentId, QueryLevel level)
        {
            return this.QuerySingle<VirtualDirectory>("parent.Id=@parentId and name=@name", (data) =>
            {
                data.Add("parentId", parentId);
                data.Add("name", name);
            }, level);
        }

        public IEnumerable<VirtualDirectory> FindParents(Guid id)
        {
            return this.Query<VirtualDirectory>("key findParents", (data) =>
            {
                data.Add("id", id);
            }, QueryLevel.None);
        }

        public Page<VirtualDirectory> FindPageByParent(Guid parentId, int pageIndex, int pageSize)
        {
            return this.Query<VirtualDirectory>("parent.id=@parentId[order by createTime]", pageIndex, pageSize, (data) =>
            {
                data.Add("parentId", parentId);
            });
        }

        public int GetChildCount(Guid parentId, QueryLevel level)
        {
            return this.GetCount<VirtualDirectory>("parent.Id=@parentId", (data) =>
            {
                data.Add("parentId", parentId);
            }, level);
        }
    }

    [SafeAccess]
    internal class VirtualDirectoryMapper : TreeDataMapper
    {
        protected override DomainProperty IdProperty => VirtualDirectory.IdProperty;

        protected override DomainProperty ParentProperty => VirtualDirectory.ParentProperty;

       

    }

}
