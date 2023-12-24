using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace CodeArt.Web
{
    public class WebException : Exception
    {
        public WebException()
            : base()
        {
        }

        public WebException(string message)
            : base(message)
        {
        }

    }
}