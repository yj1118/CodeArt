using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

using CodeArt.Util;
using CodeArt.AppSetting;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    internal class StorageData : IEnumerable<KeyValuePair<Guid, object>>
    {
        public StorageData() { }

        private Dictionary<Guid, object> _datas = new Dictionary<Guid, object>();


        public object Load(Guid dataId)
        {
            object data = null;
            if (_datas.TryGetValue(dataId, out data)) return data;
            return null;
        }

        public void Save(Guid dataId, object value)
        {
            _datas[dataId] = value;
        }

        public bool Contains(Guid dataId)
        {
            return _datas.ContainsKey(dataId);
        }

        public void Clear()
        {
            _datas.Clear();
        }

        public IEnumerator<KeyValuePair<Guid, object>> GetEnumerator()
        {
            return _datas.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _datas.GetEnumerator();
        }

    }
}
