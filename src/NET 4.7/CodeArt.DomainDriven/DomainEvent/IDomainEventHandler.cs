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
    public interface IDomainEventHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        void Handle(IDomainEvent @event);
    }
}