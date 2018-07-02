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
    /// 文件基类
    /// </summary>
    [Aspect(typeof(WebFileInitializer))]
    public class WebFile : FilePage
    {
        protected override int GetBufferSize(long size)
        {
            const int minBufferSize = 100 * 1024 * 50; //每次最小传输5000K
            return minBufferSize;
        }

        protected override long GetFileSize()
        {
            string key = this.Request.QueryString["key"];
            long size = 0;
            var storage = FileStorage.Instance;
            using (var stream = storage.Load(key))
            {
                size = stream.Length;
            }
            return size;
        }

        protected override byte[] ReadFileBuffer(long offset, int count)
        {
            string key = this.Request.QueryString["key"];
            if (string.IsNullOrEmpty(key)) return null;

            byte[] buffer = null;
            var storage = FileStorage.Instance;
            using (var stream = storage.Load(key))
            {
                stream.Seek(offset, SeekOrigin.Begin);
                long size = (offset + count) < stream.Length ? count : (stream.Length - offset);
                buffer = new byte[size];
                stream.Read(buffer, 0, buffer.Length);
            }
            return buffer;


        }
    }
}