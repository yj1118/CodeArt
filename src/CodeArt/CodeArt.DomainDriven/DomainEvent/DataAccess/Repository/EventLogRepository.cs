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
    public class EventLogRepository : DataRepository
    {
        public override void Initialize()
        {
            const string sql = "if ISNULL(object_id(N'[dbo].[EventLog]'),'') = 0 " +
                "begin " +
                "CREATE TABLE [dbo].[EventLog](" +
                "[Id][uniqueidentifier] NOT NULL," +
                "[Language] [varchar](50) NOT NULL," +
                "[TenantId] [bigint] NOT NULL," +
                "[Status] [tinyint] NOT NULL," +
                "[EntryCount] [int] NOT NULL," +
                "CONSTRAINT [PK_EventLog_Id] PRIMARY KEY CLUSTERED " +
                "(" +
                "[Id] ASC" +
                ")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]" +
                ") ON [PRIMARY] " +
                "end";

            DataContext.NewScope((conn) =>
            {
                conn.Execute(sql);
            });
        }

        public void Add(EventLog obj)
        {
            this.Connection.Execute("insert into dbo.EventLog(Id,Language,TenantId,Status,EntryCount) values(@Id,@Language,@TenantId,@Status,@EntryCount);", obj);
        }

        public EventLog Find(Guid id, QueryLevel level)
        {
            return this.Connection.QuerySingle<EventLog>("select * from dbo.EventLog where id=@id;", new { Id = id }, level);
        }

        public void Update(EventLog obj)
        {
            if (!obj.IsDirty) return;
            this.Connection.Execute("update dbo.EventLog set Status=@Status,EntryCount=@EntryCount where id=@id;", new { obj.Id, obj.Status,obj.EntryCount });
            obj.ClearDirty();
        }

        public void Delete(Guid id)
        {
            this.Connection.Execute("delete dbo.EventLog where Id=@id;", new { id });
        }


        private EventLogRepository() { }

        public static readonly EventLogRepository Instance = new EventLogRepository();
    }
}
