using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

using HtmlAgilityPack;

namespace CodeArt.HtmlWrapper
{
    public static class HtmlAttributeCollectionExtensions
    {
        internal static int GetAttributeIndex(this HtmlAttributeCollection attrs, string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            string str = name.ToLower();
            for (int i = 0; i < attrs.Count; i++)
            {
                if (attrs[i].Name == str)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}