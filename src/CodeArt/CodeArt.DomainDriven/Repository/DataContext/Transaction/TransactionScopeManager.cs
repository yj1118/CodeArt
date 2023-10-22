//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Text;
//using System.Transactions;

//using CodeArt.Concurrent;

//namespace CodeArt.DomainDriven
//{
//    internal sealed class SqlTransactionManager : ITransactionManager
//    {
//        private DataConnection _conn;
//        /// <summary>
//        /// 是否为本地创建的连接，本地创建的连接由本地释放
//        /// </summary>
//        private bool _localConnection = false;

//        public SqlTransactionManager()
//        {
//        }

//        /// <summary>
//        /// 开始事务
//        /// </summary>
//        public void Begin()
//        {
//            if(DataContext.Current.RequiresNew)
//            {
//                //新的事务
//                _conn = DataConnection.BeginTransaction();
//                _localConnection = true;
//            }
//            else
//            {
//                if(DataConnection.Current != null)
//                {
//                    _conn = DataConnection.Current;
//                    _localConnection = false; //不是自己创建的
//                }
//                else
//                {
//                    _conn = DataConnection.BeginTransaction();
//                    _localConnection = true;
//                }
//            }
//        }

//        /// <summary>
//        /// 回滚事务
//        /// </summary>
//        public void RollBack()
//        {
//            _conn.Rollback();
//        }

//        /// <summary>
//        /// 提交事务
//        /// </summary>
//        public void Commit()
//        {
//            if(_localConnection)
//                _conn.Commit();
//        }

//        public void Dispose()
//        {
//            if (_localConnection)
//            {
//                //如果不是本地建立的，那么不要回收
//                DataConnection.EndTransaction(_conn);
//            }
//            _localConnection = false;
//            _conn = null;
//        }
//    }


//    //internal sealed class TransactionScopeManager : ITransactionManager
//    //{
//    //    private TransactionScope _scope = null;

//    //    public TransactionScopeManager()
//    //    {
//    //    }

//    //    /// <summary>
//    //    /// 开始事务
//    //    /// </summary>
//    //    public void Begin()
//    //    {
//    //        TransactionOptions option = new TransactionOptions();
//    //        option.IsolationLevel = IsolationLevel.ReadUncommitted;

//    //        var scopeOption = DataContext.Current.RequiresNew ? TransactionScopeOption.RequiresNew : TransactionScopeOption.Required;
//    //        _scope = new TransactionScope(scopeOption, option);
//    //    }

//    //    /// <summary>
//    //    /// 回滚事务
//    //    /// </summary>
//    //    public void RollBack()
//    //    {
//    //        _scope.Dispose();
//    //    }

//    //    /// <summary>
//    //    /// 提交事务
//    //    /// </summary>
//    //    public void Commit()
//    //    {
//    //        _scope.Complete();
//    //    }

//    //    public void Dispose()
//    //    {
//    //        _scope.Dispose();
//    //    }
//    //}
//}
