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
            ExecuteImpl(()=>
            {
                DataContext.Using(ExecuteProcedure);
            });
        }

        protected abstract void ExecuteProcedure();
    }

    public abstract class Command<T> : CommandBase, ICommand<T>
    {
        public T Execute()
        {
            T result = default(T);
            ExecuteImpl(() =>
            {
                DataContext.Using(() =>
                {
                    result = ExecuteProcedure();
                });
            });
            return result;
        }

        protected abstract T ExecuteProcedure();
    }   
}
