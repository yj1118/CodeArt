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
    public class EventLogEntryRepository : DataRepository
    {
        public override void Initialize()
        {
            const string sql = "if ISNULL(object_id(N'[dbo].[EventLogEntry]'),'') = 0 " +
                "begin " +
                "CREATE TABLE [dbo].[EventLogEntry](" +
                "[Id][uniqueidentifier] NOT NULL," +
                "[LogId] [uniqueidentifier] NOT NULL," +
                "[Operation] [tinyint] NOT NULL," +
                "[ContentCode] [nvarchar](max) NOT NULL," +
                "[OrderIndex] [int] NOT NULL," +
                "[IsReversed] [bit] NOT NULL," +
                "CONSTRAINT [PK_EventLogEntry_Id] PRIMARY KEY CLUSTERED " +
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

        public void Add(EventLogEntry obj)
        {
            this.Connection.Execute("insert into dbo.EventLogEntry(Id,LogId,Operation,ContentCode,OrderIndex,IsReversed) " +
                                        "values(@Id,@LogId,@Operation,@ContentCode,@OrderIndex,@IsReversed);", obj);
        }

        //public EventLog Find(Guid id, QueryLevel level)
        //{
        //    return this.Helper.QuerySingle<EventLog>("select * from dbo.EventLog where id=@id;", new { Id = id }, level);
        //}

        public void Update(EventLogEntry obj)
        {
            if (!obj.IsDirty) return;
            this.Connection.Execute("update dbo.EventLogEntry set IsReversed=@IsReversed where id=@id;", new { obj.Id, obj.IsReversed });
            obj.ClearDirty();
        }

        /// <summary>
        /// 删除所有条目
        /// </summary>
        /// <param name="logId"></param>
        public void Deletes(Guid logId)
        {
            this.Connection.Execute("delete dbo.EventLogEntry where LogId=@logId;", new { logId });
        }


        public IEnumerable<EventLogEntry> FindByReverseOrder(Guid logId)
        {
            return this.Connection.Query<EventLogEntry>("select * from dbo.EventLogEntry where LogId=@logId order by OrderIndex desc", new { logId }, QueryLevel.None);
        }


        private EventLogEntryRepository() { }

        public static readonly EventLogEntryRepository Instance = new EventLogEntryRepository();
    }
}
