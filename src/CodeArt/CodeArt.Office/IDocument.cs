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
        bool ExtractImages(int index, int count, Action<int, Stream, Progress> callBack, CancelToken token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <param name="getFileName">传入文件序号，需要返回文件的保存名称</param>
        /// <param name="callBack"></param>
        /// <param name="token"></param>
        bool SaveImages(int index, int count, Func<int,string> getFileName, Action<int, string, Progress> callBack, CancelToken token);

        void Cancel();

        /// <summary>
        /// 以线程安全的方式使用doc
        /// </summary>
        void SafeUse(Action<IDocument> action);

        /// <summary>
        /// 是否只有一个线程在使用该doc
        /// </summary>
        int ReferenceTimes
        {
            get;
        }

    }
}
