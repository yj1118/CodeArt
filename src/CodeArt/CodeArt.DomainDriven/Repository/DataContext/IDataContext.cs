using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CodeArt.DomainDriven
{
    public interface IDataContext : IDisposable
    {
        #region 事务管理
        /// <summary>
        /// 表示当前数据上下文的实例是否正在事务模式中
        /// </summary>
        bool InTransaction { get; }

        /// <summary>
        /// 开始进入事务模式
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// 提交所有执行计划
        /// </summary>
        void Commit();
        /// <summary>
        /// 抛弃所有未提交的执行计划，并重置事务模式
        /// </summary>
        void Rollback();

        /// <summary>
        /// 注册回滚项，外界可以指定回滚内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        void RegisterRollback(RepositoryRollbackEventArgs e);


        /// <summary>
        /// 执行计划中是否有未提交的单元
        /// </summary>
        bool IsDirty { get; }

        /// <summary>
        /// 是否正在提交事务
        /// </summary>
        bool IsCommiting { get; }

        #endregion

        #region 工作单元

        void RegisterAdded<T>(T item, IPersistRepository repository) where T : IAggregateRoot;

        void RegisterUpdated<T>(T item, IPersistRepository repository) where T : IAggregateRoot;

        void RegisterDeleted<T>(T item, IPersistRepository repository) where T : IAggregateRoot;

        /// <summary>
        /// 向数据上下文注册查询，该方法会控制锁和同步查询结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="level"></param>
        /// <param name="persistQuery"></param>
        /// <returns></returns>
        T RegisterQueried<T>(QueryLevel level, Func<T> persistQuery) where T : IAggregateRoot;

        /// <summary>
        /// 向数据上下文注册查询，该方法会控制锁和同步查询结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="level"></param>
        /// <param name="persistQuery"></param>
        /// <returns></returns>
        IEnumerable<T> RegisterQueried<T>(QueryLevel level, Func<IEnumerable<T>> persistQuery) where T : IAggregateRoot;

        /// <summary>
        /// 向数据上下文注册查询，该方法会控制锁和同步查询结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="level"></param>
        /// <param name="persistQuery"></param>
        /// <returns></returns>
        Page<T> RegisterQueried<T>(QueryLevel level, Func<Page<T>> persistQuery) where T : IAggregateRoot;


        #endregion

        #region 锁

        /// <summary>
        /// 开启锁
        /// </summary>
        /// <param name="level"></param>
        void OpenLock(QueryLevel level);

        #endregion
    }
}
