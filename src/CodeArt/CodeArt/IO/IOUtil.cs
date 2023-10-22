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

        /// <summary>
        /// 获取没有扩展名的文件名称
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetNameWithNoExtension(string fileName)
        {
            var name = GetName(fileName);
            var dot = name.LastIndexOf(".");
            if (dot == -1) return name;
            return name.Substring(0, dot);
        }

        /// <summary>
        /// 获取文件名称，就算名称是全路径也可以获取到
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetName(string fileName)
        {
            var pos = fileName.LastIndexOf("\\");
            return pos == -1 ? fileName : fileName.Substring(pos + 1);
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

        public static void CreateDirectory(string path)
        {
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static string GetFileDirectory(string fileName)
        {
            FileInfo fi = new FileInfo(fileName);
            return fi.Directory.FullName;
        }

        public static void CreateFile(string fileName)
        {
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {

            }
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

        /// <summary>
        /// 获得路径<paramref name="path"/>下的所有文件，包括子目录的文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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

        public static string[] GetDirectFiles(string path)
        {
            return GetFiles(path, true, (t) =>
            {
                return true;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="onlyDirect">是否只读一级</param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static string[] GetFiles(string path,bool onlyDirect, Func<string,bool> filter)
        {
            List<string> files = new List<string>();
            if (Directory.Exists(path))
            {
                //收集当前目录的文件
                string[] fs = Directory.GetFiles(path);
                foreach(var f in fs)
                {
                    var r = filter(f);
                    if(r) files.Add(f);
                }

                if (!onlyDirect)
                {
                    //再收集该目录下的文件
                    string[] dis = Directory.GetDirectories(path);
                    foreach (string d in dis)
                    {
                        files.AddRange(GetFiles(d,onlyDirect, filter));
                    }
                }
            }
            return files.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="onlyDirect">是否只读一级目录</param>
        /// <param name="extensions"></param>
        /// <returns></returns>
        public static string[] GetFiles(string path, bool onlyDirect, string extensions)
        {
            var es = extensions.Split(',').Select((t)=>
            {
                if (t.StartsWith("*.")) return t.Substring(2);
                return t;
            });
            return GetFiles(path, onlyDirect, (fileName) =>
             {
                 var t = IOUtil.GetExtension(fileName);
                 return es.Contains(t, StringComparer.OrdinalIgnoreCase);
             });
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
        /// <param name="fileName">文件名（不带路径）</param>
        /// <param name="folder">需要存放的目录，会在此目录下均衡</param>
        /// <returns></returns>
        public static string BalancedPath(this string fileName, string folder)
        {
            MD5 md = new MD5CryptoServiceProvider();
            var code = BitConverter.ToString(md.ComputeHash(new UnicodeEncoding().GetBytes(fileName)));

            var pos = fileName.LastIndexOf('.');
            string extension = pos > -1 ? fileName.Substring(pos) : string.Empty;

            var temp = code.Split('-');
            code = code.Replace("-", string.Empty);
            return Path.Combine(folder, temp[0], temp[1], string.Format("{0}{1}", code, extension));
        }


        public static FileSystemWatcher WatchFiles(string path, string filter, FileSystemEventHandler handler, NotifyFilters notify)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.IncludeSubdirectories = true;
            watcher.Filter = filter;
            watcher.Path = path;
            watcher.NotifyFilter = notify;

            watcher.Created += handler;
            watcher.Changed += handler;
            watcher.Deleted += handler;
            watcher.Renamed += (s, e) => { handler(s, new FileSystemEventArgs(e.ChangeType, e.FullPath, e.Name)); };
            watcher.EnableRaisingEvents = true;
            return watcher;
        }
    }
}
