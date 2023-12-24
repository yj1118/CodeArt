using System;
using System.IO;
using System.Web;

namespace CodeArt.Web.WebPages
{
    public sealed class NonCompressor : ICompressor
    {
        private NonCompressor() { }

        public bool IsAccepted(WebPageContext context)
        {
            return false;
        }

        public void SetEncoding(WebPageContext context) { }

        public void Compress(WebPageContext context,Stream source, Stream target) { }

        public static readonly ICompressor Instance = new NonCompressor();

    }
}
