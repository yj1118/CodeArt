using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

using CodeArt.Concurrent;

namespace CodeArt.DomainDriven
{
    [SafeAccess]
    internal sealed class TransactionScopeManagerFactory : ITransactionManagerFactory
    {
        private TransactionScopeManagerFactory() { }

        public ITransactionManager CreateManager()
        {
            return null;
            //return new SqlTransactionManager();
        }

        public static readonly TransactionScopeManagerFactory Instance = new TransactionScopeManagerFactory();

    }
}
