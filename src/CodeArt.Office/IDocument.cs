using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CodeArt.Office
{
    public interface IDocument: IDisposable
    {
        int PageCount
        {
            get;
        }

        /// <summary>
        /// 提取指定范围的页
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <param name="callBack"></param>
        /// <param name="token"></param>
        void ExtractImages(int index, int count, Action<int, Stream, Progress> callBack, CancelToken token);

    }
}
