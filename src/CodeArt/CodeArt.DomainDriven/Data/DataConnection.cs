
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Dapper;
using CodeArt.Util;
using CodeArt.Concurrent;
using CodeArt.Data.MSSql;
using CodeArt.AppSetting;

namespace CodeArt.DomainDriven
{
    public class DataConnection : IDisposable
    {
        public SqlConnection _conn;
        public SqlTransaction _tran;

        internal DataConnection()
        {

        }



        #region 初始化

        internal void Initialize()
        {
            _conn = CreateConnection();
        }

        internal void Begin()
        {
            if (_tran != null) throw new DomainDrivenException("事务已开启");
            _tran = OpenTransaction(_conn);
        }

        //internal void Init(bool openTransaction)
        //{
        //    _conn = CreateConnection();

        //    if(openTransaction)
        //        _tran = OpenTransaction(_conn);
        //}

        public static string GetConnectionString()
        {
            return SqlHelper.GetConnectionString("local"); //本地连接
        }

        private static SqlConnection CreateConnection()
        {
            var connectionString = GetConnectionString();
            var conn = new SqlConnection(connectionString);
            conn.Open();
            return conn;
        }

        private static SqlTransaction OpenTransaction(SqlConnection conn)
        {
            return conn.BeginTransaction(IsolationLevel.ReadUncommitted);
        }

        #endregion


        internal void Rollback()
        {
            if(_tran != null)
                _tran.Rollback();
        }

        internal void Commit()
        {
            if (_tran != null)
                _tran.Commit();
        }

        #region 释放资源

        public void Dispose()
        {
            if (_tran != null)
            {
                _tran.Dispose();
                _tran = null;
            }
            if (_conn != null)
            {
                _conn.Close();
                _conn.Dispose();
                _conn = null;
            }
        }

        #endregion


        public object ExecuteScalar(string sql, object param = null, QueryLevel level = null)
        {
            sql = GetLevelSql(sql, level);
            return _conn.ExecuteScalar(sql, param, _tran);
        }

        public T ExecuteScalar<T>(string sql, object param = null, QueryLevel level = null)
        {
            sql = GetLevelSql(sql, level);
            return _conn.ExecuteScalar<T>(sql, param, _tran);
        }

        public int Execute(string sql, object param = null, QueryLevel level = null)
        {
            sql = GetLevelSql(sql, level);
            return _conn.Execute(sql, param, _tran);
        }

        public IEnumerable<T> ExecuteScalars<T>(string sql, object param = null, QueryLevel level = null)
        {
            sql = GetLevelSql(sql, level);
            return _conn.Query<T>(sql, param, _tran);
        }

        public IDataReader ExecuteReader(string sql, object param = null, QueryLevel level = null)
        {
            sql = GetLevelSql(sql, level);
            return _conn.ExecuteReader(sql, param, _tran);
        }


        public T QuerySingle<T>(string sql, object param = null, QueryLevel level = null) where T : IDataObject, new()
        {
            sql = GetLevelSql(sql, level);
            var obj = new T();
            using (var reader = _conn.ExecuteReader(sql, param, _tran))
            {
                obj.Load(reader);
            }
            return obj;
        }

        public dynamic QuerySingle(string sql, object param = null, QueryLevel level = null)
        {
            sql = GetLevelSql(sql, level);
            return _conn.QuerySingle(sql, param, _tran);
        }

        public IEnumerable<T> Query<T>(string sql, object param = null, QueryLevel level = null) where T : IDataObject, new()
        {
            sql = GetLevelSql(sql, level);
            List<T> objs = new List<T>();
            using (var reader = _conn.ExecuteReader(sql, param, _tran))
            {
                while (true)
                {
                    var obj = new T();
                    obj.Load(reader);
                    if (obj.IsEmpty()) break;
                    objs.Add(obj);
                }
            }
            return objs;
        }

        public IEnumerable<dynamic> Query(string sql, object param = null, QueryLevel level = null)
        {
            sql = GetLevelSql(sql, level);
            return _conn.Query(sql, param, _tran);
        }

        public dynamic QueryFirstOrDefault(string sql, object param = null, QueryLevel level = null)
        {
            sql = GetLevelSql(sql, level);
            return _conn.QueryFirstOrDefault(sql, param, _tran);
        }


        private static RegexPool _regexPool = new RegexPool(".+from[ ](.+?)(where|inner|left)", RegexOptions.IgnoreCase);

        private static Func<string, Func<QueryLevel, string>> _getLevelSql = LazyIndexer.Init<string, Func<QueryLevel, string>>((sql) =>
         {
             return LazyIndexer.Init<QueryLevel, string>((level) =>
             {
                 using (var temp = _regexPool.Borrow())
                 {
                     var reg = temp.Item;
                     var math = reg.Match(sql);
                     if (!math.Success) throw new DomainEventException("解析level错误");
                     var tableName = math.Groups[1].Value;
                     var index = math.Groups[1].Index;
                     return sql.Insert(index + tableName.Length, level.GetMSSqlLockCode());
                 }
             });
         });

        private static string GetLevelSql(string sql, QueryLevel level)
        {
            if (level == null) return sql;
            return _getLevelSql(sql)(level);
        }
    }

}
