using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;

using CodeArt.AppSetting;
using CodeArt.Util;

namespace CodeArt.DomainDriven.DataAccess
{
    public abstract class SqlRepository<TRoot> : Repository<TRoot>
        where TRoot : class, IAggregateRoot
    {
        #region 增删改

        protected override void PersistAddRoot(TRoot obj)
        {
            DataContext.Using(() =>
            {
                DataPortal.Create(obj as DomainObject);
            });
        }


        protected override void PersistUpdateRoot(TRoot obj)
        {
            DataContext.Using(() =>
            {
                DataPortal.Update(obj as DomainObject);
            });
        }
        protected override void PersistDeleteRoot(TRoot obj)
        {
            DataContext.Using(() =>
            {
                DataPortal.Delete(obj as DomainObject);
            });
        }

        #endregion

        protected override TRoot PersistFind(object id, QueryLevel level)
        {
            return DataPortal.QuerySingle<TRoot>(id, level);
        }

        /// <summary>
        /// 基于对象表达式的查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        protected T QuerySingle<T>(string expression, Action<DynamicData> fillArg, QueryLevel level) where T : class, IAggregateRoot
        {
            T result = null;
            DataContext.Using(() =>
            {
                result = DataContext.Current.QuerySingle<T>(expression, fillArg, level);
            });
            return result;
        }

        //protected T QuerySingle<T>(IQueryBuilder compiler, Action<DynamicData> fillArg, QueryLevel level) where T : class, IRepositoryable
        //{
        //    return DataContext.Current.QuerySingle<T>(compiler, fillArg, level);
        //}

        /// <summary>
        /// 基于对象表达式的查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(string expression, Action<DynamicData> fillArg, QueryLevel level) where T : class, IAggregateRoot
        {
            IEnumerable<T> result = null;
            DataContext.Using(() =>
            {
                result = DataContext.Current.Query<T>(expression, fillArg, level);
            });
            return result;
        }

        /// <summary>
        /// 基于对象表达式的查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public Page<T> Query<T>(string expression, int pageIndex, int pageSize, Action<DynamicData> fillArg) where T : class, IAggregateRoot
        {
            Page<T> result = default(Page<T>);
            DataContext.Using(() =>
            {
                result = DataContext.Current.Query<T>(expression, pageIndex, pageSize, fillArg);
            });
            return result;
        }

        public int GetCount<T>(string expression, Action<DynamicData> fillArg, QueryLevel level) where T : class, IAggregateRoot
        {
            int result = 0;
            DataContext.Using(() =>
            {
                result = DataContext.Current.GetCount<T>(expression, fillArg, level);
            });
            return result;
        }

        /// <summary>
        /// 使用适配器查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="adapterName">适配器的名称，在类型<typeparam name="T"/>下唯一，该名称会用来提高程序性能</param>
        /// <returns></returns>
        public QueryAdapter<T> Adapter<T>(string adapterName) where T : class, IAggregateRoot
        {
            QueryAdapter<T> result = null;
            DataContext.Using(() =>
            {
                result = DataContext.Current.Adapter<T>(adapterName);
            });
            return result;
        }
    }
}
