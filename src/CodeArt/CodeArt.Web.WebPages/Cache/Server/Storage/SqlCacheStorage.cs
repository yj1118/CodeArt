using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.IO;

using CodeArt.Data;
using CodeArt.Data.MSSql;
using CodeArt.IO;

namespace CodeArt.Web.WebPages
{
    public sealed class SqlCacheStorage : ICacheStorage
    {
        private SqlCacheStorage() { }

        public static ICacheStorage Instance = new SqlCacheStorage();

        private const string _dbName = "db-cache";

        public bool TryGetLastModified(CacheVariable variable, out DateTime lastModified)
        {
            lastModified = DateTime.Now;
            const string sql = "select lastModified from dbo.cache where code=@code";
            var prms = SqlHelper.CreateParameters((c) =>
            {
                c.Add("@code", variable.UniqueCode);
            });

            SqlData data = SqlHelper.ExecuteCommandText(_dbName, sql, prms);
            if (data.IsEmpty()) return false;

            lastModified = data.GetScalar<DateTime>();
            return true;
        }

        public Stream Read(CacheVariable variable)
        {
            const string sql = "select cont from dbo.cache where code=@code";
            var prms = SqlHelper.CreateParameters((c) =>
            {
                c.Add("@code", variable.UniqueCode);
            });

            SqlData data = SqlHelper.ExecuteCommandText(_dbName, sql, prms);
            if (data.IsEmpty()) return null;

            byte[] buffer = data.GetScalar<byte[]>();
            return new MemoryStream(buffer);
        }

        private static string _updateContentSql = GetUpdateContentSql();

        private static string GetUpdateContentSql()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("begin transaction;");
            sql.AppendLine("if exists(select code from dbo.cache with(xlock,holdlock) where code=@code)");
            sql.AppendLine("begin");
            sql.AppendLine("	update dbo.cache set lastModified=getdate(),cont=@cont where code=@code;");
            sql.AppendLine("end");
            sql.AppendLine("else");
            sql.AppendLine("begin");
            sql.AppendLine("	insert into dbo.cache(code,lastModified,cont) values(@code,getdate(),@cont);");
            sql.AppendLine("end");
            sql.AppendLine("commit;");
            return sql.ToString();
        }

        private static readonly string _createCacheTableSql = GetCreateCacheTableSql();

        private static string GetCreateCacheTableSql()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("if ISNULL(object_id(N'dbo.cache'),'') = 0");
            sql.AppendLine("begin");
            sql.AppendLine("CREATE TABLE [dbo].[cache](");
            sql.AppendLine("	[code] [varchar](100) NOT NULL,");
            sql.AppendLine("	[lastModified] [datetime] NOT NULL,");
            sql.AppendLine("	[cont][varbinary](max) NOT NULL,");
            sql.AppendLine(" CONSTRAINT [PK_cache] PRIMARY KEY CLUSTERED");
            sql.AppendLine("(");
            sql.AppendLine("	[code] ASC");
            sql.AppendLine(")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
            sql.AppendLine(") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]");
            sql.Append("end");
            return sql.ToString();
        }

        static SqlCacheStorage()
        {
            if (!SqlHelper.ExistConnection(_dbName)) return;
            SqlHelper.ExecuteCommandText(_dbName, _createCacheTableSql);
        }


        /// <summary>
        /// 更新缓存器内容
        /// </summary>
        /// <param name="url"></param>
        public void Update(CacheVariable variable, Stream content)
        {
            if (content.Length > int.MaxValue)
                throw new WebCacheException("SqlCacheStorage无法存储超过" + int.MaxValue + "个字节的缓存内容！");

            byte[] buffer = new byte[content.Length];
            content.ReadPro(buffer, 0, (int)content.Length);

            var prms = SqlHelper.CreateParameters((c) =>
            {
                c.Add("@code", variable.UniqueCode);
                c.Add("@cont", buffer);
            });

            SqlData data = SqlHelper.ExecuteCommandText(_dbName, _updateContentSql, prms);
        }

        public void Delete(CacheVariable variable)
        {
            const string sql = "delete from dbo.cache where code=@code";
            var prms = SqlHelper.CreateParameters((c) =>
            {
                c.Add("@code", variable.UniqueCode);
            });
            SqlHelper.ExecuteCommandText("db-cache", sql, prms);
        }

        public void DeleteAll()
        {
            const string sql = "delete from dbo.cache";
            SqlHelper.ExecuteCommandText("db-cache", sql);
        }

    }
}