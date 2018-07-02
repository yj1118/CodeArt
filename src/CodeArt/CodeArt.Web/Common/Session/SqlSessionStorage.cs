using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Configuration;
using System.Text;

using CodeArt.Data.MSSql;

namespace CodeArt.Web
{
    public sealed class SqlSessionStorage : ISessionStorage
    {
        private SqlSessionStorage() { }

        public static readonly ISessionStorage Instance = new SqlSessionStorage(); 

        private const string _dbName = "db-session";

        public object Load(string sessionId, string itemId)
        {
            var prms = SqlHelper.CreateParameters((c) =>
            {
                c.Add("@sessionId", sessionId);
                c.Add("@itemId", itemId);
            });

            var data = SqlHelper.ExecuteCommandText(_dbName, "select value from [dbo].[session] where sessionId=@sessionId and itemId=@itemId", prms);

            if (data.IsEmpty()) return null;
            return GetObjectBy(data.GetScalar<Byte[]>());
        }

        public void Save(string sessionId, string itemId, object value)
        {
            var prms = SqlHelper.CreateParameters((c) =>
            {
                c.Add("@sessionId", sessionId);
                c.Add("@itemId", itemId);
                c.Add("@value", GetObjectBytesBy(value));
                c.Add("@accessTime", DateTime.Now);
            });

            SqlHelper.ExecuteCommandText(_dbName, _saveSessionSql, prms);
        }

        public void DeleteItem(string sessionId, string itemId)
        {
            var prms = SqlHelper.CreateParameters((c) =>
            {
                c.Add("@sessionId", sessionId);
                c.Add("@itemId", itemId);
            });

            SqlHelper.ExecuteCommandText(_dbName, "delete from dbo.[session] with(rowlock) where sessionId=@sessionId and itemId=@itemId", prms);
        }

        public void DeleteItems(string sessionId)
        {
            var prms = SqlHelper.CreateParameters((c) =>
            {
                c.Add("@sessionId", sessionId);
            });

            SqlHelper.ExecuteCommandText(_dbName, "delete from dbo.[session] with(rowlock) where sessionId=@sessionId", prms);
        }

        /// <summary>
        /// 移除已超过minutes分钟未访问的会话数据
        /// minutes小于或等于0代表删除所有数据
        /// </summary>
        /// <param name="minutes"></param>
        public void Clear(int minutes)
        {
            var prms = SqlHelper.CreateParameters((c) =>
            {
                c.Add("@minutes", minutes);
                c.Add("@accessTime", DateTime.Now);
            });

            SqlHelper.ExecuteCommandText(_dbName, _clearSessionSql, prms);
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj">需要序列化的对象</param>
        /// <returns></returns>
        private static byte[] GetObjectBytesBy(object obj)
        {
            byte[] bytes = null;
            using (MemoryStream buffer = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(buffer, obj);
                buffer.Position = 0;
                bytes = buffer.ToArray();
            }
            return bytes;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="bytes">需要反序列化的对象</param>
        /// <returns></returns>
        private static object GetObjectBy(byte[] bytes)
        {
            object obj = null;
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                obj = formatter.Deserialize(stream);
            }
            return obj;
        }


        private static readonly string _createSessionTableSql = GetCreateSessionTableSql();

        private static string GetCreateSessionTableSql()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("if ISNULL(object_id(N'dbo.session'),'') = 0");
            sql.AppendLine("begin");
            sql.AppendLine("CREATE TABLE [dbo].[session](");
            sql.AppendLine("	[sessionId] [varchar](50) NOT NULL,");
            sql.AppendLine("	[itemId] [varchar](50) NOT NULL,");
            sql.AppendLine("	[value] [varbinary](max) NOT NULL,");
            sql.AppendLine("	[accessTime] [datetime] NOT NULL,");
            sql.AppendLine(" CONSTRAINT [PK_session] PRIMARY KEY CLUSTERED");
            sql.AppendLine("(");
            sql.AppendLine("	[sessionId] ASC,");
            sql.AppendLine("	[itemId] ASC");
            sql.AppendLine(")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
            sql.AppendLine(") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]");
            sql.Append("end");
            return sql.ToString();
        }

        private static readonly string _saveSessionSql = GetSaveSessionSql();

        private static string GetSaveSessionSql()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("if(exists(select sessionId from [dbo].[session] where sessionId=@sessionId and itemId=@itemId))");
            sql.AppendLine(" begin");
            sql.AppendLine("   update [dbo].[session] set accessTime=@accessTime,value=@value where sessionId=@sessionId and itemId=@itemId;");
            sql.AppendLine(" end ");
            sql.AppendLine("else");
            sql.AppendLine(" begin");
            sql.AppendLine("   insert into [dbo].[session](sessionId,itemId,value,accessTime) values(@sessionId,@itemId,@value,@accessTime);");
            sql.Append(" end");
            return sql.ToString();
        }

        private static readonly string _clearSessionSql = GetClearSessionSql();

        private static string GetClearSessionSql()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("if(@minutes <= 0)");
            sql.AppendLine(" begin");
            sql.AppendLine("   delete from dbo.[session];");
            sql.AppendLine(" end");
            sql.AppendLine("else");
            sql.AppendLine(" begin");
            sql.AppendLine("     delete from dbo.[session] where datediff(minute,accessTime,@accessTime)>@minutes");
            sql.Append(" end");
            return sql.ToString();
        }

        static SqlSessionStorage()
        {
            SqlHelper.ExecuteCommandText(_dbName, _createSessionTableSql);
        }


    }
}
