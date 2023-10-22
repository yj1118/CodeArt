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
    public static class SqlHelper
    {
        #region 内部方法

        public static int Execute(string sql, object param = null)
        {
            int result = 0;
            DataContext.Using((conn)=>
            {
                result = conn.Execute(sql, param);
            });
            return result;
        }

        public static T ExecuteScalar<T>(string sql, object param = null)
        {
            T result = default(T);
            DataContext.Using((conn) =>
            {
                result = conn.ExecuteScalar<T>(sql, param);
            });
            return result;
        }

        public static object ExecuteScalar(string sql, object param = null)
        {
            object result = null;
            DataContext.Using((conn) =>
            {
                result = conn.ExecuteScalar(sql, param);
            });
            return result;
        }

        public static DynamicData QueryFirstOrDefault(string sql, object param = null)
        {
            DynamicData data = new DynamicData();
            DataContext.Using((conn) =>
            {
                using (var reader = conn.ExecuteReader(sql, param))
                {
                    FillSingleData(reader, data);
                }
            });
            return data;
        }

        public static void QueryFirstOrDefault(string sql, object param, DynamicData data)
        {
            DataContext.Using((conn) =>
            {
                using (var reader = conn.ExecuteReader(sql, param))
                {
                    FillSingleData(reader, data);
                }
            });
        }


        public static void Query(string sql, object param, List<DynamicData> datas)
        {
            DataContext.Using((conn) =>
            {
                using (var reader = conn.ExecuteReader(sql, param))
                {
                    FillMultipleData(reader, datas);
                }
            });
        }

        public static IEnumerable<DynamicData> Query<T>(string sql, object param = null) where T : IAggregateRoot
        {
            var objectType = typeof(T);
            List<DynamicData> datas = new List<DynamicData>();
            Query(sql, param, datas);
            return datas;
        }

        public static string GetConnection()
        {
            var provider = SqlContext.GetConnectionProvider();
            return provider.GetConnectionString("local");
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
