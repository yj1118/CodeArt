using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven
{
    public abstract class DomainService : IDomainService
    {
        public DomainService()
        {
        }

        public abstract void Execute();
    }

    public abstract class DomainService<T> : IDomainService
    {
        public DomainService()
        {
        }

        public abstract T Execute();
    }

}