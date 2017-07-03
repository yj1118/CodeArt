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

        /// <summary>
        /// 根据对象类型，获取一个自增的编号，该编号由数据层维护递增
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public static long GetIdentity<T>() where T : class, IEntityObject
        {
            var objectType = typeof(T);
            var model = DataModel.Create(objectType);

            long id = 0;
            DataContext.UseTransactionScope(()=>
            {
                id = model.GetIdentity();
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
        /// 在数据层中销毁数据模型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Dispose()
        {
            DataModel.Drop();
        }

        /// <summary>
        /// 直接使用数据库连接操作数据库
        /// </summary>
        /// <param name="action"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Direct<T>(Action<IDbConnection> action) where T : IDomainObject
        {
            var objectType = typeof(T);
            var model = DataModel.Create(objectType);
            var connectionString = SqlHelper.GetConnectionString(model.ConnectionName);
            using (IDbConnection conn = new SqlConnection(connectionString))
            {
                action(conn);
            }
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
        /// 在数据层中查找指定编号的成员数据，并加载到对象实例中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        internal static T QuerySingle<T>(object rootId, object id) where T : class, IEntityObject
        {
            var objectType = typeof(T);
            var model = DataModel.Create(objectType);
            return model.QuerySingle<T>(rootId, id);
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

        //internal static T QuerySingle<T>(IQueryBuilder compiler, Action<DynamicData> fillArg, QueryLevel level) where T : class, IDomainObject
        //{
        //    var objectType = typeof(T);
        //    var model = DataModel.Create(objectType);
        //    return model.QuerySingle<T>(compiler, fillArg, level);
        //}


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

        //internal static IEnumerable<T> Query<T>(IQueryBuilder query, Action<DynamicData> fillArg) where T : class, IDomainObject
        //{
        //    var objectType = typeof(T);
        //    var model = DataModel.Create(objectType);
        //    return model.Query<T>(query, fillArg);
        //}

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


        //internal static Page<T> Query<T>(IQueryBuilder pageCompiler, IQueryBuilder countCompiler, int pageIndex, int pageSize, Action<DynamicData> fillArg) where T : class, IDomainObject
        //{
        //    var objectType = typeof(T);
        //    var model = DataModel.Create(objectType);
        //    return model.Query<T>(pageCompiler, countCompiler, pageIndex, pageSize, fillArg);
        //}

        internal static int GetCount<T>(string expression, Action<DynamicData> fillArg, QueryLevel level) where T : class, IDomainObject
        {
            var objectType = typeof(T);
            var model = DataModel.Create(objectType);
            return model.GetCount(expression, fillArg, level);
        }

        //internal static int GetCount<T>(IQueryBuilder query, Action<DynamicData> fillArg) where T : class, IDomainObject
        //{
        //    var objectType = typeof(T);
        //    var model = DataModel.Create(objectType);
        //    return model.GetCount(query, fillArg);
        //}
    }
}
