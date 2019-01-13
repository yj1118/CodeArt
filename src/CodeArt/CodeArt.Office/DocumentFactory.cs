using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using CodeArt.IO;
using System.Threading;

namespace CodeArt.Office
{
    public static class DocumentFactory
    {
        private static Dictionary<string, DocumentProxy> _cache = new Dictionary<string, DocumentProxy>();

        private class DocumentProxy : IDocument
        {
            public int ReferenceTimes
            {
                get
                {
                    return _document.ReferenceTimes;
                }
                set
                {
                    _document.ReferenceTimes = value;
                }
            }

            private string _fileName;
            private DocumentBase _document;

            public DocumentProxy(string fileName, DocumentBase document)
            {
                _fileName = fileName;
                _document = document;
                this.ReferenceTimes = 0;
            }

            public int PageCount
            {
                get
                {
                    return _document.PageCount;
                }
            }

            public bool ExtractImages(int index, int count, Action<int, Stream, Progress> callBack, CancelToken token)
            {
                return _document.ExtractImages(index, count, callBack, token);
            }

            public bool SaveImages(int index, int count, Func<int, string> getFileName, Action<int, string, Progress> callBack, CancelToken token)
            {
                return _document.SaveImages(index, count, getFileName, callBack, token);
            }

            public void Cancel()
            {
                _document.Cancel();
            }

            public void Dispose()
            {
                this.ReferenceTimes--;
                if (this.ReferenceTimes == 0)
                {
                    _document.Dispose();
                    _cache.Remove(_fileName);
                }
            }

            public void SafeUse(Action<IDocument> action)
            {
                _document.SafeUse(action);
            }
        }
            
        /// <summary>
        /// 同一时刻，只有一个对象处理同一文件(IDocument Document,Action completed)中的completed是指当document用完后，需要调用该方法
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static IDocument Create(string fileName)
        {
           lock(_cache)
            {
                if (!_cache.TryGetValue(fileName, out var proxy))
                {
                    var doc = NewDocument(fileName);
                    if (doc == null) return null;//证明不是合法的文档
                    proxy = new DocumentProxy(fileName, doc);
                    _cache.Add(fileName, proxy);
                }
                proxy.ReferenceTimes++;
                return proxy;
            }
        }

        private static DocumentBase NewDocument(string fileName)
        {
            var e = IOUtil.GetExtension(fileName).ToLower();
            switch (e)
            {
                case "doc":
                case "docx":
                    return new Word(fileName);
                case "ppt":
                case "pptx":
                    return new PPT(fileName);
                case "pdf":
                    return new Pdf(fileName);
                case "xls":
                case "xlsx":
                    return new Excel(fileName);
            }
            return null;
        }


    }
}
