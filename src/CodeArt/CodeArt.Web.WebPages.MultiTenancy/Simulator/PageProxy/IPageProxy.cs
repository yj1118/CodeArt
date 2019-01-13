using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web.Compilation;

using System.Web;
using System.IO;

using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.Log;

using CodeArt.AOP;
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages.MultiTenancy
{
    public interface IPageProxy
    {
        string Process(string contextCode);

        void Initialize(string tenancyId);

        void Dispose();
    }
}
