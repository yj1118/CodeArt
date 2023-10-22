using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using CodeArt.IO;

using CodeArt.DTO;
using CodeArt.Concurrent;
using CodeArt.AppSetting;
using System.Threading;

namespace CodeArt.Caching
{
	public class DiskCacheTenant
    {
        private string _root;

        public DiskCacheTenant(string root)
        {
            _root = root;
        }

        public DiskCache GetCache()
        {
            return GetCache(AppSession.TenantId);
        }

        private ConcurrentDictionary<long, DiskCache> _caches = new ConcurrentDictionary<long, DiskCache>();

        public DiskCache GetCache(long tenantId)
        {
            DiskCache cache = null;
            long key = tenantId;
            if (_caches.TryGetValue(key, out cache)) return cache;
            else
            {
                lock (_caches)
                {
                    cache = new DiskCache(Path.Combine(_root, key.ToString()));
                    _caches.TryAdd(key, cache);
                }
                return cache;
            }
        }
    }
}
