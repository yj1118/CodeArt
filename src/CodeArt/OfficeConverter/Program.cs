using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using CodeArt;
using CodeArt.IO;
using CodeArt.Office;
using CodeArt.DTO;
using CodeArt.Util;
using CodeArt.AppSetting;

namespace OfficeConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                AppInitializer.Initialize();

                var fileName = args[0];
                var folder = args[1];  //转换的图片存放的目录
                var progressFileName = args[2];  //记录进度的文件

                DTObject gt = DTObject.Create();

                using (var doc = DocumentFactory.Create(fileName))
                {
                    if (doc != null)
                    {
                        var token = new CancelToken();
                        var range = GetRange(folder, doc);
                        doc.ExtractImages(range.Index, range.Count, (pageIndex, content, g) =>
                        {
                            SavePage(folder, pageIndex, content, token, g);
                            RaiseCallback(progressFileName, g, gt);  //更新进度
                    }, token);

                        RaiseCallback(progressFileName, new Progress(doc.PageCount, doc.PageCount), gt);
                    }
                }
            }
            catch(Exception ex)
            {
                var errorFileName = args[3];  //记录错误的文件
                WriteError(errorFileName,ex);
            }
            finally
            {
                AppInitializer.Cleanup();
            }
        }

        private static void WriteError(string errorFileName,Exception ex)
        {
            using (var fs = new FileStream(errorFileName, FileMode.Open, FileAccess.Write, FileShare.Read))
            {
                fs.SetLength(0);
                using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
                {
                    writer.Write(ex.GetCompleteMessage());
                    writer.Write(ex.GetCompleteStackTrace());
                }
            }
        }

        /// <summary>
        /// 进度写入到文件，以便外界读取
        /// </summary>
        /// <param name="progressFileName"></param>
        /// <param name="g"></param>
        /// <param name="gt"></param>
        private static void RaiseCallback(string progressFileName, Progress g, DTObject gt)
        {
            gt["completedCount"] = g.CompletedCount;
            gt["pageCount"] = g.PageCount;
            var code = gt.GetCode();
            using (var fs = new FileStream(progressFileName, FileMode.Open, FileAccess.Write, FileShare.Read))
            {
                fs.SetLength(0);
                using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
                {
                    writer.Write(code);
                }
            }
        }

        private static void SavePage(string folder, int pageIndex, Stream content, CancelToken token, Progress g)
        {
            if (token.IsCanceled) return;
            lock (_syncObject)
            {
                string fileName = Path.Combine(folder, GetPageName(pageIndex + 1));
                IOUtil.Delete(fileName);
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    content.Seek(0, SeekOrigin.Begin);
                    using (var temp = SegmentReaderSlim.Borrow(SegmentSize.KB128))
                    {
                        var reader = temp.Item;
                        reader.Read(content, (seg) =>
                        {
                            var bytes = seg.GetContent();
                            fs.Write(bytes, 0, bytes.Length);
                            if (token.IsCanceled) return false;
                            return true;
                        });
                    }
                }
            }
        }

        private static string GetPageName(int number)
        {
            return string.Format("{0:D6}.png", number);
        }

        private static object _syncObject = new object();

        private static (int Index, int Count) GetRange(string folder, IDocument doc)
        {
            lock (_syncObject)
            {
                var pageFiles = IOUtil.GetAllFiles(folder).OrderBy((t) => t);
                var length = pageFiles.Count();
                int index = 0;
                int count = doc.PageCount;
                if (length > 0)
                {
                    var lastFileName = Path.Combine(folder, GetPageName(length));
                    //提取还未被提取的页，为了防止最后一个页是损坏的，我们删除最后一页，再提取
                    IOUtil.Delete(lastFileName);
                    index = length - 1; //因为删除了最后一页，所以-1
                    count = doc.PageCount - index;
                }
                return (index, count);
            }
        }

    }
}
