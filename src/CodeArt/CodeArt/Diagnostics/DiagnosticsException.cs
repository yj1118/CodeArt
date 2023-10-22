using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace CodeArt.Diagnostics
{
    /// <summary>
    /// 
    /// </summary>
    public class DiagnosticsException : Exception
    {
        public DiagnosticsException(string message)
            : base(message)
        {
        }
    }
}
