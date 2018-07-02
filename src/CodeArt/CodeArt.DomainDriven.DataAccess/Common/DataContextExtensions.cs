using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Data;

namespace CodeArt.DomainDriven.DataAccess
{
    /// <summary>
    /// 数据上下文扩展
    /// </summary>
    public static class DataContextExtensions
    {
        /// <summary>
        /// 由数据上下文托管的基于对象表达式的查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static T QuerySingle<T>(this IDataContext dataContext, string expression, Action<DynamicData> fillArg, QueryLevel level) where T : class, IAggregateRoot
        {
            return dataContext.RegisterQueried<T>(level, () =>
            {
                return DataPortal.QuerySingle<T>(expression, fillArg, level);
            });
        }


        /// <summary>
        /// 由数据上下文托管的基于对象表达式的查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static IEnumerable<T> Query<T>(this IDataContext dataContext, string expression, Action<DynamicData> fillArg, QueryLevel level) where T : class, IAggregateRoot
        {
            return dataContext.RegisterQueried<T>(level, () =>
            {
                return DataPortal.Query<T>(expression, fillArg, level);
            });
        }


        /// <summary>
        /// 由数据上下文托管的基于对象表达式的查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static Page<T> Query<T>(this IDataContext dataContext, string expression, int pageIndex, int pageSize, Action<DynamicData> fillArg) where T : class, IAggregateRoot
        {
            return dataContext.RegisterQueried<T>(QueryLevel.None, () =>
            {
                return DataPortal.Query<T>(expression, pageIndex, pageSize, fillArg);
            });
        }

        /// <summary>
        /// 由数据上下文托管的统计方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataContext"></param>
        /// <param name="expression"></param>
        /// <param name="fillArg"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static int GetCount<T>(this IDataContext dataContext, string expression, Action<DynamicData> fillArg, QueryLevel level) where T : class, IAggregateRoot
        {
            dataContext.OpenLock(level);
            return DataPortal.GetCount<T>(expression, fillArg, level);
        }


        public static void Execute<T>(this IDataContext dataContext, string expression, Action<DynamicData> fillArg, QueryLevel level) where T : class, IAggregateRoot
        {
            dataContext.OpenLock(level);
            DataPortal.Execute<T>(expression, fillArg, level);
        }


        /// <summary>
        /// 通过适配器查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="adapterName">适配器的名称，在类型<typeparam name="T"/>下唯一，该名称会用来提高程序性能</param>
        /// <returns></returns>
        public static QueryAdapter<T> Adapter<T>(this IDataContext dataContext, string adapterName) where T : class, IAggregateRoot
        {
            return QueryAdapter<T>.Create(adapterName);
        }
    }
}
