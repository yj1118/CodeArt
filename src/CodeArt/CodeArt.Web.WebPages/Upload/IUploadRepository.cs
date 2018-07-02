using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web.Compilation;

using System.Web;
using System.IO;

using CodeArt.Runtime;

namespace CodeArt.Web.WebPages
{
    /// <summary>
    /// 上传的文件存储器
    /// </summary>
    public interface IUploadRepository
    {
        /// <summary>
        /// 开始写入文件至临时目录，返回临时文件的key
        /// </summary>
        /// <returns></returns>
        string Begin(string extension);

        /// <summary>
        /// 写入临时文件
        /// </summary>
        /// <param name="tempKey"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        void Write(string tempKey, byte[] buffer, int offset, int count);

        /// <summary>
        /// 关闭临时文件的写入并且返回最终文件的key
        /// </summary>
        /// <param name="tempKey"></param>
        /// <returns></returns>
        string Close(string tempKey);

        /// <summary>
        /// 删除临时文件
        /// </summary>
        /// <param name="tempKey"></param>
        void Delete(string tempKey);
    }
}
