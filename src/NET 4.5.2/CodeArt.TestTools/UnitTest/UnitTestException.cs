using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace CodeArt.TestTools
{
    /// <summary>
    /// 
    /// </summary>
    public class UnitTestException : Exception
    {
        public UnitTestException()
            : base()
        {
        }

        public UnitTestException(string message)
            : base(message)
        {
        }
    }
}
