using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace CodeArt.DomainDriven.DataAccess
{
    public class NotSupportDatabaseException : DataAccessException
    {
        public NotSupportDatabaseException(string operation, string dbType)
            : base(string.Format(Strings.NotSupportDatabase, operation, dbType))
        {
        }

        public NotSupportDatabaseException(string operation)
            : this(operation, SqlContext.GetDbType())
        {
        }
    }
}
