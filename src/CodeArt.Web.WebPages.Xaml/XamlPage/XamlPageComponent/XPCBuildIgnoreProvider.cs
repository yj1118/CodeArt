using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Compilation;
using System.CodeDom;
using System.IO;
using System.Web;

using Microsoft.CSharp;
using CodeArt.Web.WebPages;
using CodeArt.IO;

namespace CodeArt.Web.WebPages.Xaml
{
    public class XPCBuildIgnoreProvider : BuildProvider
    {
        public override void GenerateCode(AssemblyBuilder assemblyBuilder)
        {
            //什么都不要做
        }
    }


}
