using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace CodeArt.DomainDriven.DataAccess
{
    public class DataAccessException : Exception
    {
        public DataAccessException(string message)
            : base(message)
        {

        }
    }
}
