using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CodeArt.DomainDriven
{
    public interface IRepository
    {
        ///// <summary>
        ///// 将对象添加到仓储
        ///// </summary>
        ///// <param name="obj"></param>
        //void Add(IAggregateRoot obj);

        ///// <summary>
        ///// 修改对象在仓储中的信息
        ///// </summary>
        ///// <param name="obj"></param>
        //void Update(IAggregateRoot obj);

        ///// <summary>
        ///// 从仓储中删除对象
        ///// </summary>
        ///// <param name="obj"></param>
        //void Delete(IAggregateRoot obj);

        ///// <summary>
        ///// 锁定对象
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <param name="level">锁定级别</param>
        //void Lock(IAggregateRoot obj, QueryLevel level);

        /// <summary>
        /// 根据编号查找对象
        /// </summary>
        /// <param name="id"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        IAggregateRoot Find(object id, QueryLevel level);


        //#region 内聚边界内的高级实体对象

        //void AddEntityPro(IEntityObjectPro obj);

        //void UpdateEntityPro(IEntityObjectPro obj);

        //void DeleteEntityPro(IEntityObjectPro obj);

        ///// <summary>
        ///// 除了内聚根外，其余对象都不能带锁查询，高级引用对象也是如此，所以查询高级引用对象是不能带锁级别的
        ///// 请先带锁查询内聚根对象，再执行高级成员的查询操作
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="rootId"></param>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //IEntityObjectPro FindEntityPro(object rootId, object id);

        //#endregion
    }



    public interface IRepository<TRoot> : IRepository
        where TRoot : class, IAggregateRoot, IRepositoryable
    {
        /// <summary>
        /// 将对象添加到持久层中
        /// </summary>
        /// <param name="obj"></param>
        void Add(TRoot obj);

        /// <summary>
        /// 修改对象在持久层中的信息
        /// </summary>
        /// <param name="obj"></param>
        void Update(TRoot obj);

        /// <summary>
        /// 从持久层中删除对象
        /// </summary>
        /// <param name="obj"></param>
        void Delete(TRoot obj);

        /// <summary>
        /// 锁定对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="level">锁定级别</param>
        void Lock(TRoot obj, QueryLevel level);

        /// <summary>
        /// 根据编号查找对象
        /// </summary>
        /// <param name="id"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        new TRoot Find(object id, QueryLevel level);


        #region 内聚边界内的高级实体对象

        void AddEntityPro<T>(T obj) where T : class, IEntityObjectPro<TRoot>;

        void UpdateEntityPro<T>(T obj) where T : class, IEntityObjectPro<TRoot>;

        void DeleteEntityPro<T>(T obj) where T : class, IEntityObjectPro<TRoot>;

        /// <summary>
        /// 除了内聚根外，其余对象都不能带锁查询，高级引用对象也是如此，所以查询高级引用对象是不能带锁级别的
        /// 请先带锁查询内聚根对象，再执行高级成员的查询操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rootId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        T FindEntityPro<T>(object rootId, object id) where T : class, IEntityObjectPro<TRoot>;

        #endregion
    }
}
