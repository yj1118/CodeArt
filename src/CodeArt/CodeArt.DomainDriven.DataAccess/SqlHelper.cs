using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Reflection;


using Dapper;
using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using CodeArt.AppSetting;

namespace CodeArt.DomainDriven.DataAccess
{
    internal static class SqlHelper
    {
        #region 内部方法

        public static int Execute(string connName, string sql, object param = null)
        {
            var connectionString = GetConnectionString(connName);
            using (IDbConnection conn = new SqlConnection(connectionString))
            {
                return conn.Execute(sql, param);
            }
        }

        public static T ExecuteScalar<T>(string connName, string sql, object param = null)
        {
            var connectionString = GetConnectionString(connName);
            using (IDbConnection conn = new SqlConnection(connectionString))
            {
                return conn.ExecuteScalar<T>(sql, param);
            }
        }

        public static object ExecuteScalar(string connName, string sql, object param = null)
        {
            var connectionString = GetConnectionString(connName);
            using (IDbConnection conn = new SqlConnection(connectionString))
            {
                return conn.ExecuteScalar(sql, param);
            }
        }

        public static DynamicData QueryFirstOrDefault(string connName, string sql, object param = null)
        {
            var connectionString = GetConnectionString(connName);
            using (IDbConnection conn = new SqlConnection(connectionString))
            {
                using (var reader = conn.ExecuteReader(sql, param))
                {
                    var data = new DynamicData();
                    FillSingleData(reader, data);
                    return data;
                }   
            }
        }

        public static void QueryFirstOrDefault(string connName, string sql, object param, DynamicData data)
        {
            var connectionString = GetConnectionString(connName);
            using (IDbConnection conn = new SqlConnection(connectionString))
            {
                using (var reader = conn.ExecuteReader(sql, param))
                {
                    FillSingleData(reader, data);
                }
            }
        }


        public static void Query(string connName, string sql, object param, List<DynamicData> datas)
        {
            var connectionString = GetConnectionString(connName);
            using (IDbConnection conn = new SqlConnection(connectionString))
            {
                using (var reader = conn.ExecuteReader(sql, param))
                {
                    FillMultipleData(reader, datas);
                }
            }
        }

        public static string GetConnectionString(string connName)
        {
            var provider = SqlContext.GetConnectionProvider();
            return provider.GetConnectionString(connName);
        }

        #endregion

        #region 辅助

        private static void LoadRowData(IDataReader reader, DynamicData data, int fieldCount)
        {
            for (var i = 0; i < fieldCount; i++)
            {
                if (reader.IsDBNull(i))
                {
                    //过滤空数据，只要是空数据，不予加入到结果集中，这对由于引用了外部根，
                    //外部内聚根被删除了导致的情况很有帮助，而且过滤空数据也符合领域驱动empty的原则，
                    //因此数据直接过滤
                    continue;
                }

                var name = reader.GetName(i);
                var value = reader.GetValue(i);
                data.Add(name, value);
            }
        }


        private static void FillSingleData(IDataReader reader, DynamicData data)
        {
            int count = reader.FieldCount;
            if (reader.Read())
            {
                LoadRowData(reader, data, count);
            }
        }

        private static void FillMultipleData(IDataReader reader, List<DynamicData> datas)
        {
            int count = reader.FieldCount;
            while (reader.Read())
            {
                var data = new DynamicData();
                LoadRowData(reader, data, count);
                datas.Add(data);
            }
        }


        #endregion

        #region 池

        private static Pool<DynamicData> _dataPool = new Pool<DynamicData>(() =>
        {
            return new DynamicData();
        }, (data, phase) =>
        {
            data.Clear();
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 300 //闲置时间300秒
        });

        internal static IPoolItem<DynamicData> BorrowData()
        {
            return _dataPool.Borrow();
        }

        private static Pool<List<DynamicData>> _datasPool = new Pool<List<DynamicData>>(() =>
        {
            return new List<DynamicData>();
        }, (datas, phase) =>
        {
            datas.Clear();
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 300 //闲置时间300秒
        });

        internal static IPoolItem<List<DynamicData>> BorrowDatas()
        {
            return _datasPool.Borrow();
        }

        #endregion

    }
}
