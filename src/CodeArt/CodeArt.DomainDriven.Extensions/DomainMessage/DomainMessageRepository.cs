using System;
using System.Collections.Generic;
using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;
using Dapper;

namespace CodeArt.DomainDriven
{
    public partial interface IDomainMessageRepository : IRepository<DomainMessage>
    {
        IEnumerable<Guid> Top10Idles();

        void DeleteExpired(int days);
    }

    [SafeAccess]
    public class SqlDomainMessageRepository : SqlRepository<DomainMessage>, IDomainMessageRepository
    {

        private SqlDomainMessageRepository() { }

        public static readonly IDomainMessageRepository Instance = new SqlDomainMessageRepository();

        public IEnumerable<Guid> Top10Idles()
        {
            IEnumerable<Guid> result = null;
            DataPortal.Direct<DomainMessage>((conn) =>
            {
                result = conn.ExecuteScalars<Guid>("select top 10 Id from dbo.DomainMessage where Status=@Status order by createTime asc;", new { Status = DomainMessageStatus.Idle }, QueryLevel.None);
            });
            return result;
        }

        public void DeleteExpired(int days)
        {
            var time = DateTime.Now.AddDays(-days);
            DataPortal.Direct<DomainMessage>((conn) =>
            {
                conn.Execute("delete dbo.DomainMessage where Status=@Status and createTime <=@time;", new { Status = DomainMessageStatus.Sent,Time= time });
            });
        }

    }
}
