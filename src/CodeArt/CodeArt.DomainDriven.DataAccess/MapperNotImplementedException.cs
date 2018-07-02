using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace CodeArt.DomainDriven.DataAccess
{
    public class MapperNotImplementedException : DataAccessException
    {
        public MapperNotImplementedException(Type objectType, string operation)
            : base(string.Format(Strings.NoPersistentOperation, objectType.Name, operation))
        {
        }
    }
}
