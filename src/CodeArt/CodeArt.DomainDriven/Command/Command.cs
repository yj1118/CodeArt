using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeArt.AppSetting;

namespace CodeArt.DomainDriven
{
    public abstract class Command : CommandBase, ICommandImp
    {
        public void Execute()
        {
            ExecuteImpl(()=>
            {
                DataContext.Using(()=>
                {
                    Logable.Using(this, () =>
                    {
                        ExecuteProcedure();
                    });
                });
            });
        }

        protected abstract void ExecuteProcedure();
    }

    public abstract class Command<T> : CommandBase, ICommandImp<T>
    {
        public T Execute()
        {
            T result = default(T);
            ExecuteImpl(() =>
            {
                DataContext.Using(() =>
                {
                    Logable.Using(this,()=>
                    {
                        result = ExecuteProcedure();
                    });
                });
            });
            return result;
        }

        protected abstract T ExecuteProcedure();
    }

    /// <summary>
    /// 该命令不会自动开启事务，需要程序员自行决定开启事务的时机
    /// </summary>
    public abstract class CommandPro : CommandBase, ICommandImp
    {
        public void Execute()
        {
            ExecuteProcedure();
        }

        /// <summary>
        /// 独立的事务
        /// </summary>
        /// <param name="action"></param>
        protected void NewScope(Action action)
        {
            DataContext.Using(()=>
            {
                Logable.Using(this, () =>
                {
                    action();
                });
            });
        }

        protected abstract void ExecuteProcedure();
    }


    public abstract class CommandPro<T> : CommandBase, ICommandImp<T>
    {
        public T Execute()
        {
            return ExecuteProcedure();
        }

        /// <summary>
        /// 独立的事务
        /// </summary>
        /// <param name="action"></param>
        protected void NewScope(Action action)
        {
            DataContext.Using(()=>
            {
                Logable.Using(this, () =>
                {
                    action();
                });
            });
        }

        protected abstract T ExecuteProcedure();
    }

}
