using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.Data
{
    public static class SqlContext
    {
        public static ISqlConnectionProvider GetConnectionProvider()
        {
            if (_connectionProvider == null) _connectionProvider = SqlConnectionProvider.Instance;
            return _connectionProvider;
        }

        private static ISqlConnectionProvider _connectionProvider;

        public static void RegisterConnectionProvider(ISqlConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

    }
}