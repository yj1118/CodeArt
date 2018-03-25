using System;
using System.Collections.Generic;
using System.Runtime;
using System.IO;

namespace CodeArt.Util
{
    public static class PathUtil
    {
        public static string GetExtension(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }
            int length = path.Length;
            while (--length >= 0)
            {
                char ch = path[length];
                if (ch == '.')
                {
                    if (length != (path.Length - 1))
                    {
                        return path.Substring(length).ToLower();
                    }
                    break;
                }
                if ((ch == Path.DirectorySeparatorChar) || (ch == Path.AltDirectorySeparatorChar))
                {
                    break;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取文件名称，不是完全限定名
        /// </summary>
        /// <param name="fileName">支持绝对路径和虚拟路径</param>
        /// <returns></returns>
        public static string GetName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return fileName;
            var pos = fileName.LastIndexOf('/');
            if (pos == -1) pos = fileName.LastIndexOf('\\');
            return fileName.Substring(pos + 1);
        }

    }
}
