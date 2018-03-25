using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using CodeArt.DTO;
using CodeArt.Concurrent.Pattern;
using CodeArt.IO;
using System.Diagnostics;
using CodeArt.Concurrent;

namespace CodeArt.Office
{
    /// <summary>
    /// 文档转换器
    /// </summary>
    public class DocumentConverter
    {
        public string FileName
        {
            get;
            private set;
        }

        public CancelToken Token
        {
            get;
            private set;
        }

        public string Folder
        {
            get;
            private set;
        }


        public event Action<int, Progress> Callback;

        private void RaiseCallback(int pageIndex, Progress progress)
        {
            if (this.Callback != null)
                this.Callback(pageIndex, progress);
        }

        //public event Action<Progress> Completed;

        //private void RaiseCompleted(Progress progress)
        //{
        //    if (this.Completed != null)
        //        this.Completed(progress);
        //}


        private string _progressFileName;
        private string _errorFileName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="folder">存放转换后文件的目录</param>
        /// <param name="token"></param>
        public DocumentConverter(string fileName, string folder, CancelToken token)
        {
            this.FileName = fileName;
            this.Folder = folder;
            this.Token = token;
            _progressFileName = GetProgressFileName(folder);
            _errorFileName = GetErrorFileName(folder);
        }

        private FileSystemWatcher GetFileProgress()
        {
            if (!File.Exists(_progressFileName))
            {
                IOUtil.CreateFileDirectory(_progressFileName);
                IOUtil.CreateFile(_progressFileName);
            }
            return IOUtil.WatchFiles(IOUtil.GetFileDirectory(_progressFileName), "*.pro", OnProgressChanged, NotifyFilters.LastWrite);
        }

        private void OnProgressChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (this.Token.IsCanceled)
                {
                    _future.SetResult(false);
                    return;
                }

                var result = GetProgress(_progressFileName);
                if (!result.Success) return; //获取失败，直接返回

                var p = result.progress;
                if (p.CompletedCount == 0)
                {
                    _future.SetResult(false);
                    return;
                }
                RaiseCallback(p.CompletedCount - 1, p);
                if (p.IsCompleted)
                {
                    _future.SetResult(true);
                }
            }
            catch(Exception ex)
            {
                _future.SetError(ex);
            }
        }

        private FileSystemWatcher GetFileError()
        {
            if (!File.Exists(_errorFileName))
            {
                IOUtil.CreateFileDirectory(_errorFileName);
                IOUtil.CreateFile(_errorFileName);
            }
            return IOUtil.WatchFiles(IOUtil.GetFileDirectory(_errorFileName), "*.err", OnErrorChanged, NotifyFilters.LastWrite);
        }

        private void OnErrorChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                string code = string.Empty;
                using (var fs = new FileStream(_errorFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var reader = new StreamReader(fs))
                    {
                        code = reader.ReadToEnd();
                    }
                }
                _future.SetError(new ApplicationException(code));
            }
            catch (Exception ex)
            {
                _future.SetError(ex);
            }
        }

        private void TryDeleteError()
        {
            try
            {
                string code = string.Empty;
                using (var fs = new FileStream(_errorFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var reader = new StreamReader(fs))
                    {
                        code = reader.ReadToEnd();
                    }
                }
                if(code.Length == 0)
                {
                    File.Delete(_errorFileName);
                }
                
            }
            catch (Exception ex)
            {
                //写日志
            }
        }

        private Future<bool> _future;

        public bool Convert()
        {
            IOUtil.CreateDirectory(this.Folder);
            IOUtil.ClearDirectory(this.Folder);

            var success = false;
            using (var pw = GetFileProgress())
            {
                using (var ew = GetFileError())
                {
                    if (IsCompleted())
                    {
                        success = true;
                    }
                    else
                    {
                        try
                        {
                            RaiseConverting(this.FileName);
                            using (var process = GetProcess())
                            {
                                _future = new Future<bool>();
                                _future.Start();
                                process.Start();
                                _future.Wait();
                            }
                            success = _future.Result;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            TryDeleteError();
                            RaiseConverted(this.FileName);
                        }
                    }
                }
                return success;
            }
        }


        private Process GetProcess()
        {
            var process = new Process();
            process.StartInfo.FileName = ConverterFileName;

            // 启动参数设置
            process.StartInfo.Arguments = string.Format("{0} {1} {2} {3}", this.FileName, this.Folder, _progressFileName, _errorFileName);

            // 启动方式设置
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            return process;
        }

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

        private static string GetPageName(int number)
        {
            return string.Format("{0:D6}.png", number);
        }

        private static object _syncObject = new object();


        public bool IsCompleted()
        {
            var g = GetProgress(_progressFileName).progress;
            return g.IsCompleted;
        }

        public static bool IsCompleted(string folder)
        {
            var fileName = GetProgressFileName(folder);
            if (!File.Exists(fileName)) return false;
            var g = GetProgress(fileName).progress;
            return g.IsCompleted;
        }

        public static string GetProgressFileName(string folder)
        {
            return Path.Combine(folder, "progress.pro");
        }

        public static string GetErrorFileName(string folder)
        {
            return Path.Combine(folder, "error.err");
        }

        private static (Progress progress, bool Success) GetProgress(string progressFileName)
        {
            string code = string.Empty;
            using (var fs = new FileStream(progressFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var reader = new StreamReader(fs))
                {
                    code = reader.ReadToEnd();
                }
            }
            if (code.Length == 0) return (new Progress(0, 0), false);  //由于每次写入文件，会先把流重置为0，所以这时候读取文件就可能会读取空字符串
            var dto = DTObject.Create(code);
            var pageCount = dto.GetValue<int>("pageCount");
            var completedCount = dto.GetValue<int>("completedCount");
            return (new Progress(pageCount, completedCount), true);
        }

        #region 全局事件

        private static ActionPipelineSlim _queueEvent = new ActionPipelineSlim();


        private static List<string> _cacheFileNames = new List<string>();

        public static event Action Converting;
        private static void RaiseConverting(string fileName)
        {
            lock (_cacheFileNames)
            {
                _cacheFileNames.Add(fileName);
            }

            _queueEvent.Queue((complete) =>
            {
                if (Converting != null)
                    Converting();
                complete();
            });
        }

        /// <summary>
        /// 文件已处理完毕，这有可能是取消处理或者处理完成触发的事件
        /// </summary>
        public static event Action Converted;
        private static void RaiseConverted(string fileName)
        {
            bool empty = false;
            lock (_cacheFileNames)
            {
                _cacheFileNames.Remove(fileName);
                empty = _cacheFileNames.Count == 0;
            }

            _queueEvent.Queue((complete) =>
            {
                if (empty)
                {
                    if (Converted != null)
                        Converted();
                }
                complete();
            });
        }

        #endregion

        private static string ConverterFileName = string.Format("{0}OfficeConverter.exe", AppDomain.CurrentDomain.BaseDirectory);
    }
}
