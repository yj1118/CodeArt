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
//        /// �Ƿ�Ϊ���ش��������ӣ����ش����������ɱ����ͷ�
//        /// </summary>
//        private bool _localConnection = false;

//        public SqlTransactionManager()
//        {
//        }

//        /// <summary>
//        /// ��ʼ����
//        /// </summary>
//        public void Begin()
//        {
//            if(DataContext.Current.RequiresNew)
//            {
//                //�µ�����
//                _conn = DataConnection.BeginTransaction();
//                _localConnection = true;
//            }
//            else
//            {
//                if(DataConnection.Current != null)
//                {
//                    _conn = DataConnection.Current;
//                    _localConnection = false; //�����Լ�������
//                }
//                else
//                {
//                    _conn = DataConnection.BeginTransaction();
//                    _localConnection = true;
//                }
//            }
//        }

//        /// <summary>
//        /// �ع�����
//        /// </summary>
//        public void RollBack()
//        {
//            _conn.Rollback();
//        }

//        /// <summary>
//        /// �ύ����
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
//                //������Ǳ��ؽ����ģ���ô��Ҫ����
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
//    //    /// ��ʼ����
//    //    /// </summary>
//    //    public void Begin()
//    //    {
//    //        TransactionOptions option = new TransactionOptions();
//    //        option.IsolationLevel = IsolationLevel.ReadUncommitted;

//    //        var scopeOption = DataContext.Current.RequiresNew ? TransactionScopeOption.RequiresNew : TransactionScopeOption.Required;
//    //        _scope = new TransactionScope(scopeOption, option);
//    //    }

//    //    /// <summary>
//    //    /// �ع�����
//    //    /// </summary>
//    //    public void RollBack()
//    //    {
//    //        _scope.Dispose();
//    //    }

//    //    /// <summary>
//    //    /// �ύ����
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
