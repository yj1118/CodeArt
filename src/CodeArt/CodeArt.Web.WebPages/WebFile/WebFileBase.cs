using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web.Compilation;

using System.Web;
using System.IO;

using CodeArt.AOP;
using CodeArt.Runtime;

namespace CodeArt.Web.WebPages
{
    [AspectRemove(typeof(WebPageInitializer))]
    public abstract class WebFileBase : WebPage
    {
        protected override void ProcessGET()
        {
            var range = HttpRequestRange.New(this.Request);
            if (!range.IsEmpty())
            {
                var size = GetFileSize();
                long offset = 0;
                long count = 0;
                //分段读取
                range.Process(this.Response, size, out offset, out count);

                byte[] buffer = ReadFileBuffer(offset, (int)count);
                this.Response.BinaryWrite(buffer);
                this.Response.Flush();
            }
            else
            {
                //普通文件
                var size = GetFileSize();
                int bufferSize = GetBufferSize(size);
                long offset = 0;
                while (offset < size)
                {
                    byte[] buffer = ReadFileBuffer(offset, bufferSize);
                    this.Response.BinaryWrite(buffer);
                    this.Response.Flush();
                    offset += buffer.Length;
                }
            } 
        }

        /// <summary>
        /// 根据文件大小，计算缓冲区大小
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        protected abstract int GetBufferSize(long size);

        protected abstract long GetFileSize();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset">读取流的偏移量</param>
        /// <param name="count">读取的最大字节数</param>
        /// <returns>返回实际读取的字节数</returns>
        protected abstract byte[] ReadFileBuffer(long offset, int count);

        protected override void ProcessPOST()
        {
            SaveFiles(this.Request.InputStream, this.Request.Files);
        }

        /// <summary>
        /// 处理上传的文件
        /// </summary>
        /// <param name="files"></param>
        protected abstract void SaveFiles(Stream input, HttpFileCollection files);

    }
}
