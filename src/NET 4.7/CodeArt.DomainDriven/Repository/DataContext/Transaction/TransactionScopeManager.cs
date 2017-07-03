using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace CodeArt.DomainDriven
{
    internal sealed class TransactionScopeManager : ITransactionManager
    {
        private TransactionScope _scope = null;

        public TransactionScopeManager()
        {
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        public void Begin()
        {
            TransactionOptions option = new TransactionOptions();
            option.IsolationLevel = IsolationLevel.ReadUncommitted;
            _scope = new TransactionScope(TransactionScopeOption.Required, option);
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollBack()
        {
            _scope.Dispose();
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void Commit()
        {
            _scope.Complete();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}
