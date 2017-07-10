using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

using CodeArt;
using CodeArt.Runtime;
using CodeArt.AppSetting;

namespace CodeArt.DomainDriven
{
    public abstract class DomainEventHandler<H, T> : IDomainEventHandler
        where T : DomainEvent
        where H : DomainEventHandler<H, T>, new()
    {
        protected DomainEventHandler() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        public void Handle(IDomainEvent @event)
        {
            Handle((T)@event);
        }

        protected abstract void Handle(T @event);

        public static readonly H Instance = new H();
    }
}