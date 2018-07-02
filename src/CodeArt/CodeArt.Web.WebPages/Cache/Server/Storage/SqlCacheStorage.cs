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

        public bool TryGetLastModified(CacheVariable variable, out DateTime lastModified)
        {
            lastModified = DateTime.Now;
            const string sql = "select lastModified from dbo.cache where code=@code";
            var prms = SqlHelper.CreateParameters((c) =>
            {
                c.Add("@code", variable.UniqueCode);
            });

            SqlData data = SqlHelper.ExecuteCommandText("db-cache", sql, prms);
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

            SqlData data = SqlHelper.ExecuteCommandText("db-cache", sql, prms);
            if (data.IsEmpty()) return null;

            byte[] buffer = data.GetScalar<byte[]>();
            return new MemoryStream(buffer);
        }

        private static string _updateContentSql = null;

        static SqlCacheStorage()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("begin transaction;");
            sql.AppendLine("if exists(select code from dbo.cache with(xlock,holdlock) where code=@code)");
            sql.AppendLine("begin");
            sql.AppendLine("	update dbo.cache set lastModified=getdate(),cont=@cont;");
            sql.AppendLine("end");
            sql.AppendLine("else");
            sql.AppendLine("begin");
            sql.AppendLine("	insert into dbo.cache(code,lastModified,cont) values(@code,getdate(),@cont);");
            sql.AppendLine("end");
            sql.AppendLine("commit;");
            _updateContentSql = sql.ToString();
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

            SqlData data = SqlHelper.ExecuteCommandText("db-cache", _updateContentSql, prms);
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

    }
}