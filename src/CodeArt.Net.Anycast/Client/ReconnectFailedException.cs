using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Net;

using CodeArt.Util;

namespace CodeArt.Net.Anycast
{
    public class ReconnectFailedException : Exception
    {
        public ReconnectFailedException()
            : base(Strings.ReconnectFailedError)
        {
        }
    }
}
