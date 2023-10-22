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
            public int Quality;
        }

        protected override void PreLoad()
        {
            //System.Threading.Thread.Sleep(1000);

            var query = this.GetQuery();
            CheckSize(query);
            this.PageContext.SetItem("query", query);
        }

        private void CheckSize(Query query)
        {
            if (Sizes.Count == 0) return; //不做限制

            var allow = Sizes.Contains((t) => (t.Width == query.Width && t.Height == query.Height));
            if (!allow) throw new WebSecurityException("图片大小非法");
        }

        private Query GetQuery()
        {
            return new Query()
            {
                FileKey = this.Request.QueryString["key"],
                Width = this.PageContext.GetQueryValue<int>("w", 0),
                Height = this.PageContext.GetQueryValue<int>("h", 0),
                CutType = this.PageContext.GetQueryValue<int>("c", 1),
                Quality = this.PageContext.GetQueryValue<int>("q", 50)
            };
        }

        protected override long GetFileSize()
        {
            var query = this.PageContext.GetItem<Query>("query");

            long size = 0;
            var storage = FileStorage.Instance;
            using (var stream = storage.LoadByImage(query.FileKey, query.Width, query.Height, query.CutType, query.Quality))
            {
                size = stream.Length;
            }
            return size;
        }

        protected override byte[] ReadFileBuffer(long offset, int count)
        {
            var query = this.PageContext.GetItem<Query>("query");
            return Load(query.FileKey, query.Width, query.Height, query.CutType, query.Quality, offset, count);
        }

        protected byte[] Load(string key, int width, int height, int cutType,int highQuality, long offset,int count)
        {
            if (string.IsNullOrEmpty(key)) return null;

            byte[] buffer = null;
            var storage = FileStorage.Instance;
            using (var stream = storage.LoadByImage(key, width, height, cutType, highQuality))
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


        private static List<(int Width, int Height)> Sizes = new List<(int Width, int Height)>();

        static WebImage()
        {
            //加载Sizes
            var sizes = ConfigurationManager.AppSettings["webImages"];
            if (!string.IsNullOrEmpty(sizes))
            {
                var _sizes = sizes.Split(',');
                foreach (var s in _sizes)
                {
                    var size = s.Split('x');
                    Sizes.Add((int.Parse(size[0]), int.Parse(size[1])));
                }
            }
        }


    }
}