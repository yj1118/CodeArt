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
    public class EventMonitorRepository : DataRepository
    {
        public override void Initialize()
        {
            const string sql = "if ISNULL(object_id(N'[dbo].[EventMonitor]'),'') = 0 " +
                "begin " +
                "CREATE TABLE [dbo].[EventMonitor](" +
                "[Id][uniqueidentifier] NOT NULL," +
                "[Interrupted] [bit] NOT NULL," +
                "[CreateTime] [datetime] NOT NULL," +
                "CONSTRAINT [PK_EventMonitor_Id] PRIMARY KEY CLUSTERED " +
                "(" +
                "[Id] ASC" +
                ")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]" +
                ") ON [PRIMARY] " +
                "end";

            DataContext.NewScope(() =>
            {
                DataContext.Current.Connection.Execute(sql);
            });
        }

        public void Add(EventMonitor obj)
        {
            this.Connection.Execute("insert into dbo.EventMonitor(Id,Interrupted,CreateTime) values(@Id,@Interrupted,@CreateTime);", obj);
        }


        public EventMonitor Find(Guid queueId, QueryLevel level)
        {
            return this.Connection.QuerySingle<EventMonitor>("select * from dbo.EventMonitor where id=@id;", new { Id = queueId }, level);
        }

        public void Update(EventMonitor obj)
        {
            this.Connection.Execute("update dbo.EventMonitor set Interrupted=@Interrupted where id=@id;", new { obj.Id, obj.Interrupted });
            obj.ClearDirty();
        }

        public void Delete(EventMonitor obj)
        {
            this.Connection.Execute("delete dbo.EventMonitor where id=@id;", new { obj.Id });
        }

        /// <summary>
        /// 获得已中断的监视器信息,
        /// 每次获取10个，按照时间排正序，越早的排越前面（也就是中断时间越长的排前面）
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Guid> Top10Interrupteds()
        {
            return this.Connection.ExecuteScalars<Guid>("select top 10 Id from dbo.EventMonitor where interrupted=@interrupted order by createTime asc;", new { Interrupted=true }, QueryLevel.None);
        }

        private EventMonitorRepository() { }

        public static readonly EventMonitorRepository Instance = new EventMonitorRepository();
    }
}
