using System;
using System.Collections.Concurrent;
using System.Text;
using System.Web;
using System.IO;
using System.Web.Hosting;

namespace CodeArt.Web.WebPages
{
    public class DTOSenderException : Exception
    {
        public DTOSenderException(string message)
            : base(message)
        {
        }
    }
}
