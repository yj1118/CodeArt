using System;
using System.Collections.Concurrent;
using System.Text;
using System.Web;
using System.IO;
using System.Web.Hosting;

using CodeArt;
using CodeArt.Web;
using CodeArt.Web.WebPages;

namespace Module.WebUI
{
    public class WebSecurityException : UserUIException
    {
        public WebSecurityException(string messsage)
            : base(messsage)
        {
        }
    }
}
