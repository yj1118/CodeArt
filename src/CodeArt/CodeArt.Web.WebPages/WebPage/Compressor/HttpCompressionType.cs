using System;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Web;

namespace CodeArt.Web.WebPages
{
    public enum HttpCompressionType : byte
    {
        None = 1, 
        GZip = 2, 
        Deflate = 3
    }
}
