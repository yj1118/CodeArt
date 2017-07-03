using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven
{
    public sealed class AggregateRootEmpty : AggregateRoot<AggregateRootEmpty,int>
    {
        private AggregateRootEmpty()
            : base(0)
        {
        }

        public override bool IsEmpty()
        {
            return true;
        }

        public static readonly IAggregateRoot Instance = new AggregateRootEmpty();

    }
}
