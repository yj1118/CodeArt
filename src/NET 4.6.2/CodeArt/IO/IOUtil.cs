using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Cryptography;


namespace CodeArt.IO
{
    public static class IOUtil
    {
        public static void SetEveryoneFullControl(string fileName)
        {
            FileInfo fi = new FileInfo(fileName);
            if (fi.Exists)
            {
                FileSecurity security = new FileSecurity();
                security.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow));
                fi.SetAccessControl(security);
            }
        }

        public static string GetExtension(string fileName)
        {
            var dot = fileName.LastIndexOf(".");
            if (dot == -1) return string.Empty;
            var ext = fileName.Substring(dot + 1);
            return ext;
        }

        /// <summary>
        /// 创建文件所在的父目录
        /// </summary>
        /// <param name="fileName"></param>
        public static void CreateFileDirectory(string fileName)
        {
            FileInfo fi = new FileInfo(fileName);
            if (!fi.Directory.Exists) fi.Directory.Create();
        }

        public static string GetFileDirectory(string fileName)
        {
            FileInfo fi = new FileInfo(fileName);
            return fi.Directory.FullName;
        }

        #region 删除操作

        /// <summary>
        /// 清空目录，但不删除目录本身
        /// </summary>
        /// <param name="path"></param>
        public static void ClearDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Delete(path, false);
            }
        }

        /// <summary>
        /// 删除目录或文件
        /// </summary>
        /// <param name="path"></param>
        public static void Delete(string path)
        {
            Delete(path, true);
        }

        private static void Delete(string path, bool self)
        {
            if (File.Exists(path)) DeleteFile(path);
            else if (Directory.Exists(path))
            {
                //先删除该目录下的文件
                string[] fs = Directory.GetFiles(path);
                foreach (string f in fs) DeleteFile(f);

                //再删除该目录下的目录
                string[] dis = Directory.GetDirectories(path);
                foreach (string d in dis) Delete(d);

                //最后删除目录本身
                if (self) Directory.Delete(path);
            }
        }

        public static string[] GetAllFiles(string path)
        {
            List<string> files = new List<string>();
            if (Directory.Exists(path))
            {
                //收集当前目录的文件
                string[] fs = Directory.GetFiles(path);
                files.AddRange(fs);

                //再收集该目录下的文件
                string[] dis = Directory.GetDirectories(path);
                foreach (string d in dis)
                {
                    files.AddRange(GetAllFiles(d));
                }
            }
            return files.ToArray();
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileName"></param>
        private static void DeleteFile(string fileName)
        {
            try
            {
                FileInfo fi = new FileInfo(fileName);
                //判断文件属性是否只读?是则修改为一般属性再删除
                if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                    fi.Attributes = FileAttributes.Normal;
                File.Delete(fileName);
            }
            catch(DirectoryNotFoundException)
            {
                //由于内部使用DeleteFile，
                //那么抛出异常DirectoryNotFoundException一定是其他线程或者程序刚好在运行
                //DeleteFile时把文件删除了
                //这种错误我们直接吞噬异常即可
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        /// <summary>
        /// 均衡路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string BalancedPath(this string fileName,string folder)
        {
            MD5 md = new MD5CryptoServiceProvider();
            var code = BitConverter.ToString(md.ComputeHash(new UnicodeEncoding().GetBytes(fileName)));

            var pos = fileName.LastIndexOf('.');
            string extension = pos > -1 ? fileName.Substring(pos) : string.Empty;

            var temp = code.Split('-');
            code = code.Replace("-", string.Empty);
            return Path.Combine(folder, temp[0], temp[1], string.Format("{0}{1}", code, extension));
        }


        public static void WatchFiles(string path, string filter, FileSystemEventHandler handler)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.IncludeSubdirectories = true;
            watcher.Filter = filter;
            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size;

            watcher.Created += handler;
            watcher.Changed += handler;
            watcher.Deleted += handler;
            watcher.EnableRaisingEvents = true;
        }
    }
}
