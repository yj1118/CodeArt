using CodeArt.IO;
using System;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Web;

namespace CodeArt.Web.WebPages
{
    public sealed class HttpCompressor : ICompressor
    {
        /// <summary>
        /// 判断浏览器是否支持压缩模式
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool IsAccepted(WebPageContext context)
        {
            return context.CompressionType != HttpCompressionType.None;
        }

        /// <summary>
        /// 设置响应的头，以便浏览器识别该压缩模式
        /// </summary>
        /// <param name="context"></param>
        public void SetEncoding(WebPageContext context)
        {
            var compressionType = context.CompressionType;
            var response = context.Response;

            if (compressionType != HttpCompressionType.None)
            {
                response.AppendHeader("Content-Encoding", compressionType.ToString().ToLower());
                //if (compressionType == CompressionType.GZip)
                //{
                //    response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
                //}
                //else
                //{
                //    response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
                //}
            }
        }

        /// <summary>
        /// 压缩流
        /// </summary>
        /// <param name="source">需要压缩的流</param>
        /// <param name="target">需要写入压缩内容的目标流</param>
        public void Compress(WebPageContext context, Stream source, Stream target)
        {
            var compressionType = context.CompressionType;

            int bufferSize = 8000;

            Stream compressStream = null;

            try
            {
                if (compressionType == HttpCompressionType.GZip)
                {
                    compressStream = new GZipStream(target, CompressionMode.Compress, true);
                }
                else if (compressionType == HttpCompressionType.Deflate)
                {
                    compressStream = new DeflateStream(target, CompressionMode.Compress, true);
                }
                else{
                    compressStream = target;
                }

                byte[] buffer = new byte[bufferSize];
                int retval = source.ReadPro(buffer, 0, bufferSize);
                while (retval == bufferSize)
                {
                    compressStream.Write(buffer, 0, bufferSize);
                    compressStream.Flush();
                    retval = source.ReadPro(buffer, 0, bufferSize);
                }
                // 写入剩余的字节。
                compressStream.Write(buffer, 0, retval);
                compressStream.Close();//一定要close才有效，否则gzip的字节数出错
            }
            catch (Exception ex)
            {
                if (compressStream != null) compressStream.Close();
                throw ex;
            }
        }

        public static readonly ICompressor Instance = new HttpCompressor();

    }
}
