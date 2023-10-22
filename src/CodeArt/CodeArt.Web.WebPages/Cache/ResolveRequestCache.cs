using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using CodeArt.Concurrent;
using CodeArt.Util;

namespace CodeArt.Web.WebPages
{
    /// <summary>
    /// 
    /// </summary>
    public class ResolveRequestCache
    {
        private WebPageContext _context;
        private string _pageKey;

        internal ResolveRequestCache(WebPageContext context)
        {
            _context = context;
        }

        public WebPageContext Context
        {
            get
            {
                return _context;
            }
        }

        private static Func<string, string[]> _getQuerys = LazyIndexer.Init<string, string[]>((query) =>
        {
            if (string.IsNullOrEmpty(query)) return Array.Empty<string>();
            return query.Split(',');
        });


        public string PageKey
        {
            get
            {
                if (_pageKey == null) _pageKey = GetPageKey();
                return _pageKey;
            }
        }

        public HttpCompressionType CompressionType
        {
            get
            {
                return _context.CompressionType;
            }
        }

        public ClientDevice Device
        {
            get
            {
                return _context.Device;
            }
        }

        private string GetPageKey()
        {
            var query = _context.GetConfigValue<string>("Page", "query", string.Empty);
            var temp = _getQuerys(query);

            var uri = _context.VirtualPath;
            if (temp.Length == 0) return uri;
            if (temp.Length == 1)
            {
                var name = temp[0];
                var value = _context.GetQueryValue<string>(name, string.Empty);
                return string.Format("{0}-{1}-{2}", uri, name, value);
            }

            using (var pool = StringPool.Borrow())
            {
                var code = pool.Item;
                code.Append(uri);
                foreach (var name in temp)
                {
                    var value = _context.GetQueryValue<string>(name, string.Empty);
                    code.AppendFormat("-{0}-{1}", name, value);
                }
                return code.ToString();
            }
        }

        #region 缓存

        /// <summary>
        /// 缓存加载
        /// </summary>
        /// <returns></returns>
        public bool LoadFromCache()
        {
            return ClientCacheIsEffective() || TryOutputFromServerCache();
        }

        private bool _isLoadFromServerCache = false;


        protected bool ClientCacheIsEffective()
        {
            return !this.ClientCache.IsExpired(this);//客户端缓存没有过期
        }

        public void SetCache(Stream content)
        {
            if (!_isLoadFromServerCache) //缓存无效时需要更新缓存
            {
                WriteServerCache(content);
                SetClientCache();
            }
        }

        private IClientCache _clientCache = null;
        private IClientCache ClientCache
        {
            get
            {
                if (_clientCache == null) _clientCache = ClientCacheFactory.Create(_context);
                return _clientCache;
            }
        }

        private IServerCache _serverCache = null;
        private IServerCache ServerCache
        {
            get
            {
                if (_serverCache == null) _serverCache = ServerCacheFactory.Create(_context);
                return _serverCache;
            }
        }


        private ICacheStorage _storage = null;
        private ICacheStorage Storage
        {
            get
            {
                if (_storage == null)
                {
                    return InjectionServerCacheFactory.Instance.CreateStorage(_context);
                }
                return _storage;
            }
        }


        private bool TryOutputFromServerCache()
        {
            IServerCache server = this.ServerCache;
            bool cacheIsEffective = false;
            if (!server.IsExpired(this,this.Storage))
            {
                //服务器端缓存没有过期
                using (Stream stream = server.Read(this, this.Storage))
                {
                    if (stream != null)
                    {
                        _isLoadFromServerCache = true;
                        WebPageWriter.Instance.Write(_context, stream);
                        cacheIsEffective = true;
                    }
                }
            }
            return cacheIsEffective;
        }

        private void WriteServerCache(Stream content)
        {
            this.ServerCache.Write(this, content, this.Storage);
        }

        private void SetClientCache()
        {
            //设置客户端缓存
            this.ClientCache.SetCache(this);
        }

        #endregion

        //public static void Delete(string storageName, IList<CacheVariable> variables)
        //{
        //    var storage = ServerCacheFactory.CreateStorage(storageName);
        //    foreach (var variable in variables)
        //    {
        //        storage.Delete(variable);
        //    }
        //}

        //public static void Delete(IList<CacheVariable> variables)
        //{
        //    var storages = ServerCacheFactory.CreateStorages();
        //    foreach (var storage in storages)
        //    {
        //        foreach (var variable in variables)
        //        {
        //            storage.Delete(variable);
        //        }
        //    }
        //}

        /// <summary>
        /// 是否为删除缓存的操作
        /// </summary>
        /// <returns></returns>
        public bool RemoveCache()
        {
            var tip = _context.GetQueryValue<string>("CodeArtWebPageRemoveCache", string.Empty);
            if (string.IsNullOrEmpty(tip)) return false;
            if (tip == "all")
            {
                this.DeleteAll();
                return true;
            }

            DeleteSingle(); //删除单个缓存，就是与目前页面路径匹配的缓存
            return true;
        }

        private void DeleteAll()
        {
            var storages = ServerCacheFactory.CreateStorages();
            foreach (var storage in storages)
            {
                storage.DeleteAll();
            }
        }

        private void DeleteSingle()
        {
            var storages = ServerCacheFactory.CreateStorages();
            foreach (var storage in storages)
            {
                storage.Delete(new CacheVariable(this.PageKey, HttpCompressionType.None, _pc));
                storage.Delete(new CacheVariable(this.PageKey, HttpCompressionType.None, _mobile));

                storage.Delete(new CacheVariable(this.PageKey, HttpCompressionType.GZip, _pc));
                storage.Delete(new CacheVariable(this.PageKey, HttpCompressionType.GZip, _mobile));

                storage.Delete(new CacheVariable(this.PageKey, HttpCompressionType.Deflate, _pc));
                storage.Delete(new CacheVariable(this.PageKey, HttpCompressionType.Deflate, _mobile));
            }
        }


        private static ClientDevice _pc = new ClientDevice() { IsMobile = false };
        private static ClientDevice _mobile = new ClientDevice() { IsMobile = true };


        /// <summary>
        /// 删除url对应的所有缓存
        /// </summary>
        public static void DeleteAll(string url)
        {
            var p = url.IndexOf("?") > -1 ? "&" : "?";
            WebUtil.SendGet(string.Format("{0}{1}CodeArtWebPageRemoveCache=all",url, p));
        }

        /// <summary>
        /// 删除url对应的单条数据缓存
        /// </summary>
        /// <param name="url"></param>
        public static void Delete(string url)
        {
            var p = url.IndexOf("?") > -1 ? "&" : "?";
            WebUtil.SendGet(string.Format("{0}{1}CodeArtWebPageRemoveCache=true", url, p));
        }

    }
}