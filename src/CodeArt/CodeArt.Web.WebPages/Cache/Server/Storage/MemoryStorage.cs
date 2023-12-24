using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.IO;
using System.Net;
using CodeArt.IO;

namespace CodeArt.Web.WebPages
{
    public sealed class MemoryStorage : ICacheStorage
    {
        private MemoryStorage() { }

        public static MemoryStorage Instance = new MemoryStorage();

        private static ConcurrentDictionary<string, DateTime> _modifiedData = new ConcurrentDictionary<string, DateTime>();

        private static ConcurrentDictionary<string, byte[]> _contentData = new ConcurrentDictionary<string, byte[]>();


        public bool TryGetLastModified(CacheVariable variable, out DateTime lastModified)
        {
            return _modifiedData.TryGetValue(variable.UniqueCode, out lastModified);
        }

        public Stream Read(CacheVariable variable)
        {
            byte[] buffer = null;
            if (_contentData.TryGetValue(variable.UniqueCode, out buffer)) return new MemoryStream(buffer);
            return null;
        }

        /// <summary>
        /// 更新缓存器内容
        /// </summary>
        /// <param name="variable"></param>
        public void Update(CacheVariable variable, Stream content)
        {
            if (content.Length > int.MaxValue)
                throw new WebCacheException("MemoryStorage无法存储超过" + int.MaxValue + "个字节的缓存内容！");

            byte[] buffer = new byte[content.Length];
            content.ReadPro(buffer, 0, (int)content.Length);

            _contentData.AddOrUpdate(variable.UniqueCode, buffer, (key, value) => buffer);
            _modifiedData.AddOrUpdate(variable.UniqueCode, DateTime.Now, (key, value) => DateTime.Now);
        }


        internal void DeleteLocal(string uniqueCode)
        {
            byte[] buffer = null;
            _contentData.TryRemove(uniqueCode, out buffer);

            DateTime lastModified = DateTime.Now;
            _modifiedData.TryRemove(uniqueCode, out lastModified);
        }

        public void DeleteAll()
        {
            throw new NotImplementedException("暂未实现");
        }


        public void Delete(CacheVariable variable)
        {
            var host = new WebHost(variable.Key);
            var target = string.Format("{0}://{1}/cache/memory.htm?code={2}", host.Protocol, host.Host, HttpUtility.UrlEncode(variable.UniqueCode, Encoding.UTF8));

            using (WebClient client = new WebClient())
            {
                try
                {
                    client.DownloadData(target);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

    }
}