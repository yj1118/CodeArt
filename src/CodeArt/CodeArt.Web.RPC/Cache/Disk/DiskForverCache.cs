//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Caching;
//using System.Text;
//using System.Threading.Tasks;
//using System.Configuration;
//using System.IO;
//using System.Threading;

//using CodeArt.Concurrent;
//using CodeArt.IO;


//namespace CodeArt.Web.Mobile
//{
//    [SafeAccess]
//    public class DiskForverCache : ICache
//    {
//        private string _folder;

//        private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="cacheKey">通过该键值在配置文件中找到对应的缓存目录</param>
//        private DiskForverCache(string cacheKey)
//        {
//            _folder = ConfigurationManager.AppSettings[cacheKey];
//            if (_folder == null) throw new ApplicationException("没有配置缓存目录" + cacheKey);
//        }

//        /// <summary>
//        /// 尝试从缓冲区中获取数据
//        /// </summary>
//        /// <param name="cacheKey"></param>
//        /// <param name="value"></param>
//        /// <returns></returns>
//        public bool TryGet(string uniqueKey, out string value)
//        {
//            bool result = false;
//            try
//            {
//                _lock.EnterReadLock();
//                string fileName = Path.Combine(_folder, uniqueKey).BalancedPath(_folder);
//                if (!File.Exists(fileName))
//                {
//                    value = null;
//                    result = false;
//                }
//                else
//                {
//                    value = File.ReadAllText(fileName);
//                    result = true;
//                }
//            }
//            catch(Exception)
//            {
//                throw;
//            }
//            finally
//            {
//                _lock.ExitReadLock();
//            }
//            return result;
//        }

//        /// <summary>
//        /// 追加或修改缓冲对象
//        /// </summary>
//        /// <param name="uniqueKey"></param>
//        /// <param name="value"></param>
//        /// <returns>返回执行完修改操作后，缓冲区中对应的数据</returns>
//        public bool AddOrUpdate(string uniqueKey, string value)
//        {
//            bool result = false;
//            try
//            {
//                _lock.EnterWriteLock();
//                string fileName = Path.Combine(_folder, uniqueKey).BalancedPath(_folder);
//                if (!File.Exists(fileName))
//                {
//                    value = null;
//                    result = false;
//                }
//                else
//                {
//                    value = File.ReadAllText(fileName);
//                    result = true;
//                }
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//            finally
//            {
//                _lock.ExitWriteLock();
//            }
//            return result;

//            CacheItemPolicy policy = GetPolicy();
//            _buffer.Set(uniqueKey, value, policy);
//            return true;
//        }

//        private CacheItemPolicy GetPolicy()
//        {
//            CacheItemPolicy policy = new CacheItemPolicy();
//            policy.SlidingExpiration = TimeSpan.FromMinutes(10);
//            return policy;
//        }

//        public string Remove(string uniqueKey)
//        {
//            return _buffer.Remove(uniqueKey) as string;
//        }

//        public void Clear()
//        {
//            foreach (var item in _buffer)
//            {
//                _buffer.Remove(item.Key);
//            }
//        }


//        public static readonly MemoryCache Instance = new MemoryCache();

//    }
//}
