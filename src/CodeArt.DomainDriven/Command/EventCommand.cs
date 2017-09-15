using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeArt.AppSetting;

namespace CodeArt.DomainDriven
{
    public abstract class EventCommand<T> : CommandBase, ICommand
        where T : DomainEvent
    {
        public void Execute()
        {
            ExecuteImpl(() =>
            {
                T source = default(T);
                DataContext.Using(() =>
                {
                    source = CreateEvent();
                });
                EventTrigger.Start(source);
            });
        }

        protected abstract T CreateEvent();
    }
}
