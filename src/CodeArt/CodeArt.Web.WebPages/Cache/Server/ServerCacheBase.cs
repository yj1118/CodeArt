using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;


using CodeArt.Util;
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages
{
    public abstract class ServerCacheBase : IServerCache
    {
        public abstract bool IsExpired(ResolveRequestCache controller, ICacheStorage storage);

        /// <summary>
        /// 读取缓存区中的流信息
        /// </summary>
        /// <returns></returns>
        public abstract Stream Read(ResolveRequestCache controller, ICacheStorage storage);

        /// <summary>
        /// 向缓存区中写入信息
        /// </summary>
        /// <param name="content"></param>
        public abstract void Write(ResolveRequestCache controller, Stream content, ICacheStorage storage);

    }
}