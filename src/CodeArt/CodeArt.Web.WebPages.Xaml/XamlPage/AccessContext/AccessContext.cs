using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CodeArt.Web;
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages.Xaml
{
    public class AccessContext
    {
        public static IAccessContext Current
        {
            get
            {
                return AccessContextFactory.GetContext();
            }
        }
    }
   
}
