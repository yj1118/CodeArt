using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CodeArt.Office
{
    public abstract class DocumentBase : IDocument
    {
        public abstract int PageCount
        {
            get;
        }

        public abstract void Dispose();

        private volatile bool _isCancelled;

        public bool IsCancelled
        {
            get
            {
                return _isCancelled;
            }
        }

        public void Cancel()
        {
            _isCancelled = true;
        }

        /// <summary>
        /// 以线程安全的方式使用doc
        /// </summary>
        public void SafeUse(Action<IDocument> action)
        {
            lock(_syncObject)
            {
                action(this);
            }
        }

        private int _referenceTimes = 0;

        /// <summary>
        /// 该对象通过factory被引用的次数
        /// </summary>
        public int ReferenceTimes
        {
            get
            {
                return _referenceTimes;
            }
            internal set
            {
                lock (_syncObject)
                {
                    _referenceTimes = value;
                }
            }
        }


        public bool ExtractImages(int index, int count, Action<int, Stream, Progress> callBack, CancelToken token)
        {
            var total = index + count;
            for (var i = index; i < total; i++)
            {
                using (var content = ExtractImage(i))
                {
                    var g = new Progress(this.PageCount, i + 1);
                    callBack(i, content, g);
                }
                if (token.IsCancelled || _isCancelled) return false;
            }
            return true;
        }

        private object _syncObject = new object();

        public bool SaveImages(int index, int count, Func<int, string> getFileName, Action<int,string, Progress> callBack, CancelToken token)
        {
            var total = index + count;
            for (var i = index; i < total; i++)
            {
                var fileName = getFileName(i);
                lock (_syncObject)  //当外界有多线程访问时，此处解决并发冲突
                {
                    if (!File.Exists(fileName))
                    {
                        SaveImage(i, fileName);
                    }
                }
                var g = new Progress(this.PageCount, i + 1);
                callBack(i, fileName, g);
                if (token.IsCancelled || _isCancelled) return false;
            }
            return true;
        }

        protected abstract Stream ExtractImage(int pageIndex);

        protected abstract void SaveImage(int pageIndex, string fileName);

    }
}
