using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.IO;

namespace CodeArt.Web.WebPages
{
    public sealed class StorageEmpty : ICacheStorage
    {
        private StorageEmpty() { }

        public static ICacheStorage Instance = new StorageEmpty();


        public bool TryGetLastModified(CacheVariable variable, out DateTime lastModified)
        {
            lastModified = DateTime.Now;
            return false;
        }

        public Stream Read(CacheVariable variable)
        {
            return null;
        }

        /// <summary>
        /// 更新缓存器内容
        /// </summary>
        /// <param name="variable"></param>
        public void Update(CacheVariable variable, Stream content)
        {
           
        }

        public void Delete(CacheVariable variable)
        {

        }

        public void DeleteAll()
        {

        }

    }
}