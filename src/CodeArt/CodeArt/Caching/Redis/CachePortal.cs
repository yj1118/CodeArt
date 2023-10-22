using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using CodeArt.DTO;
using StackExchange.Redis;

namespace CodeArt.Caching.Redis
{
    public class CachePortal : IDisposable
    {
        #region 连接

        private ConnectionMultiplexer _connection;
        private IDatabase _database;


        private void InitConnection()
        {
            var connectionString = Configuration.Current.CacheConfig.GetConnectionString(this.ConnectionKey);
            ArgumentAssert.IsNotNullOrEmpty(connectionString, "cache-connectionString");
            _connection = ConnectionMultiplexer.Connect(connectionString);
            _database = _connection.GetDatabase();
        }


        public string ConnectionKey
        {
            get;
            private set;
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }

        #endregion

        public int ExpireMinutes
        {
            get;
            private set;
        }

        private Func<string, DTObject> _creator;

        private string _category;

        private string GetStorageKey(string key)
        {
            if (string.IsNullOrEmpty(_category)) return key;
            return string.Format("{0}-{1}",_category,key);
        }

        public CachePortal(string connectionKey, int expireMinutes, Func<string, DTObject> creator)
            : this(connectionKey, null, expireMinutes, creator)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionKey"></param>
        /// <param name="expireMinutes">0表示永久缓存</param>
        /// <param name="creator"></param>
        public CachePortal(string connectionKey,string category, int expireMinutes, Func<string, DTObject> creator)
        {
            this.ConnectionKey = connectionKey;
            _category = category;
            this.ExpireMinutes = expireMinutes;
            _creator = creator;
            InitConnection();
        }

        /// <summary>
        /// 该构造常用于只删除缓存的场景
        /// </summary>
        /// <param name="connectionKey"></param>
        /// <param name="expireMinutes"></param>
        public CachePortal(string connectionKey)
           : this(connectionKey, null, 0, null)
        {
        }

        /// <summary>
        /// 该构造常用于只删除缓存的场景
        /// </summary>
        /// <param name="connectionKey"></param>
        /// <param name="expireMinutes"></param>
        public CachePortal(string connectionKey, string category)
           : this(connectionKey, category, 0, null)
        {
        }

        public void Set(string key, string value)
        {
            var storageKey = GetStorageKey(key);
            if (this.ExpireMinutes > 0)
            {
                _database.StringSet(storageKey, value, TimeSpan.FromMinutes(this.ExpireMinutes));
            }
            else
            {
                _database.StringSet(storageKey, value);
            }
        }

        public void Update(string key, string value)
        {
            if (Exists(key))
            {
                lock (_syncObject)
                {
                    if (Exists(key))
                    {
                        Set(key, value);
                    }
                }
            }
        }

        private object _syncObject = new object();

        public DTObject Get(string key)
        {
            //如果没有，就创建缓存
            if (!Exists(key))
            {
                lock (_syncObject)
                {
                    if (!Exists(key))
                    {
                        var value = _creator(key);
                        if (value.IsEmpty()) return DTObject.Empty;
                        Set(key, value.GetCode(false, false));
                    }
                }
            }

            //如果有，就取数据
            var storageKey = GetStorageKey(key);
            var data = _database.StringGet(storageKey);

            return DTObject.Create(data.ToString());
        }

        public bool Exists(string key)
        {
            var storageKey = GetStorageKey(key);
            return _database.KeyExists(storageKey);
        }

        public bool Remove(string key)
        {
            var storageKey = GetStorageKey(key);
            return _database.KeyDelete(storageKey);
        }

        /// <summary>
        /// 使用默认的缓存门户
        /// </summary>
        /// <param name="category"></param>
        /// <param name="expireMinutes"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        public static CachePortal UseDefault(string category, int expireMinutes, Func<string, DTObject> creator)
        {
            return new CachePortal("default", category, expireMinutes, creator);
        }


        /// <summary>
        /// 用户只删除缓存的场景
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static CachePortal UseDefault(string category)
        {
            return new CachePortal("default", category);
        }

    }
}