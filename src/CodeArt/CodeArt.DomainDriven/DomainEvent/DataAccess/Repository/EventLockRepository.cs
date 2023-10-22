using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using Dapper;

namespace CodeArt.DomainDriven
{
    public class EventLockRepository : DataRepository
    {
        public override void Initialize()
        {
            const string sql = "if ISNULL(object_id(N'[dbo].[EventLock]'),'') = 0 " +
                "begin " +
                "CREATE TABLE [dbo].[EventLock](" +
                "[Id][uniqueidentifier] NOT NULL," +
                "[CreateTime] [datetime] NOT NULL," +
                "CONSTRAINT [PK_EventLock_Id] PRIMARY KEY CLUSTERED " +
                "(" +
                "[Id] ASC" +
                ")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]" +
                ") ON[PRIMARY] " +
                "end";
            DataContext.NewScope((conn) =>
            {
                conn.Execute(sql);
            });
        }

        public void Add(EventLock obj)
        {
            this.Connection.Execute("insert into dbo.EventLock(Id,CreateTime) values(@Id,@CreateTime);", obj);
        }


        public EventLock Find(Guid queueId, QueryLevel level)
        {
            return this.Connection.QuerySingle<EventLock>("select * from dbo.EventLock where Id=@id;", new { Id = queueId }, level);
        }

        public void Delete(Guid id)
        {
            this.Connection.Execute("delete dbo.EventLock where Id=@id", new { id });
        }

        /// <summary>
        /// 找到过期的
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EventLock> FindExpireds(int expiredHours)
        {
            return this.Connection.Query<EventLock>("select * from dbo.EventLock where datediff(hh,createTime,@currentTime)>@hours;",
                                            new { CurrentTime = DateTime.Now, hours = expiredHours }, QueryLevel.None);
        }


        private EventLockRepository() { }

        public static readonly EventLockRepository Instance = new EventLockRepository();

        ///// <summary>
        ///// 找到过期的
        ///// </summary>
        ///// <returns></returns>
        //public IEnumerable<EventLock> FindExpireds(int expiredHours)
        //{
        //    return DataContext.Current.Query<EventLock>("datediff(hh,createTime,@currentTime)>@hours",(arg)=>
        //    {
        //        arg.Add("currentTime",DateTime.Now);
        //        arg.Add("hours", expiredHours);
        //    }, QueryLevel.None);
        //}
    }
}
