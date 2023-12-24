using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using CodeArt.IO;
using CodeArt.Util;

using HtmlAgilityPack;
using CodeArt.HtmlWrapper;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    public class XamlException : Exception
    {
        public XamlException()
            : base()
        {
        }

        public XamlException(string message)
            : base(message)
        {
        }

    }

}
