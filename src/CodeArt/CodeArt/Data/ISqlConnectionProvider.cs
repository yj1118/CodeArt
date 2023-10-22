using System;
using System.Collections.Generic;

namespace CodeArt.Data
{
    public interface ISqlConnectionProvider
    {
        string GetConnectionString(string connName);

        bool ExistConnectionString(string connName);
    }
}