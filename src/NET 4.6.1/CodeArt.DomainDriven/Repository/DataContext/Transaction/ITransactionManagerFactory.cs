using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CodeArt.DomainDriven
{
    public interface ITransactionManagerFactory
    {
        ITransactionManager CreateManager();
    }
}
