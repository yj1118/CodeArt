using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using CodeArt.IO;

using CodeArt.DTO;
using CodeArt.Concurrent;
using System.Threading;

namespace CodeArt.Caching
{
	public class DiskCache
    {
        private string _path;

        private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        private string GetFileName(string key)
        {
            return Path.Combine(_path, key);
        }

        public DiskCache(string path)
        {
            _path = path;
        }

        public DTObject Get(string key, Func<string, DTObject> generate)
        {
            DTObject result = null;

            string fileName = GetFileName(key);

            _lock.Read(() =>
            {
                if (File.Exists(fileName))
                {
                    Read();
                }
            });

            if (result != null) return result;


            _lock.Write(() =>
            {
                if (File.Exists(fileName))
                {
                    Read();
                }
                else
                {
                    var target = generate(key);
                    var code = target.GetCode(false, false);
                    IO.IOUtil.CreateFileDirectory(fileName);
                    File.WriteAllText(fileName, code);
                    result = target;
                    DiskCacheLog.Current?.Write(key, DiskCacheLog.Status.NotFromCache);
                }

            });


            void Read()
            {
                string code = File.ReadAllText(fileName);
                result = DTObject.Create(code);
                DiskCacheLog.Current?.Write(key, DiskCacheLog.Status.FromCache);
            }

          
            return result;
        }

        public void Remove(Func<string, bool> canRemove)
        {
            _lock.Write(() =>
            {
                var fileNames = IOUtil.GetDirectFiles(_path);
                foreach (var fileName in fileNames)
                {
                    var key = IOUtil.GetName(fileName);
                    if (canRemove(key))
                    {
                        IOUtil.Delete(fileName);
                        DiskCacheLog.Current?.Write(key, DiskCacheLog.Status.RemoveCache);
                    }
                }
            });
        }

        public void Remove(string key)
        {
            _lock.Write(() =>
            {
                var fileName = GetFileName(key);
                IOUtil.Delete(fileName);
                DiskCacheLog.Current?.Write(key, DiskCacheLog.Status.RemoveCache);
            });
        }

        public bool Exist(string key)
        {
            bool exist = false;
            _lock.Read(() =>
            {
                var fileName = GetFileName(key);
                exist = File.Exists(fileName);
            });
            return exist;
        }

        public void Clear()
        {
            _lock.Write(() =>
            {
                IOUtil.ClearDirectory(_path);
                DiskCacheLog.Current?.Write("*", DiskCacheLog.Status.RemoveCache);
            });
        }
    }


    public class DiskCacheLog
    {

        private DiskCacheLog()
        {

        }

        private List<(string, Status)> _items = new List<(string, Status)>();

        public void Write(string key, Status status)
        {
            lock(_items)
                _items.Add((key, status));
        }

        public Status GetLastStatus(string key)
        {
            var statuses = GetStatus(key);
            if (statuses.Count() == 0) return Status.None;
            return statuses.Last();
        }

        public IEnumerable<Status> GetStatus(string key)
        {
            var items = _items.FindAll((t) => t.Item1 == key);
            return items.Select((t) =>{ return t.Item2; });
        }

        public enum Status : byte
        {
            None = 0,
            /// <summary>
            /// 指示标识数据从缓存中加载
            /// </summary>
            FromCache = 1,
            /// <summary>
            /// 指示标识数据没有从缓存中加载
            /// </summary>
            NotFromCache = 2,
            /// <summary>
            /// 指示标识数据从缓存中移除
            /// </summary>
            RemoveCache = 3
        }


        internal static DiskCacheLog Current = null;


        public static void Using(Action<DiskCacheLog> action)
        {
            var log =  Current = new DiskCacheLog();
            action(log);
            Current = null;
        }


    }

}
