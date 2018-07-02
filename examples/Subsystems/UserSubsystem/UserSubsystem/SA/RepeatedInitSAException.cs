using System;
using System.Linq;

using CodeArt;
using CodeArt.DomainDriven;

namespace UserSubsystem
{
    public sealed class RepeatedInitSAException : UserUIException
    {
        public RepeatedInitSAException()
            : base(Strings.RepeatedInitSA)
        {

        }
    }
}
