using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeArt.AppSetting;

namespace CodeArt.DomainDriven
{
    public abstract class Command : CommandBase, ICommand
    {
        public void Execute()
        {
            Execute(ExecuteProcedure);
        }

        protected abstract void ExecuteProcedure();

    }

    public abstract class Command<T> : CommandBase, ICommand<T>
    {
        public T Execute()
        {
            T result = default(T);
            Execute(() =>
            {
                result = ExecuteProcedure();
            });
            return result;
        }

        protected abstract T ExecuteProcedure();
    }

    public abstract class CommandBase
    {
        protected void Execute(Action action)
        {
            var dataContext = DataContext.Current;
            try
            {
                dataContext.BeginTransaction();
                action();
                dataContext.Commit();
            }
            catch (Exception ex)
            {
                Failed();
                if (dataContext.IsInTransaction)
                {
                    dataContext.Rollback();
                }
                throw ex;
            }
            Success();
        }



        /// <summary>
        /// 当事务失败时所做的处理
        /// </summary>
        protected virtual void Failed() { }

        /// <summary>
        /// 当事务成功时所做的处理
        /// </summary>
        protected virtual void Success() { }
    }

   
}
