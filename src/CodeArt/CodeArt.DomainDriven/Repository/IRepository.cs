using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 为了方便实际使用中重构代码，新的版本中提供了非泛型版的CUD操作接口
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// 根据编号查找对象
        /// </summary>
        /// <param name="id"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        IAggregateRoot Find(object id, QueryLevel level);


        /// <summary>
        /// 将对象添加到仓储
        /// </summary>
        /// <param name="obj"></param>
        void Add(IAggregateRoot obj);

        /// <summary>
        /// 修改对象在仓储中的信息
        /// </summary>
        /// <param name="obj"></param>
        void Update(IAggregateRoot obj);

        /// <summary>
        /// 从仓储中删除对象
        /// </summary>
        /// <param name="obj"></param>
        void Delete(IAggregateRoot obj);
    }



    public interface IRepository<TRoot> : IRepository
        where TRoot : class, IAggregateRoot
    {
        /// <summary>
        /// 将对象添加到仓储
        /// </summary>
        /// <param name="obj"></param>
        void Add(TRoot obj);

        /// <summary>
        /// 修改对象在仓储中的信息
        /// </summary>
        /// <param name="obj"></param>
        void Update(TRoot obj);

        /// <summary>
        /// 从仓储中删除对象
        /// </summary>
        /// <param name="obj"></param>
        void Delete(TRoot obj);

        /// <summary>
        /// 根据编号查找对象
        /// </summary>
        /// <param name="id"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        new TRoot Find(object id, QueryLevel level);
    }
}