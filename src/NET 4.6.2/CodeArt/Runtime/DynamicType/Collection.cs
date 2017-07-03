using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace CodeArt.Runtime
{
    internal class Collection<T>
    {
        private T[] _array;

        public T[] Array
        {
            get
            {
                if (_array == null)
                {
                    _array = this.List.ToArray();
                }
                return _array;
            }
        }


        public List<T> List
        {
            get;
            private set;
        }

        public Collection()
        {
            this.List = new List<T>();
        }


        public void Add(T item)
        {
            this.List.Add(item);
            _array = null;
        }
    }


}
