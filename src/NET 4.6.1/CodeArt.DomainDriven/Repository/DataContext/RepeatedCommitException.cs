using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Transactions;

using CodeArt;

namespace CodeArt.DomainDriven
{
    public class RepeatedCommitException : DomainDrivenException
    {
        public RepeatedCommitException(String message)
            : base(message)
        {
        }
    }
}
