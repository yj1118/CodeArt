using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace CodeArt.Drawing
{
    /// <summary>
    /// 
    /// </summary>
    public class DrawingException : Exception
    {
        public DrawingException()
            : base()
        {
        }

        public DrawingException(string message)
            : base(message)
        {
        }
    }
}
