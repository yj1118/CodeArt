using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CodeArt.DomainDriven
{
    public interface ITransactionManager : IDisposable
    {
        /// <summary>
        /// 开始事务
        /// </summary>
        void Begin();

        /// <summary>
        /// 回滚事务
        /// </summary>
        void RollBack();

        /// <summary>
        /// 提交事务
        /// </summary>
        void Commit();
    }
}
