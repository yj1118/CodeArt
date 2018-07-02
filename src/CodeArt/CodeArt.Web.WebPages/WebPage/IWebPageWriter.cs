using System;
using System.Web;
using System.Text;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;
using System.IO;


namespace CodeArt.Web.WebPages
{
    internal interface IWebPageWriter
    {
        void Write(WebPageContext context, Stream content);
    }
}
