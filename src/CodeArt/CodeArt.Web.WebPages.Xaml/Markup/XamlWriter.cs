using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace CodeArt.Web.WebPages.Xaml.Markup
{
    public sealed class XamlWriter : IXamlWriter
    {
        private XamlWriter() { }

        public string Write(UIElement element)
        {
            throw new NotImplementedException();
        }

        public static string Save(UIElement element)
        {
            return XamlWriter.Instance.Write(element);
        }

        private static XamlWriter Instance = new XamlWriter();

    }

}
