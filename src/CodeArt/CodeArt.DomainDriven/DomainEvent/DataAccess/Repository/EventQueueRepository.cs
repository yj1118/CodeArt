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
    public class EventQueueRepository : DataRepository
    {
        public override void Initialize()
        {
            const string sql0 = "if ISNULL(object_id(N'[dbo].[EventQueue]'),'') = 0 " +
                "begin " +
                "CREATE TABLE [dbo].[EventQueue](" +
                "[Id][uniqueidentifier] NOT NULL," +
                "[Language] [varchar](50) NOT NULL," +
                "[TenantId] [bigint] NOT NULL," +
                "[IsSubqueue] [bit] NOT NULL," +
                "[CreateTime] [datetime] NOT NULL," +
                "CONSTRAINT [PK_EventQueue_Id] PRIMARY KEY CLUSTERED " +
                "(" +
                "[Id] ASC" +
                ")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]" +
                ") ON [PRIMARY] " +
                "end";

            const string sql1 = "if ISNULL(object_id(N'[dbo].[EventEntry]'),'') = 0 " +
                "begin " +
                "CREATE TABLE [dbo].[EventEntry](" +
                "[RootId] [uniqueidentifier] NOT NULL,"+
                "[Id] [uniqueidentifier] NOT NULL," +
                "[SourceId] [uniqueidentifier] NOT NULL," +
                "[EventName] [varchar](100) NOT NULL," +
                "[Status] [tinyint] NOT NULL," +
                "[ArgsCode] [nvarchar](max) NOT NULL," +
                "[OrderIndex] [int] NOT NULL," +
                "CONSTRAINT [PK_EventEntry_RootId_Id] PRIMARY KEY CLUSTERED " +
                "(" +
                "[RootId] ASC," +
                "[Id] ASC" +
                ")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]" +
                ") ON [PRIMARY] " +
                "end";

            DataContext.NewScope((conn) =>
            {
                conn.Execute(sql0);
                conn.Execute(sql1);
            });
        }

        public void Add(EventQueue obj)
        {
            this.Connection.Execute("insert into dbo.EventQueue(Id,Language,TenantId,IsSubqueue,CreateTime) values(@Id,@Language,@TenantId,@IsSubqueue,@CreateTime);",
                                new { obj.Id, obj.Language, obj.TenantId, obj.IsSubqueue, obj.CreateTime });

            foreach(var entry in obj.Entries)
            {
                this.AddEntry(obj,entry);
            }
        }

        private void AddEntry(EventQueue root, EventEntry entry)
        {
            this.Connection.Execute("insert into dbo.EventEntry(RootId,Id,SourceId,EventName,Status,ArgsCode,OrderIndex) " +
                                    "values(@RootId,@Id,@SourceId,@EventName,@Status,@ArgsCode,@OrderIndex);",
                                new { RootId = root.Id, entry.Id, entry.SourceId, entry.EventName, entry.Status, entry.ArgsCode, entry.OrderIndex });
        }

        public void Update(EventQueue obj)
        {
            //EventQueue本身是没有属性可以被修改的，只用检查条目项
            var helper = this.Connection;
            foreach(var entry in obj.Entries)
            {
                if(entry.IsDirty)
                {
                    helper.Execute("update dbo.EventEntry set Status=@Status,ArgsCode=@ArgsCode where id=@id;", new { entry.Id, entry.Status, entry.ArgsCode });
                    entry.ClearDirty();
                }
            }
        }

        public void Delete(Guid queueId)
        {
            var helper = this.Connection;
            helper.Execute("delete dbo.EventQueue where Id=@id;", new { Id = queueId });
            helper.Execute("delete dbo.EventEntry where RootId=@id;", new { Id = queueId });
        }


        public EventQueue Find(Guid queueId, QueryLevel level)
        {
            var queue = this.Connection.QuerySingle<EventQueue>("select * from dbo.EventQueue where id=@id;", new { Id = queueId }, level);
            var entries = this.Connection.Query<EventEntry>("select * from dbo.EventEntry where RootId=@id order by OrderIndex asc;", new { Id = queueId });
            //为每个条目加载事件源
            foreach(var entry in entries)
            {
                var sourceId = entry.SourceId;
                if (sourceId == Guid.Empty)
                    entry.Source = EventEntry.Empty; //这意味着是源事件，源事件没有源事件
                else
                {
                    var source = entries.FirstOrDefault((e) => e.Id == sourceId);
                    if (source.IsEmpty()) throw new DomainEventException(string.Format("事件{0}-{1}没有找到源{2}", entry.EventName, entry.EventId, entry.SourceId));
                    entry.Source = source;
                }
                entry.ClearDirty();
            }
            queue.Entries = entries;
            return queue;
        }

        public EventQueue FindByEventId(Guid eventId, QueryLevel level)
        {
            var helper = this.Connection;
            var queueId = helper.ExecuteScalar<Guid>("select RootId from dbo.EventEntry where Id=@eventId;", new { eventId }, level);
            return Find(queueId, level);
        }

        private EventQueueRepository() { }

        public static readonly EventQueueRepository Instance = new EventQueueRepository();
    }
}
