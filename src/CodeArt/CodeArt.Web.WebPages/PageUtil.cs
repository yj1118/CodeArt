using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.IO;

using CodeArt.Util;
using CodeArt.IO;
using System.Web;

namespace CodeArt.Web.WebPages
{
    public static class PageUtil
    {
        public static ICodePreprocessor GetCodePreprocessor()
        {
            ICodePreprocessor reader = null;
            var config = WebPagesConfiguration.Global.PageConfig;
            if (config == null || config.CodePreprocessor == null)
            {
                reader = CodePreprocessorFactory.Create();
            }
            else
            {
                reader = config.CodePreprocessor.GetInstance<ICodePreprocessor>();
                if (reader == null) throw new WebException("ICodePreprocessor 定义错误!");
            }
            return reader;
        }

        private static string ActualGetRawCode(string virtualPath)
        {
            var reader = GetCodePreprocessor();
            return reader.ReadCode(virtualPath);
        }

        private static Func<string, string> _getRawCode = LazyIndexer.Init<string, string>((virtualPath) =>
        {
            return ActualGetRawCode(virtualPath);
        });


        /// <summary>
        /// release模式下缓存，debug模式下不缓存
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public static string GetRawCode(string virtualPath)
        {
#if (DEBUG)
            return ActualGetRawCode(virtualPath);
#endif

#if (!DEBUG)
            return _getRawCode(virtualPath);
#endif
        }

        /// <summary>
        /// 计算以<paramref name="sourcePath"/>为当前路径，<paramref name="relativePath"/>为相对路径的从根开始的虚拟路径
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static string MapVirtualPath(string sourcePath, string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                throw new ArgumentException("参数relativePath为空");
            if (relativePath[0] == '/') return relativePath;//已经是从根开始的虚拟路径

            Stack<string> targets = new Stack<string>(sourcePath.Split('/'));
            if (sourcePath[sourcePath.Length - 1] != '/')
                targets.Pop();//不是以目录做结尾，那么弹出最后一个页面段，保持整个栈存放的都是目录
            var relatives = relativePath.Split('/');
            for (var i = 0; i < relatives.Length; i++)
            {
                var seg = relatives[i];
                if (i == relatives.Length - 1)
                    targets.Push(seg);
                else if (seg == "..")
                    targets.Pop();
                else
                    targets.Push(seg);
            }

            StringBuilder virtualPath = new StringBuilder();
            while (targets.Count > 1) //最后一个为""
            {
                virtualPath.Insert(0, targets.Pop());
                virtualPath.Insert(0, "/");
                
            }
            return virtualPath.ToString();
        }

        #region 监视文件

        /// <summary>
        /// 监视网站根目录（包括子目录）下的文件
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="handler"></param>
        public static FileSystemWatcher WatchFiles(string extension, Action<FileInfo, string> handler)
        {
            return WatchFiles(WebUtil.GetAbsolutePath(), extension, handler);
        }

        /// <summary>
        /// 监视文件
        /// 因为vs对文件的保存很特别（应该是为了数据安全），它保存的时候：
        ///1. 建立临时文件A，把修改的结果保存到A；
        ///2. 建立临时文件B，把原文件内容保存到B，删除原文件；
        ///3. 把A重命名到原文件名；
        ///4. 删除临时文件B。
        ///这个保存过程就没有修改原文件，所以用LastWrite监视原文件没有效果。
        ///想要监视这个修改，根据它的步骤有多种办法。比如你监视 "a.txt*" 就会发现它修改了一个 "a.txt~xxxxxxxxx.TMP" 的文件，
        ///或者合并使用NotifyFilters.FileName，用Renamed事件能看到一个临时文件改名成a.txt，
        ///或者合并使用NotifyFilters.CreationTime，这样Changed事件也能监视到
        /// </summary>
        public static FileSystemWatcher WatchFiles(string path, string extension, Action<FileInfo, string> handler)
        {
            const NotifyFilters notify = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size | NotifyFilters.CreationTime;
            return IOUtil.WatchFiles(path, string.Format("*{0}", extension), CreateWatcherHandler(handler), notify);
        }

        private static FileSystemEventHandler CreateWatcherHandler(Action<FileInfo, string> handler)
        {
            return (sender, e) =>
            {
                try
                {
                    FileInfo file = new FileInfo(e.FullPath);
                    string virtualPath = WebUtil.MapVirtualPath(file.FullName);
                    handler(file, virtualPath);
                }
                catch (Exception ex)
                {
                    //写日志
                }
            };
        }

        #endregion

    }
}