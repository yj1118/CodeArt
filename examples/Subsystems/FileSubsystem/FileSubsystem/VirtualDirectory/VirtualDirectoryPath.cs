using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace FileSubsystem
{
    public sealed class VirtualDirectoryPath
    {
        private IEnumerable<VirtualDirectory> _list;
        public IEnumerable<VirtualDirectory> List
        {
            get { return _list; }
        }

        public VirtualDirectoryPath(IEnumerable<VirtualDirectory> list)
        {
            _list = list;
        }

        public string GetText(string symbol)
        {
            StringBuilder text = new StringBuilder();
            foreach (VirtualDirectory item in _list)
            {
                text.AppendFormat("{0}{1}", item.Name, symbol);
            }
            if (_list.Count() > 0)
                text.Length -= symbol.Length;
            return text.ToString();
        }

        public string GetText()
        {
            return GetText(" > ");
        }

        public static readonly VirtualDirectoryPath Empty = new VirtualDirectoryPath(new List<VirtualDirectory>());

    }
}
