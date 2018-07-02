using System;
using System.Collections.Concurrent;
using System.Text;
using System.Web;
using System.IO;
using System.Web.Hosting;


namespace CodeArt.Web.WebPages
{
    public class WebCacheException : WebException
    {
        public WebCacheException(string message)
            : base(message)
        {
        }
    }
}
