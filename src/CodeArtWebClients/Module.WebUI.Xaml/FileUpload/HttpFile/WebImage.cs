using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Configuration;

using System.Collections.Specialized;

using CodeArt.Web.WebPages;
using CodeArt.ServiceModel;

using Module.WebUI;
using CodeArt.Util;
using CodeArt.IO;

using CodeArt.AOP;
using CodeArt.Web.WebPages.Xaml;
using Module.File;

namespace Module.WebUI.Xaml
{
    /// <summary>
    /// 图片基类
    /// </summary>
    [Aspect(typeof(WebImageInitializer))]
    [AspectRemove(typeof(WebPageInitializer))]
    public class WebImage : FilePage
    {
        protected struct Query
        {
            public string FileKey;
            public int Width;
            public int Height;
            public int CutType;
        }

        protected virtual Query GetQuery()
        {
            return new Query()
            {
                FileKey = this.Request.QueryString["key"],
                Width = this.PageContext.GetQueryValue<int>("w", 0),
                Height = this.PageContext.GetQueryValue<int>("h", 0),
                CutType = this.PageContext.GetQueryValue<int>("c", 1)
            };
        }

        protected override long GetFileSize()
        {
            var query = this.GetQuery();

            long size = 0;
            var storage = FileStorage.Instance;
            using (var stream = storage.LoadByImage(query.FileKey, query.Width, query.Height, query.CutType))
            {
                size = stream.Length;
            }
            return size;
        }

        protected override byte[] ReadFileBuffer(long offset, int count)
        {
            var query = this.GetQuery();
            return Load(query.FileKey, query.Width, query.Height, query.CutType, offset, count);
        }

        protected byte[] Load(string key, int width, int height, int cutType,long offset,int count)
        {
            if (string.IsNullOrEmpty(key)) return null;

            byte[] buffer = null;
            var storage = FileStorage.Instance;
            using (var stream = storage.LoadByImage(key, width, height, cutType))
            {
                stream.Seek(offset, SeekOrigin.Begin);
                long size = (offset + count) < stream.Length ? count : (stream.Length - offset);
                buffer = new byte[size];
                stream.Read(buffer, 0, buffer.Length);
            }
            return buffer;
        }

        protected override int GetBufferSize(long size)
        {
            const int minBufferSize = 100 * 1024 * 5; //每次最小传输500K
            return minBufferSize;
        }

    }
}