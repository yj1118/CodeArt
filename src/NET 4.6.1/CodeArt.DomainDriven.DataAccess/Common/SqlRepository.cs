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
            DataPortal.Create(obj as DomainObject);
        }


        protected override void PersistUpdateRoot(TRoot obj)
        {
            DataPortal.Update(obj as DomainObject);
        }
        protected override void PersistDeleteRoot(TRoot obj)
        {
            DataPortal.Delete(obj as DomainObject);
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
            return DataContext.Current.QuerySingle<T>(expression, fillArg, level);
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
            return DataContext.Current.Query<T>(expression, fillArg, level);
        }

        //public IEnumerable<T> Query<T>(IQueryBuilder compiler, Action<DynamicData> fillArg, QueryLevel level) where T : class, IRepositoryable
        //{
        //    return DataContext.Current.Query<T>(compiler, fillArg, level);
        //}

        /// <summary>
        /// 基于对象表达式的查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public Page<T> Query<T>(string expression, int pageIndex, int pageSize, Action<DynamicData> fillArg) where T : class, IAggregateRoot
        {
            return DataContext.Current.Query<T>(expression, pageIndex, pageSize, fillArg);
        }

        //public Page<T> Query<T>(IQueryBuilder pageCompiler, IQueryBuilder countCompiler, int pageIndex, int pageSize, Action<DynamicData> fillArg) where T : class, IRepositoryable
        //{
        //    return DataContext.Current.Query<T>(pageCompiler, countCompiler, pageIndex, pageSize, fillArg);
        //}

        public int GetCount<T>(string expression, Action<DynamicData> fillArg, QueryLevel level) where T : class, IAggregateRoot
        {
            return DataContext.Current.GetCount<T>(expression, fillArg, level);
        }

        //public int GetCount<T>(IQueryBuilder compiler, Action<DynamicData> fillArg, QueryLevel level) where T : class, IRepositoryable
        //{
        //    return DataContext.Current.GetCount<T>(compiler, fillArg, level);
        //}

        /// <summary>
        /// 使用适配器查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="adapterName">适配器的名称，在类型<typeparam name="T"/>下唯一，该名称会用来提高程序性能</param>
        /// <returns></returns>
        public QueryAdapter<T> Adapter<T>(string adapterName) where T : class, IAggregateRoot
        {
            return DataContext.Current.Adapter<T>(adapterName);
        }
    }
}
