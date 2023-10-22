using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Reflection;
using System.Transactions;
using System.ComponentModel;
using IsolationLevel = System.Transactions.IsolationLevel;

using Dapper;
using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.Concurrent;
using CodeArt.DomainDriven;


namespace CodeArt.DomainDriven.DataAccess
{
    public static class DataPortal
    {
        #region 对外公开的方法

        public static long GetIdentity(string tableName)
        {
            long id = 0;
            DataContext.UseTransactionScope(() =>
            {
                string sql = null;
                switch (SqlContext.GetDbType())
                {
                    case DatabaseType.SQLServer: sql = SQLServer.SqlStatement.GetIncrementIdentitySql(tableName);break;
                    default: throw new DataAccessException("不支持的数据库类型");
                }
                id = SqlHelper.ExecuteScalar<long>(sql);
            });
            return id;
           
        }

        /// <summary>
        /// 根据对象类型，获取一个自增的编号，该编号由数据层维护递增
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public static long GetIdentity<T>() where T : class, IAggregateRoot
        {
            var objectType = typeof(T);
            var model = DataModel.Create(objectType);

            long id = 0;
            DataContext.UseTransactionScope(() =>
            {
                id = model.GetIdentity();
            });
            return id;
        }

        /// <summary>
        /// 获得流水号,流水号保证对每个租户都是连续的;
        /// 请注意，流水号不能用于领域对象的编号，因为编号必须保证全局唯一
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static long GetSerialNumber<T>() where T : class, IAggregateRoot
        {
            var objectType = typeof(T);
            var model = DataModel.Create(objectType);

            long id = 0;
            DataContext.UseTransactionScope(() =>
            {
                id = model.GetSerialNumber();
            });
            return id;
        }

        /// <summary>
        /// 可以为非根对象建立唯一标示
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static long GetIdentity<A,T>() where A : class, IAggregateRoot
                                              where T : DomainObject
        {
            var objectType = typeof(A);
            var model = DataModel.Create(objectType);

            var table = DataTable.GetTable<T>();

            long id = 0;
            DataContext.UseTransactionScope(() =>
            {
                id = table.GetIdentity();
            });
            return id;
        }

        /// <summary>
        /// 创建运行时记录的表信息，这在关系数据库中体现为创建表
        /// 该方法一般用于单元测试
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void RuntimeBuild()
        {
            DataModel.RuntimeBuild();
        }

        /// <summary>
        /// 初始化<typeparamref name="T"/>对应的数据模型，这会初始化表结构
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Init<T>()
        {
            DataModel.Create(typeof(T));
        }


        #region 销毁数据

        /// <summary>
        /// 在数据层中销毁数据模型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Dispose()
        {
            DataModel.Drop();
        }

        #endregion

        /// <summary>
        /// 清理数据，但是不销毁数据模型
        /// </summary>
        public static void ClearUp()
        {
            DataModel.ClearUp();
            foreach(var evt in _onClearUpEvents)
            {
                evt();
            }
        }

        private static List<Action> _onClearUpEvents = new List<Action>();

        public static void OnClearUp(Action action)
        {
            _onClearUpEvents.Add(action);
        }



        /// <summary>
        /// 直接使用数据库连接操作数据库
        /// </summary>
        /// <param name="action"></param>
        public static void Direct<T>(Action<DataConnection> action) where T : IDomainObject
        {
            var objectType = typeof(T);
            Direct(objectType, action);
        }

        public static IEnumerable<dynamic> DirectQuery<T>(string sql,object param) where T : IDomainObject
        {
            IEnumerable<dynamic> result = null;
            Direct<T>((conn) =>
            {
                result = conn.Query(sql, param);
            });
            return result;
        }


        public static void Direct(Type objectType, Action<DataConnection> action)
        {
            var model = DataModel.Create(objectType);
            DataContext.Using(()=> {
                var conn = DataContext.Current.Connection;
                action(conn);
            });
        }

        public static void Direct(Action<DataConnection> action)
        {
            DataContext.Using(() => {
                var conn = DataContext.Current.Connection;
                action(conn);
            });
        }


        #endregion

        /// <summary>
        /// 在数据层创建指定对象的数据
        /// </summary>
        /// <param name="obj"></param>
        internal static void Create(DomainObject obj)
        {
            var objectType = obj.GetType();
            var model = DataModel.Create(objectType);
            model.Insert(obj);
        }

        /// <summary>
        /// 在数据层中修改指定对象的数据
        /// </summary>
        /// <param name="obj"></param>
        internal static void Update(DomainObject obj)
        {
            Type objectType = obj.GetType();
            var model = DataModel.Create(objectType);
            model.Update(obj);
        }

        /// <summary>
        /// 在数据层中删除指定对象的数据
        /// </summary>
        /// <param name="obj"></param>
        internal static void Delete(DomainObject obj)
        {
            var objectType = obj.GetType();
            var model = DataModel.Create(objectType);
            model.Delete(obj);
        }

        #region keep

        /// <summary>
        /// 赋予新建的对象可以持续存在多个线程之中，
        /// 有时候，我们需要建立领域对象，但是并不存入数据库，而是作为内存中的缓存使用，
        /// 这时候在创建对象后，就需要将对象keep下，才能在随后的多个线程里正常访问，
        /// 这里的原因是，引用的根对象是存在线程栈里的，在别的线程访问时，没有本地数据做支持就无法延迟加载，所以需要数据层处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static void Keep<T>(T obj) where T : DomainObject
        {
            var objectType = obj.ObjectType;
            var table = DataTable.GetTable<T>();
            if (table == null) throw new ApplicationException("没有找到" + objectType.FullName + "对应的表定义");
            table.Keep(obj);
        }

        #endregion

        /// <summary>
        /// 将指定的对象从缓存区从移除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        public static void Off<T>(object id) where T : DomainObject
        {
            var objectType = typeof(T);
            DomainBuffer.Public.Remove(objectType, id);
        }


        /// <summary>
        /// 在数据层中查找指定编号的数据，并加载到对象实例中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        internal static T QuerySingle<T>(object id, QueryLevel level) where T : class, IEntityObject
        {
            var objectType = typeof(T);
            var model = DataModel.Create(objectType);
            return model.QuerySingle<T>(id, level);
        }


        /// <summary>
        /// 基于对象表达式的查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        internal static T QuerySingle<T>(string expression, Action<DynamicData> fillArg, QueryLevel level) where T : class, IDomainObject
        {
            var objectType = typeof(T);
            var model = DataModel.Create(objectType);
            return model.QuerySingle<T>(expression, fillArg, level);
        }


        /// <summary>
        /// 基于对象表达式的查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        internal static IEnumerable<T> Query<T>(string expression, Action<DynamicData> fillArg, QueryLevel level) where T : class, IDomainObject
        {
            var objectType = typeof(T);
            var model = DataModel.Create(objectType);
            return model.Query<T>(expression, fillArg, level);
        }

        /// <summary>
        /// 基于对象表达式的查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        internal static Page<T> Query<T>(string expression, int pageIndex, int pageSize, Action<DynamicData> fillArg) where T : class, IDomainObject
        {
            var objectType = typeof(T);
            var model = DataModel.Create(objectType);
            return model.Query<T>(expression, pageIndex, pageSize, fillArg);
        }


        internal static int GetCount<T>(string expression, Action<DynamicData> fillArg, QueryLevel level) where T : class, IDomainObject
        {
            var objectType = typeof(T);
            var model = DataModel.Create(objectType);
            return model.GetCount(expression, fillArg, level);
        }

        internal static void Execute<T>(string expression, Action<DynamicData> fillArg, QueryLevel level) where T : class, IDomainObject
        {
            var objectType = typeof(T);
            var model = DataModel.Create(objectType);
            model.Execute(expression, fillArg, level);
        }
    }
}
