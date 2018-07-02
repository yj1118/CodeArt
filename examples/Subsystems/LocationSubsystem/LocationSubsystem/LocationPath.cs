using System;
using System.Text;
using System.Collections.Generic;
using System.Collections;

namespace LocationSubsystem
{
    /// <summary>
    /// 地理位置的路径，不包括节点自身
    /// </summary>
    public sealed class LocationPath : IEnumerable<Location>
    {
        private List<Location> _list;
        public List<Location> List
        {
            get { return _list; }
        }

        public IEnumerator<Location> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public LocationPath(IEnumerable<Location> list)
        {
            _list = new List<Location>(list);
        }

        public Location GetItem(int index)
        {
            return this.List.Count > index ? this.List[index] : Location.Empty;
        }

        public string GetText(string symbol)
        {
            StringBuilder text = new StringBuilder();

            foreach (Location item in _list)
            {
                text.AppendFormat("{0}{1}", item.Name, symbol);
            }
            if (_list.Count > 0)
                text.Length -= symbol.Length;
            return text.ToString();
        }

        public string GetText()
        {
            return GetText(" > ");
        }

        public static readonly LocationPath Empty = new LocationPath(new List<Location>());

    }
}
