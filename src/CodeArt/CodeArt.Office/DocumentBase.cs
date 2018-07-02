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

        public void ExtractImages(int index, int count, Action<int, Stream, Progress> callBack, CancelToken token)
        {
            var total = index + count;
            for (var i = index; i < total; i++)
            {
                using (var content = ExtractImage(i))
                {
                    var g = new Progress(this.PageCount, i + 1);
                    callBack(i, content, g);
                }
                if (token.IsCanceled) break;
            }
        }

        protected abstract Stream ExtractImage(int pageIndex);

    }
}
