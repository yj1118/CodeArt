using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeArt.AppSetting;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 代表是领域事件相关的命令
    /// </summary>
    public interface IEventCommand : ICommand
    {

    }

    public abstract class EventCommand<ET, RT> : CommandBase, ICommandImp<RT>, IEventCommand
        where ET : DomainEvent
    {
        public virtual RT Execute()
        {
            var queueId = Guid.NewGuid();
            var future = CreateFuture(queueId);
            future.Start();
            ExecuteImpl(() =>
            {
                ET source = default(ET);
                DataContext.Using(() =>
                {
                    source = CreateEvent();
                });

                EventProtector.UseNewQueue(queueId,(callback) =>
                {
                    EventTrigger.Start(queueId, source, callback);
                });
            });

            {
                if(future.HasError)
                {
                    throw future.Error;
                }

                var source = future.Result;
                return GetResult(source);
            }
        }

        protected abstract ET CreateEvent();

        protected abstract RT GetResult(ET @event);

        private static ConcurrentDictionary<Guid, Future<ET>> _futures = new ConcurrentDictionary<Guid, Future<ET>>();

        private static Future<ET> CreateFuture(Guid queueId)
        {
            var future = new Future<ET>();
            _futures.TryAdd(queueId, future);
            return future;
        }

        static EventCommand()
        {
            DomainEvent.Succeeded += OnDomainEventCompleted;
            DomainEvent.Failed += OnDomainEventFailed;
            DomainEvent.Error += OnDomainEventError;
        }

        private static void OnDomainEventCompleted(Guid queueId, DomainEvent @event)
        {
            if(_futures.TryRemove(queueId, out var future))
            {
                future.SetResult(@event as ET);
            }
        }

        private static void OnDomainEventFailed(Guid queueId, Exception reason)
        {
            if (_futures.TryRemove(queueId, out var future))
            {
                future.SetError(reason);
            }
        }

        private static void OnDomainEventError(Guid queueId, EventErrorException ex)
        {
            if (_futures.TryRemove(queueId, out var future))
            {
                future.SetError(ex);
            }
        }
    }

    /// <summary>
    /// 不带返回值得领域事件命令
    /// </summary>
    /// <typeparam name="ET"></typeparam>
    public abstract class EventCommand<ET> : EventCommand<ET, object>
       where ET : DomainEvent
    {
        protected override object GetResult(ET @event)
        {
            return null;
        }
    }
}
