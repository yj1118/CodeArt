using System;
using System.Collections.Generic;

namespace CodeArt.DomainDriven.DataAccess
{
    public interface ISqlConnectionProvider
    {
        string GetConnectionString(string connName);
    }
}