using System;
using System.Collections.Generic;

using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

namespace FileSubsystem
{
    public interface IVirtualDiskRepository : IRepository<VirtualDisk>
    {
        VirtualDisk FindBy(string markedCode, QueryLevel level);

        Page<VirtualDisk> FindsBy(string markedCode, int pageIndex, int pageSize);

    }

    [SafeAccess]
    public class SqlVirtualDiskRepository : SqlRepository<VirtualDisk>, IVirtualDiskRepository
    {
        private SqlVirtualDiskRepository() { }

        public VirtualDisk FindBy(string markedCode, QueryLevel level)
        {
            return this.QuerySingle<VirtualDisk>("markedCode=@markedCode", (data) =>
            {
                data.Add("markedCode", markedCode);
            }, level);
        }

        public Page<VirtualDisk> FindsBy(string markedCode, int pageIndex, int pageSize)
        {
            return this.Query<VirtualDisk>("markedCode like %@markedCode%[order by createTime]", pageIndex, pageSize, (data) =>
              {
                  data.Add("markedCode", markedCode);
              });
        }

        public static readonly SqlVirtualDiskRepository Instance = new SqlVirtualDiskRepository();
    }
}
