using System;
using System.Collections.Generic;
using System.Web;
using System.IO;


namespace CodeArt.Web.WebPages
{
    /// <summary>
    /// 
    /// </summary>
    public class ResolveRequestCache
    {
        private WebPageContext _context;

        internal ResolveRequestCache(WebPageContext context)
        {
            _context = context;
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
            return !this.ClientCache.IsExpired(_context);//客户端缓存没有过期
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
            if (!server.IsExpired(_context,this.Storage))
            {
                //服务器端缓存没有过期
                using (Stream stream = server.Read(_context, this.Storage))
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
            this.ServerCache.Write(_context, content, this.Storage);
        }

        private void SetClientCache()
        {
            //设置客户端缓存
            this.ClientCache.SetCache(_context);
        }

        #endregion

        public static void Delete(string storageName, IList<CacheVariable> variables)
        {
            var storage = ServerCacheFactory.CreateStorage(storageName);
            foreach (var variable in variables)
            {
                storage.Delete(variable);
            }
        }

        public static void Delete(IList<CacheVariable> variables)
        {
            var storages = ServerCacheFactory.CreateStorages();
            foreach (var storage in storages)
            {
                foreach (var variable in variables)
                {
                    storage.Delete(variable);
                }
            }
        }

        public static void Delete(string url)
        {
            var storages = ServerCacheFactory.CreateStorages();
            foreach (var storage in storages)
            {
                storage.Delete(new CacheVariable(url, HttpCompressionType.None, _pc));
                storage.Delete(new CacheVariable(url, HttpCompressionType.None, _mobile));

                storage.Delete(new CacheVariable(url, HttpCompressionType.GZip, _pc));
                storage.Delete(new CacheVariable(url, HttpCompressionType.GZip, _mobile));

                storage.Delete(new CacheVariable(url, HttpCompressionType.Deflate, _pc));
                storage.Delete(new CacheVariable(url, HttpCompressionType.Deflate, _mobile));
            }
        }


        private static ClientDevice _pc = new ClientDevice() { IsMobile = false };
        private static ClientDevice _mobile = new ClientDevice() { IsMobile = false };

    }
}