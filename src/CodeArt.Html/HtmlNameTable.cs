using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;

using CodeArt;

namespace CodeArt.HtmlWrapper
{
    internal class HtmlNameTable : XmlNameTable
    {
        // Fields
        private NameTable _nametable = new NameTable();

        // Methods
        public override string Add(string array)
        {
            return this._nametable.Add(array);
        }

        public override string Add(char[] array, int offset, int length)
        {
            return this._nametable.Add(array, offset, length);
        }

        public override string Get(string array)
        {
            return this._nametable.Get(array);
        }

        public override string Get(char[] array, int offset, int length)
        {
            return this._nametable.Get(array, offset, length);
        }

        internal string GetOrAdd(string array)
        {
            string str = this.Get(array);
            if (str == null)
            {
                return this.Add(array);
            }
            return str;
        }
    }
}