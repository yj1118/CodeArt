﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;


namespace CodeArt.Web.WebPages
{
    public class ForeverServerCache : ServerCacheBase
    {
        private ForeverServerCache() { }

        public static ForeverServerCache Instance = new ForeverServerCache();


        /// <summary>
        /// 检查缓存是否过期
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true:缓存已过期;false:缓存未过期</returns>
        public override bool IsExpired(WebPageContext context, ICacheStorage storage)
        {
            return false;
        }

        /// <summary>
        /// 读取缓存区中的流信息
        /// </summary>
        /// <returns></returns>
        public override Stream Read(WebPageContext context, ICacheStorage storage)
        {
            var variable = new CacheVariable(GetUrl(context), context.CompressionType, context.Device);
            return storage.Read(variable);
        }

        /// <summary>
        /// 向缓存区中写入信息
        /// </summary>
        /// <param name="content"></param>
        public override void Write(WebPageContext context, Stream content, ICacheStorage storage)
        {
            var variable = new CacheVariable(GetUrl(context), context.CompressionType, context.Device);
            storage.Update(variable, content);
        }
    }
}