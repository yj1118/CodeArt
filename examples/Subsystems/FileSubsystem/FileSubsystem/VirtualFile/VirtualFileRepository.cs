using System;
using System.Collections.Generic;

using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

namespace FileSubsystem
{
    public interface IVirtualFileRepository : IRepository<VirtualFile>
    {
        VirtualFile FindBy(string storeKey, QueryLevel level);

        VirtualFile FindByName(string name, Guid directoryId, QueryLevel level);

        Page<VirtualFile> FindsByDirectory(Guid directoryId, int pageIndex, int pageSize);
    }

    [SafeAccess]
    public class SqlVirtualFileRepository : SqlRepository<VirtualFile>, IVirtualFileRepository
    {
        private SqlVirtualFileRepository() { }

        public static readonly SqlVirtualFileRepository Instance = new SqlVirtualFileRepository();

        public VirtualFile FindBy(string storeKey, QueryLevel level)
        {
            return this.QuerySingle<VirtualFile>("storeKey=@storeKey", (data) =>
            {
                data.Add("storeKey", storeKey);
            }, level);
        }

        public VirtualFile FindByName(string name, Guid directoryId, QueryLevel level)
        {
            return this.QuerySingle<VirtualFile>("directory.id=@directoryId and name=@name", (data) =>
            {
                data.Add("directoryId", directoryId);
                data.Add("name", name);
            }, level);
        }

        public Page<VirtualFile> FindsByDirectory(Guid directoryId, int pageIndex, int pageSize)
        {
            return this.Query<VirtualFile>("directory.id=@directoryId[order by createTime]", pageIndex, pageSize, (data) =>
              {
                  data.Add("directoryId", directoryId);
              });
        }
    }
}
